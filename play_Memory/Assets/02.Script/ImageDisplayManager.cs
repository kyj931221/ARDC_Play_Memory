using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ImageDisplayManager : MonoBehaviour
{
    public RawImage imageDisplay; // 이미지를 표시할 UI RawImage

    public Text debug_Log;

    void Start()
    {
        // 이전 씬에서 저장한 이미지 URL 불러오기
        string imageUrl = PlayerPrefs.GetString("ImageURL", "");

        // imageUrl이 제대로 불러와졌는지 로그로 확인
        debug_Log.text = "이미지 URL 불러오기 완료";

        if (!string.IsNullOrEmpty(imageUrl))
        {
            // 이미지 URL이 유효하면 해당 이미지를 로드
            StartCoroutine(LoadImageFromUrl(imageUrl));
        }
        else
        {
            debug_Log.text = "이미지 URL이 잘못되었거나 누락되었습니다.";
        }
    }

    // URL을 통해 이미지 로드하는 코루틴
    IEnumerator LoadImageFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            debug_Log.text = "이미지 로드 실패: " + request.error;
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            if (texture != null)
            {
                imageDisplay.texture = texture; // RawImage에 이미지 적용
                imageDisplay.SetNativeSize(); // 이미지의 원본 크기로 설정
                debug_Log.text = "이미지 로드 성공";
            }
            else
            {
                debug_Log.text = "이미지를 로드했으나 Texture가 null입니다.";
            }
        }
    }
}
