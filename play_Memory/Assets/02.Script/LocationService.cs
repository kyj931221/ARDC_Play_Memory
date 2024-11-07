/** 
   ...
   1. Firebase���� ������ �ҷ����� : Firebase�� ����� ��� �����͸� �ҷ��� ������� ���� ��ġ�� ��
   2. ��ġ �� : ������� GPS ��ġ�� Fierbase���� �ҷ��� �������� ��ġ�� ���Ͽ�, ���� ����(�ݰ�)��
                 �ִ� �����͸� ���͸��Ѵ�.
   3. ��ũ�Ѻ� �������� ǥ�� : ���͸��� �����͸� ��ũ�Ѻ信 �������� �߰��Ͽ� UI�� ǥ��
   4. ǥ�õ� UI�� Ŭ���ϸ� ���� ������ ��ȯ
   ...
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Networking; // ��Ʈ��ũ ��û�� ���� ���ӽ����̽�
using UnityEngine.SceneManagement;

public class LocationService : MonoBehaviour
{
    public Transform contentParent; //(List_Item) ��ũ�Ѻ� ���� ������ �г� (�����͸� �߰��� �θ� ������Ʈ)
    public GameObject listItemPrefab; // ����Ʈ ������ ������
    public float radiusKm = 0.05f;

    public Text debug_Log;

    DatabaseReference reference;

    private void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        StartCoroutine(NearbyLocationRoad());
    }

    IEnumerator NearbyLocationRoad()
    {
        float userLatitude = 0f;
        float userLongitude = 0f;

        if(Input.location.isEnabledByUser)
        {
            Input.location.Start();
            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if(Input.location.status == LocationServiceStatus.Failed)
            {
                debug_Log.text = "GPS ������ �������µ� �����Ͽ����ϴ�.";
                yield break;
            }
            else
            {
                // GPS ������ ���
                userLatitude = Input.location.lastData.latitude;
                userLongitude = Input.location.lastData.longitude;

                debug_Log.text = "GPS ������ ���������� Ȯ���Ͽ����ϴ�.";
            }
        }
        else
        {
            debug_Log.text = "GPS�� ��Ȱ��ȭ�Ǿ� �ֽ��ϴ�.";
            yield break;
        }

        // Firebase���� ������ �ҷ�����
        reference.Child("locations").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                debug_Log.text = "Firebase ������ �ҷ����� ����. �� ������ ��: " + snapshot.ChildrenCount;

                foreach (DataSnapshot locationSnapshot in snapshot.Children)
                {
                    // Firebase���� JSON �����͸� Info ��ü�� ��ȯ
                    Info info = JsonUtility.FromJson<Info>(locationSnapshot.GetRawJsonValue());
                    Debug.Log("�ҷ��� ������: " + info.name + ", ����: " + info.x + ", �浵: " + info.y);

                    if (IsWithinRadius(userLatitude, userLongitude, info.x, info.y, radiusKm))
                    {
                        AddToList(info);  // �����͸� ��ũ�Ѻ信 �߰�
                    }
                }
            }
            else
            {
                debug_Log.text = "Firebase ������ �ε� ����: " + task.Exception;
            }
        });
    }

    // �� GPS ��ǥ�� �ݰ� ���� �ִ��� Ȯ���ϴ� �Լ� (Haversine ���� ���)
    private bool IsWithinRadius(float lat1, float lon1, float lat2, float lon2, float radiusKm)
    {
        float R = 6371; // ���� ������ (����: km)
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float distance = R * c; // �Ÿ� ��� (����: km)

        return distance <= radiusKm; // �־��� �ݰ� ������ Ȯ��
    }

    private void AddToList(Info info)
    {
        // ����Ʈ ������ �������� �����Ͽ� ��ũ�Ѻ��� �������� �߰�
        GameObject listItem = Instantiate(listItemPrefab, contentParent);

        // �����: �������� ����� �����Ǿ����� Ȯ��
        if (listItem != null)
        {
            debug_Log.text = "����Ʈ ������ ���� ����: "; //+ info.name;
        }
        else
        {
            debug_Log.text = "����Ʈ ������ ���� ����.";
        }

        // ����Ʈ �����ۿ� ������ ����
        listItem.transform.Find("NameText").GetComponent<Text>().text = info.name;
        listItem.transform.Find("LatitudeText").GetComponent<Text>().text = "Latitude: " + info.x.ToString();
        listItem.transform.Find("LongitudeText").GetComponent<Text>().text = "Longitude: " + info.y.ToString();
        listItem.transform.Find("UrlText").GetComponent<Text>().text = "VIDIO URL: ������ ����Ǿ����ϴ�.";

        // �̹��� URL �ؽ�Ʈ ǥ��
        listItem.transform.Find("ImageUrlText").GetComponent<Text>().text = "Image URL: �̹����� ����Ǿ����ϴ�.";

        // �̹��� URL ��ư ����
        Button imageButton = listItem.transform.Find("imageButton").GetComponent<Button>();
        imageButton.onClick.AddListener(() => LoadImageScene(info.imageUrl));

        // YouTube URL ��ư�� URL ����
        Button urlButton = listItem.transform.Find("UrlButton").GetComponent<Button>();
        urlButton.onClick.AddListener(() => Application.OpenURL(info.url));
    }

    private void LoadImageScene(string imageUrl)
    {
        // �α׸� ���� �����Ϸ��� imageUrl�� Ȯ��
        //debug_Log.text = "�����Ϸ��� �̹��� URL: " + imageUrl;

        // PlayerPrefs�� �̹��� URL ����
        PlayerPrefs.SetString("ImageURL", imageUrl);
        PlayerPrefs.Save();  // �����͸� ��� �����Ͽ� �� ��ȯ �� ���� ����

        // �� ��ȯ
        SceneManager.LoadScene("03.AR_Content_Scene");
    }
}

[System.Serializable]
public class Info
{
    public string name;  // ��� �̸�
    public string url;   // ��Ʃ�� URL
    public float x;      // ���� (latitude)
    public float y;      // �浵 (longitude)
    public string imageUrl; // �̹��� URL
}
