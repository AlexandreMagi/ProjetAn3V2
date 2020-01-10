using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private static SceneHandler _instance;
    public static SceneHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake ()
    {
        _instance = this;
    }

    public void Restart (float delay)
    {
        Invoke("RestartScene", delay);
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void ChangeScene (string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
