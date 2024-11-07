using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Extensions;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth; // �α��� / ȸ������ � ���
    private FirebaseAuth user; // ������ �Ϸ�� ���� ����

    public InputField email;
    public InputField password;
    public Text User_Info_DebugMsg;

    public AudioClip btClick;
    private AudioSource btClickPlay;

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        btClickPlay = GetComponent<AudioSource>();
    }

    public void Create()
    {
        btClickPlay.PlayOneShot(btClick);

        auth.CreateUserWithEmailAndPasswordAsync(email.text, password.text).ContinueWith(task =>
        { 
                if(task.IsCanceled)
                {
                    User_Info_DebugMsg.text = "ȸ������ ���.";
                    return;
                }
                if(task.IsFaulted)
                {
                // ȸ������ ���� ���� => �̸����� ������ /  ��й�ȣ�� �ʹ� ���� / �̹� ���Ե� �̸��� ��..
                    User_Info_DebugMsg.text ="ȸ������ ����";
                    return;
                }
                Firebase.Auth.AuthResult newUser = task.Result;
                User_Info_DebugMsg.text = "ȸ������ �Ϸ�";
        });
    }

    public void Login()
    {
        btClickPlay.PlayOneShot(btClick);

        // �α��� �Ϸ��ϸ� ���� ������ �Ѿ�� ���ؼ��� 
        // ���� ContinueWith -> ContinueWithOnMainThread ���� ���� ����Ͽ�
        // ���ν������ ������� ��� ��.

        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                User_Info_DebugMsg.text ="�α��� ���";
                return;
            }
            if (task.IsFaulted)
            {
                // ȸ������ ���� ����..
                // 1) �̸��� ������ ������
                // 2) ��й�ȣ�� �ʹ� ����
                // 3) �̹� ���Ե� �̸��� ��..

                User_Info_DebugMsg.text = "�α��� ����";
                return;
            }
            Firebase.Auth.AuthResult newUser = task.Result;
            User_Info_DebugMsg.text = "�α��� �Ϸ�";

            //�α��� ���� �� ���� ������ �̵�
            LoadNextScene();
        });
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("02.Main_Scene");
    }

    public void LogOut()
    {
        auth.SignOut();
        SceneManager.LoadScene("01.User_Info_Scene");
        Debug.Log("�α׾ƿ�");
    }
}
