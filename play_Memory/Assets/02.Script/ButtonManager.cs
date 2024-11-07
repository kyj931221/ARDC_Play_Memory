using System.Collections;
using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    private GameObject List_Info;
    private GameObject Save_Info;
    
    private AudioSource btClickPlay;
    public AudioClip btClick;

    private void Awake()
    {
        btClickPlay = GetComponent<AudioSource>();
    }

    private void Start()
    {
        List_Info = transform.Find("List_info")?.gameObject;
        Save_Info = transform.Find("Save_info")?.gameObject;

        /* 오류 방지를 위해, 오브젝트가 존재하지 않을 경우 경고 메시지를 출력
        if (List_Info == null)
            Debug.LogWarning("List_info 오브젝트를 찾을 수 없습니다.");

        if (Save_Info == null)
            Debug.LogWarning("Save_info 오브젝트를 찾을 수 없습니다.");
        */
    }

    public void Main_Load()
    {
        btClickPlay.PlayOneShot(btClick);
        SceneManager.LoadScene("02.Main_Scene");
    }

    public void Url_List_Load()
    {
        btClickPlay.PlayOneShot(btClick);
        SceneManager.LoadScene("04.Data_List_Scene");
    }

    public void Url_Save_Load()
    {
        btClickPlay.PlayOneShot(btClick);
        SceneManager.LoadScene("05.Data_Save_Scene");
    }

    public void AppQuit()
    {
        btClickPlay.PlayOneShot(btClick);
        Application.Quit();
        Debug.Log("게임종료!");
    }

    public void List_Info_Active()
    {
        btClickPlay.PlayOneShot(btClick);
        if (List_Info != null && Save_Info != null)
        {
            List_Info.SetActive(true);
            Save_Info.SetActive(false);
        }
    }

    public void Save_Info_Active()
    {
        btClickPlay.PlayOneShot(btClick);
        if (List_Info != null && Save_Info != null)
        {
            List_Info.SetActive(false);
            Save_Info.SetActive(true);
        }
    }
}
