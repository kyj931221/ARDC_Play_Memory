using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Storage;

public class DBService : MonoBehaviour
{
    private AudioSource btCapturePlay; // �̹��� ���� ��ư
    private AudioSource btSavePlay; // ������ ���� ��ư

    public AudioClip btCapture; // �̹��� ���� ��ư �Ҹ�
    public AudioClip btSave; // ������ ���� ��ư �Ҹ�

    public InputField urlInputField; // ��Ʃ�� url
    public InputField nameInputField; // DB�� ������ �̸�
    public InputField imageUrlInputField; // �̹��� URL�� �Էµ� �ʵ�

    public RawImage cameraPreview; // ī�޶� ������
    public Button uploadImageButton; // �̹��� �Կ� �� ���ε�, URL �������� ��ư
    public Button saveDataButton; // ������ ���� ��ư

    public Text debug_Log;
    public Text debug_Error;

    private WebCamTexture webCamTexture;
    private Texture2D capturedImage;

    // Firebase ����
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    private string uploadedImageUrl = null; // ���ε�� �̹��� URL�� ������ ����

    public class Info
    {
        public float x;  // ����
        public float y;  // �浵
        public string url; // ��Ʃ�� ���� ��ũ
        public string name; // ���� DB ����
        public string imageUrl; // Firebase�� ����� �̹����� URL

        public Info() { }

        public Info(string name, float x, float y, string url, string imageUrl)
        {
            this.name = name;
            this.x = x;
            this.y = y;
            this.url = url;
            this.imageUrl = imageUrl;
        }
    }

    private void Start()
    {
        // �� ��ư�� ���� �Ҹ� ������Ʈ ����
        btCapturePlay = GetComponent<AudioSource>();
        btSavePlay = GetComponent<AudioSource>();

        // Firebase �ʱ�ȭ
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://catch-you-catch-me.appspot.com");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // ī�޶� �ʱ�ȭ
        webCamTexture = new WebCamTexture();
        cameraPreview.texture = webCamTexture;
        cameraPreview.material.mainTexture = webCamTexture;
        webCamTexture.Play();

        // Firebase Realtime Database ��Ʈ ���۷���
        //databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void UploadImageToFirebase()
    {
        btCapturePlay.PlayOneShot(btCapture);
        // ī�޶󿡼� �ؽ�ó �����͸� ������ Texture2D�� ��ȯ
        capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
        capturedImage.SetPixels(webCamTexture.GetPixels());
        capturedImage.Apply();

        // �̹��� ���ε� ����
        StartCoroutine(UploadImageAndGetUrl());
    }

    // Firebase�� �̹��� ���ε� �� URL ��������
    IEnumerator UploadImageAndGetUrl()
    {
        // 1. Firebase Storage�� �̹��� ���ε�

        byte[] imageData = capturedImage.EncodeToPNG();
        string imageFileName = "CapturedImage_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        StorageReference imageRef = storageRef.Child("images/" + imageFileName);

        var uploadTask = imageRef.PutBytesAsync(imageData);
        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if (uploadTask.Exception != null)
        {
            debug_Error.text = "Debug Error: �̹��� ���ε� �� ���� �߻�: " + uploadTask.Exception.ToString();
            yield break;
        }

        // 2. �̹��� ���ε尡 ���������� �Ϸ�Ǹ� �ٿ�ε� URL�� ������

        yield return imageRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                uploadedImageUrl = task.Result.ToString(); // �̹��� �ٿ�ε� URL ����
                imageUrlInputField.text = "�̹��� URL �ۼ��� �Ϸ�Ǿ����ϴ�."; // URL�� InputField�� ǥ��
                debug_Log.text = "�̹��� URL: " + uploadedImageUrl;
            }
            else
            {
                debug_Error.text = "Debug Error: �̹��� �ٿ�ε� URL�� �������µ� �����߽��ϴ�.";
            }
        });
    }

    // ������ ���� ��ư ���� �Լ�
    public void SaveDataToFirebase()
    {
        btSavePlay.PlayOneShot(btSave);
        // �̹��� URL�� ������ �������� ����
        if (string.IsNullOrEmpty(uploadedImageUrl))
        {
            debug_Error.text = "�̹����� ���ε���� �ʾҽ��ϴ�. ���� �̹����� ���ε��ϼ���.";
            return;
        }

        // GPSService�� Latitude, Longitude ���� InputField�� ����Ǿ� �����Ƿ� �̸� float�� ��ȯ
        float latitude = 0f;
        float longitude = 0f;

        // InputField���� ���ڿ��� �޾Ƽ� float���� ��ȯ
        if (float.TryParse(GPSService.Instance.Latitude.text, out latitude) &&
            float.TryParse(GPSService.Instance.Longitude.text, out longitude))
        {
            string url = urlInputField.text; // URL�� InputField�� �Է¹���
            string name = nameInputField.text; // ������ �̸�

            // Firebase Realtime Database�� ��ġ ���� �� �̹��� URL ����
            Info info = new Info(name, latitude, longitude, url, uploadedImageUrl); // ���ε�� �̹��� URL ���
            string json = JsonUtility.ToJson(info);
            string key = databaseRef.Child("locations").Push().Key;

            databaseRef.Child("locations").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    debug_Log.text = "�����Ͱ� ���������� ����Ǿ����ϴ�.";
                }
                else
                {
                    debug_Error.text = "Debug Error: ������ ���� �� ���� �߻�: " + task.Exception.ToString();
                }
            });
        }
        else
        {
            debug_Error.text = "Debug Error: GPS ���� float�� ��ȯ�ϴµ� �����߽��ϴ�.";
        }
    }
}