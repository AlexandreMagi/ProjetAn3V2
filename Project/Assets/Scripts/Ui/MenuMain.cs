using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMain : MonoBehaviour
{

    private static MenuMain _instance;
    public static MenuMain Instance { get { return _instance; } }
    void Awake() { _instance = this; }

    [HideInInspector]
    public List<ButtonMenuScript> buttonMenuScripts;

    [HideInInspector]
    public enum menustate { intro, home, mainmenu }
    menustate currentState = menustate.intro;

    [SerializeField] private Vector2 scaleOver = new Vector2(1, 1.2f);
    [SerializeField] private float speedOver = 5;
    [SerializeField] private string sceneNameGoTo = "LD_03";

    bool canClickOnButton = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) SceneHandler.Instance.RestartScene();
        switch (currentState)
        {
            case menustate.intro:
                if (Input.GetKeyDown(KeyCode.Mouse0)) SkipToHome();
                break;
            case menustate.home:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    GetComponent<Animator>().SetTrigger("GoToMainMenu");
                    currentState = menustate.mainmenu;
                }
                break;
            case menustate.mainmenu:
                foreach (var button in buttonMenuScripts)
                {
                    if (button.gameObject.activeSelf)
                    {
                        bool mouseOver = button.CheckIfMouseOver(Input.mousePosition);
                        if (mouseOver)
                        {
                            button.transform.localScale = Vector3.Lerp(button.transform.localScale, Vector3.one * scaleOver.y, Time.unscaledDeltaTime * speedOver);
                        }
                        else
                        {
                            button.transform.localScale = Vector3.Lerp(button.transform.localScale, Vector3.one * scaleOver.x, Time.unscaledDeltaTime * speedOver);
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse0) && canClickOnButton) Click(Input.mousePosition);
                break;
            default:
                break;
        }

    }

    void Click(Vector2 mousePosition)
    {
        foreach (var button in buttonMenuScripts)
        {
            button.Click(mousePosition);
        }
    }

    public void setValueState (menustate value)
    {
        currentState = value;
    }

    public void GoToGame ()
    {
        canClickOnButton = false;
        SceneHandler.Instance.ChangeScene(sceneNameGoTo, 1,true);
    }

    public void QuitAppli ()
    {
        canClickOnButton = false;
        SceneHandler.Instance.QuitGame(0);
    }

    void SkipToHome()
    {
        GetComponent<Animator>().SetTrigger("SkipToHome");
        currentState = menustate.home;
    }
    /*
    public void IsOnMainMenu()
    {
        currentState = menustate.mainmenu;
    }

    public void IsOnHome()
    {
        currentState = menustate.home;
    }

    public void IsOnIntro()
    {
        currentState = menustate.intro;
    }*/

}
