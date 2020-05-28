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
    Canvas backgroundLoading = null;

    float timeAnim = .5f;

    bool alreadyChanging = false;

    void Awake ()
    {
        if (_instance == null)
        {
            _instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            SceneManager.LoadScene("ArduinoHandlerScene", LoadSceneMode.Additive);

//#if UNITY_EDITOR
//            Application.targetFrameRate = 300;
//#else
//            Application.targetFrameRate = 60;
//#endif 
        }
        else
        {
            Destroy(this);
        }
    }

    public void Start()
    {
        //PreLoadScene("LD_03");
    }

    public void RestartScene(float delay = 0, bool withFade = false)
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().name, delay, withFade));
    }

    public void ChangeScene(string sceneName, float delay = 0, bool withFade = false)
    {
        CheckIfMustLockFrame(sceneName);
        StartCoroutine(LoadScene(sceneName, delay, withFade));
    }

    void CheckIfMustLockFrame (string sName)
    {
        //if (sName == "MenuScene") Application.targetFrameRate = 60;
        //else Application.targetFrameRate = 300;
    }

    public void QuitGame (float delay = 0)
    {
        Invoke("_QuitGame", delay);
    }

    AsyncOperation asyncPreload;
    public void PreLoadScene(string sceneName)
    {
        if (!alreadyChanging)
        {
            Debug.Log("Load Scene : " + sceneName);
            alreadyChanging = true;
            asyncPreload = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            asyncPreload.allowSceneActivation = false;
            asyncPreload.priority = 0;
        }
    }

    public void AllowChangeToPreloadScene()
    {
        StartCoroutine(ChargePreloadedScene());
        //if (asyncPreload != null)
        //{
        //    asyncPreload.allowSceneActivation = true;
        //    alreadyChanging = false;
        //}

    }

    //private void Update()
    //{
    //    //if (asyncPreload == null && SceneManager.GetActiveScene().name == "MenuScene") PreLoadScene("LD_03");
    //    if (Input.GetKeyDown(KeyCode.V)) PreLoadScene("LD_03");
    //    //if (Input.GetKeyDown(KeyCode.B)) AllowChangeToPreloadScene();
    //    if (Input.GetKeyDown(KeyCode.B)) StartCoroutine(ChargePreloadedScene());
    //}

    void _QuitGame ()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif 
    }

    IEnumerator ChargePreloadedScene()
    {
        if (asyncPreload != null)
        {
            float delay = 0.5f;
            if (delay != 0 && UiFade.Instance != null) UiFade.Instance.ChangeAlpha(1, delay);
            if (delay != 0) yield return new WaitForSecondsRealtime(delay);

            asyncPreload.allowSceneActivation = true;
            alreadyChanging = false;
        }
        yield break;
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
            Image loadingBarCreated = canvasInstance.GetComponent<LoadingScreenAccesseur>().loadingBar;


            loadingBarCreated.gameObject.SetActive(true);
            async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            async.allowSceneActivation = false;

            yield return new WaitForSecondsRealtime(timeAnim);

            while (async.isDone == false)
            {
                loadingBarCreated.fillAmount = async.progress;
                if (async.progress == 0.9f)
                {
                    loadingBarCreated.fillAmount = 1f;
                    async.allowSceneActivation = true;
                    alreadyChanging = false;
                }
                yield return null;
            }
        }
        yield break;
    }

}
