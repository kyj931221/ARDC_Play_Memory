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
    private FirebaseAuth auth; // 로그인 / 회원가입 등에 사용
    private FirebaseAuth user; // 인증이 완료된 유저 정보

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
                    User_Info_DebugMsg.text = "회원가입 취소.";
                    return;
                }
                if(task.IsFaulted)
                {
                // 회원가입 실패 이유 => 이메일이 비정상 /  비밀번호가 너무 간단 / 이미 가입된 이메일 등..
                    User_Info_DebugMsg.text ="회원가입 실패";
                    return;
                }
                Firebase.Auth.AuthResult newUser = task.Result;
                User_Info_DebugMsg.text = "회원가입 완료";
        });
    }

    public void Login()
    {
        btClickPlay.PlayOneShot(btClick);

        // 로그인 완료하면 다음 씬으로 넘어가기 위해서는 
        // 기존 ContinueWith -> ContinueWithOnMainThread 으로 고쳐 사용하여
        // 메인스레드로 실행시켜 줘야 함.

        auth.SignInWithEmailAndPasswordAsync(email.text, password.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                User_Info_DebugMsg.text ="로그인 취소";
                return;
            }
            if (task.IsFaulted)
            {
                // 회원가입 실패 이유..
                // 1) 이메일 형식이 비정상
                // 2) 비밀번호가 너무 간단
                // 3) 이미 가입된 이메일 등..

                User_Info_DebugMsg.text = "로그인 실패";
                return;
            }
            Firebase.Auth.AuthResult newUser = task.Result;
            User_Info_DebugMsg.text = "로그인 완료";

            //로그인 성공 시 다음 씬으로 이동
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
        Debug.Log("로그아웃");
    }
}
