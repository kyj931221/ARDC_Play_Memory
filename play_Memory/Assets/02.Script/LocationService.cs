/** 
   ...
   1. Firebase에서 데이터 불러오기 : Firebase에 저장된 모든 데이터를 불러와 사용자의 현재 위치와 비교
   2. 위치 비교 : 사용자의 GPS 위치와 Fierbase에서 불러온 데이터의 위치를 비교하여, 일정 범위(반경)에
                 있는 데이터를 필터링한다.
   3. 스크롤뷰 형식으로 표시 : 필터링된 데이터를 스크롤뷰에 동적으로 추가하여 UI로 표시
   4. 표시된 UI를 클릭하면 다음 씬으로 전환
   ...
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.Networking; // 네트워크 요청을 위한 네임스페이스
using UnityEngine.SceneManagement;

public class LocationService : MonoBehaviour
{
    public Transform contentParent; //(List_Item) 스크롤뷰 내의 콘텐츠 패널 (데이터를 추가할 부모 오브젝트)
    public GameObject listItemPrefab; // 리스트 아이템 프리팹
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
                debug_Log.text = "GPS 정보를 가져오는데 실패하였습니다.";
                yield break;
            }
            else
            {
                // GPS 데이터 얻기
                userLatitude = Input.location.lastData.latitude;
                userLongitude = Input.location.lastData.longitude;

                debug_Log.text = "GPS 정보를 성공적으로 확인하였습니다.";
            }
        }
        else
        {
            debug_Log.text = "GPS가 비활성화되어 있습니다.";
            yield break;
        }

        // Firebase에서 데이터 불러오기
        reference.Child("locations").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                debug_Log.text = "Firebase 데이터 불러오기 성공. 총 데이터 수: " + snapshot.ChildrenCount;

                foreach (DataSnapshot locationSnapshot in snapshot.Children)
                {
                    // Firebase에서 JSON 데이터를 Info 객체로 변환
                    Info info = JsonUtility.FromJson<Info>(locationSnapshot.GetRawJsonValue());
                    Debug.Log("불러온 데이터: " + info.name + ", 위도: " + info.x + ", 경도: " + info.y);

                    if (IsWithinRadius(userLatitude, userLongitude, info.x, info.y, radiusKm))
                    {
                        AddToList(info);  // 데이터를 스크롤뷰에 추가
                    }
                }
            }
            else
            {
                debug_Log.text = "Firebase 데이터 로드 실패: " + task.Exception;
            }
        });
    }

    // 두 GPS 좌표가 반경 내에 있는지 확인하는 함수 (Haversine 공식 사용)
    private bool IsWithinRadius(float lat1, float lon1, float lat2, float lon2, float radiusKm)
    {
        float R = 6371; // 지구 반지름 (단위: km)
        float dLat = Mathf.Deg2Rad * (lat2 - lat1);
        float dLon = Mathf.Deg2Rad * (lon2 - lon1);
        float a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
                  Mathf.Cos(Mathf.Deg2Rad * lat1) * Mathf.Cos(Mathf.Deg2Rad * lat2) *
                  Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2);
        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        float distance = R * c; // 거리 계산 (단위: km)

        return distance <= radiusKm; // 주어진 반경 내인지 확인
    }

    private void AddToList(Info info)
    {
        // 리스트 아이템 프리팹을 복제하여 스크롤뷰의 콘텐츠에 추가
        GameObject listItem = Instantiate(listItemPrefab, contentParent);

        // 디버깅: 프리팹이 제대로 생성되었는지 확인
        if (listItem != null)
        {
            debug_Log.text = "리스트 아이템 생성 성공: "; //+ info.name;
        }
        else
        {
            debug_Log.text = "리스트 아이템 생성 실패.";
        }

        // 리스트 아이템에 데이터 적용
        listItem.transform.Find("NameText").GetComponent<Text>().text = info.name;
        listItem.transform.Find("LatitudeText").GetComponent<Text>().text = "Latitude: " + info.x.ToString();
        listItem.transform.Find("LongitudeText").GetComponent<Text>().text = "Longitude: " + info.y.ToString();
        listItem.transform.Find("UrlText").GetComponent<Text>().text = "VIDIO URL: 비디오가 연결되었습니다.";

        // 이미지 URL 텍스트 표스
        listItem.transform.Find("ImageUrlText").GetComponent<Text>().text = "Image URL: 이미지가 연결되었습니다.";

        // 이미지 URL 버튼 연결
        Button imageButton = listItem.transform.Find("imageButton").GetComponent<Button>();
        imageButton.onClick.AddListener(() => LoadImageScene(info.imageUrl));

        // YouTube URL 버튼에 URL 연결
        Button urlButton = listItem.transform.Find("UrlButton").GetComponent<Button>();
        urlButton.onClick.AddListener(() => Application.OpenURL(info.url));
    }

    private void LoadImageScene(string imageUrl)
    {
        // 로그를 통해 저장하려는 imageUrl을 확인
        //debug_Log.text = "저장하려는 이미지 URL: " + imageUrl;

        // PlayerPrefs에 이미지 URL 저장
        PlayerPrefs.SetString("ImageURL", imageUrl);
        PlayerPrefs.Save();  // 데이터를 즉시 저장하여 씬 전환 중 유실 방지

        // 씬 전환
        SceneManager.LoadScene("03.AR_Content_Scene");
    }
}

[System.Serializable]
public class Info
{
    public string name;  // 장소 이름
    public string url;   // 유튜브 URL
    public float x;      // 위도 (latitude)
    public float y;      // 경도 (longitude)
    public string imageUrl; // 이미지 URL
}
