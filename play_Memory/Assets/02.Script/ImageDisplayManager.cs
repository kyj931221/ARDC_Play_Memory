using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ImageDisplayManager : MonoBehaviour
{
    public RawImage imageDisplay; // �̹����� ǥ���� UI RawImage

    public Text debug_Log;

    void Start()
    {
        // ���� ������ ������ �̹��� URL �ҷ�����
        string imageUrl = PlayerPrefs.GetString("ImageURL", "");

        // imageUrl�� ����� �ҷ��������� �α׷� Ȯ��
        debug_Log.text = "�̹��� URL �ҷ����� �Ϸ�";

        if (!string.IsNullOrEmpty(imageUrl))
        {
            // �̹��� URL�� ��ȿ�ϸ� �ش� �̹����� �ε�
            StartCoroutine(LoadImageFromUrl(imageUrl));
        }
        else
        {
            debug_Log.text = "�̹��� URL�� �߸��Ǿ��ų� �����Ǿ����ϴ�.";
        }
    }

    // URL�� ���� �̹��� �ε��ϴ� �ڷ�ƾ
    IEnumerator LoadImageFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            debug_Log.text = "�̹��� �ε� ����: " + request.error;
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            if (texture != null)
            {
                imageDisplay.texture = texture; // RawImage�� �̹��� ����
                imageDisplay.SetNativeSize(); // �̹����� ���� ũ��� ����
                debug_Log.text = "�̹��� �ε� ����";
            }
            else
            {
                debug_Log.text = "�̹����� �ε������� Texture�� null�Դϴ�.";
            }
        }
    }
}
