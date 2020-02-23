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

    float timeAnim = .5f;

    bool alreadyChanging = false;

    void Awake ()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("ArduinoHandlerScene", LoadSceneMode.Additive);
        }
        else
        {
            Destroy(this);
        }
    }

    public void RestartScene(float delay = 0, bool withFade = false)
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, delay, withFade));
    }

    public void ChangeScene(string sceneName, float delay = 0, bool withFade = false)
    {
        StartCoroutine(LoadScene(sceneName, delay, withFade));
    }

    public void QuitGame (float delay = 0)
    {
        Invoke("_QuitGame", delay);
    }
    
    void _QuitGame ()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 
    }

        AsyncOperation async;
    IEnumerator LoadScene(string sceneName, float delay, bool withFade = false)
    {
        if (!alreadyChanging)
        {
            alreadyChanging = true;

            if (withFade && delay != 0 && UiFade.Instance != null) UiFade.Instance.ChangeAlpha(1, delay);
            if (delay != 0) yield return new WaitForSecondsRealtime(delay);

            Canvas[] allCanvas = GameObject.FindObjectsOfType<Canvas>();
            for (int i = 0; i < allCanvas.Length; i++)
            {
                allCanvas[i].gameObject.SetActive(false);
            }

            Canvas canvasInstance = Instantiate(backgroundLoading);
            Slider loadingBarCreated = Instantiate(loadingBar, canvasInstance.transform);


            loadingBarCreated.gameObject.SetActive(true);
            async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            async.allowSceneActivation = false;

            yield return new WaitForSecondsRealtime(timeAnim);

            while (async.isDone == false)
            {
                loadingBarCreated.value = async.progress;
                if (async.progress == 0.9f)
                {
                    loadingBarCreated.value = 1f;
                    async.allowSceneActivation = true;
                    alreadyChanging = false;
                }
                yield return null;
            }
        }
    }

}
