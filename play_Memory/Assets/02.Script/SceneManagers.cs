using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagers : MonoBehaviour
{
    public string mainScene;

    public void MainScne()
    {
        SceneManager.LoadScene(mainScene);
    }
}
