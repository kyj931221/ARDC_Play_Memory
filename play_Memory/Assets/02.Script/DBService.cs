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
    private AudioSource btCapturePlay; // 이미지 저장 버튼
    private AudioSource btSavePlay; // 데이터 저장 버튼

    public AudioClip btCapture; // 이미지 저장 버튼 소리
    public AudioClip btSave; // 데이터 저장 버튼 소리

    public InputField urlInputField; // 유튜브 url
    public InputField nameInputField; // DB에 저장할 이름
    public InputField imageUrlInputField; // 이미지 URL이 입력될 필드

    public RawImage cameraPreview; // 카메라 프리뷰
    public Button uploadImageButton; // 이미지 촬영 및 업로드, URL 가져오는 버튼
    public Button saveDataButton; // 데이터 저장 버튼

    public Text debug_Log;
    public Text debug_Error;

    private WebCamTexture webCamTexture;
    private Texture2D capturedImage;

    // Firebase 관련
    private FirebaseStorage storage;
    private StorageReference storageRef;
    private DatabaseReference databaseRef;

    private string uploadedImageUrl = null; // 업로드된 이미지 URL을 저장할 변수

    public class Info
    {
        public float x;  // 위도
        public float y;  // 경도
        public string url; // 유튜브 영상 링크
        public string name; // 저장 DB 제목
        public string imageUrl; // Firebase에 저장된 이미지의 URL

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
        // 각 버튼에 대한 소리 컴포넌트 적용
        btCapturePlay = GetComponent<AudioSource>();
        btSavePlay = GetComponent<AudioSource>();

        // Firebase 초기화
        storage = FirebaseStorage.DefaultInstance;
        storageRef = storage.GetReferenceFromUrl("gs://catch-you-catch-me.appspot.com");
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;

        // 카메라 초기화
        webCamTexture = new WebCamTexture();
        cameraPreview.texture = webCamTexture;
        cameraPreview.material.mainTexture = webCamTexture;
        webCamTexture.Play();

        // Firebase Realtime Database 루트 레퍼런스
        //databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void UploadImageToFirebase()
    {
        btCapturePlay.PlayOneShot(btCapture);
        // 카메라에서 텍스처 데이터를 가져와 Texture2D로 변환
        capturedImage = new Texture2D(webCamTexture.width, webCamTexture.height);
        capturedImage.SetPixels(webCamTexture.GetPixels());
        capturedImage.Apply();

        // 이미지 업로드 시작
        StartCoroutine(UploadImageAndGetUrl());
    }

    // Firebase에 이미지 업로드 및 URL 가져오기
    IEnumerator UploadImageAndGetUrl()
    {
        // 1. Firebase Storage에 이미지 업로드

        byte[] imageData = capturedImage.EncodeToPNG();
        string imageFileName = "CapturedImage_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
        StorageReference imageRef = storageRef.Child("images/" + imageFileName);

        var uploadTask = imageRef.PutBytesAsync(imageData);
        yield return new WaitUntil(() => uploadTask.IsCompleted);

        if (uploadTask.Exception != null)
        {
            debug_Error.text = "Debug Error: 이미지 업로드 중 오류 발생: " + uploadTask.Exception.ToString();
            yield break;
        }

        // 2. 이미지 업로드가 성공적으로 완료되면 다운로드 URL을 가져옴

        yield return imageRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                uploadedImageUrl = task.Result.ToString(); // 이미지 다운로드 URL 저장
                imageUrlInputField.text = "이미지 URL 작성이 완료되었습니다."; // URL을 InputField에 표시
                debug_Log.text = "이미지 URL: " + uploadedImageUrl;
            }
            else
            {
                debug_Error.text = "Debug Error: 이미지 다운로드 URL을 가져오는데 실패했습니다.";
            }
        });
    }

    // 데이터 저장 버튼 연결 함수
    public void SaveDataToFirebase()
    {
        btSavePlay.PlayOneShot(btSave);
        // 이미지 URL이 없으면 저장하지 않음
        if (string.IsNullOrEmpty(uploadedImageUrl))
        {
            debug_Error.text = "이미지가 업로드되지 않았습니다. 먼저 이미지를 업로드하세요.";
            return;
        }

        // GPSService의 Latitude, Longitude 값이 InputField에 저장되어 있으므로 이를 float로 변환
        float latitude = 0f;
        float longitude = 0f;

        // InputField에서 문자열을 받아서 float으로 변환
        if (float.TryParse(GPSService.Instance.Latitude.text, out latitude) &&
            float.TryParse(GPSService.Instance.Longitude.text, out longitude))
        {
            string url = urlInputField.text; // URL을 InputField로 입력받음
            string name = nameInputField.text; // 데이터 이름

            // Firebase Realtime Database에 위치 정보 및 이미지 URL 저장
            Info info = new Info(name, latitude, longitude, url, uploadedImageUrl); // 업로드된 이미지 URL 사용
            string json = JsonUtility.ToJson(info);
            string key = databaseRef.Child("locations").Push().Key;

            databaseRef.Child("locations").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    debug_Log.text = "데이터가 성공적으로 저장되었습니다.";
                }
                else
                {
                    debug_Error.text = "Debug Error: 데이터 저장 중 오류 발생: " + task.Exception.ToString();
                }
            });
        }
        else
        {
            debug_Error.text = "Debug Error: GPS 값을 float로 변환하는데 실패했습니다.";
        }
    }
}