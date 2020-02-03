using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField]
    Slider loadingBar = null;
    [SerializeField]
    Canvas backgroundLoading = null;

    void Awake ()
    {
        _instance = this;
    }

    public void RestartScene(float delay)
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, delay));
    }

    public void ChangeScene (string sceneName, float delay)
    {
        StartCoroutine(LoadScene(sceneName, delay));
    }

    AsyncOperation async;
    IEnumerator LoadScene(string sceneName, float delay)
    {
        Canvas[] allCanvas = GameObject.FindObjectsOfType<Canvas>();
        for (int i = 0; i < allCanvas.Length; i++)
        {
            allCanvas[i].gameObject.SetActive(false);
        }

        Canvas canvasInstance = Instantiate(backgroundLoading);
        Slider loadingBarCreated = Instantiate(loadingBar, canvasInstance.transform);

        yield return new WaitForSecondsRealtime(delay);

        loadingBarCreated.gameObject.SetActive(true);
        async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            loadingBarCreated.value = async.progress;
            if (async.progress == 0.9f)
            {
                loadingBarCreated.value = 1f;
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}
