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

        /* ���� ������ ����, ������Ʈ�� �������� ���� ��� ��� �޽����� ���
        if (List_Info == null)
            Debug.LogWarning("List_info ������Ʈ�� ã�� �� �����ϴ�.");

        if (Save_Info == null)
            Debug.LogWarning("Save_info ������Ʈ�� ã�� �� �����ϴ�.");
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
        Debug.Log("��������!");
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
