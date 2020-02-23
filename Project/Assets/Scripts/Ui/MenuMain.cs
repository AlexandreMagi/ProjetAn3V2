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


    [SerializeField] private float timeBeforeGoBackToStart = 5;
    private float timerGoBack = 5;
    private Vector3 saveLastCursorPos = Vector3.zero;

    [SerializeField] DataWeapon dataWeapon = null;
    float currentChargePurcentage = 0;

    private bool playerCanShoot = true;

    bool canClickOnButton = true;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A)) isArduinoMode = !isArduinoMode;

        if (arduinoTransmettor == null)
        {
            arduinoTransmettor = IRCameraParser.Instance;
        }
        Vector3 posCursor = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
        if (Vector3.Distance(saveLastCursorPos, posCursor) > 0 || currentState != menustate.mainmenu) 
            timerGoBack = timeBeforeGoBackToStart;
        else
        {
            if (timerGoBack < Time.unscaledDeltaTime)
            {
                currentState = menustate.home;
                GetComponent<Animator>().SetTrigger("GoHome");
            }
            else
                timerGoBack -= Time.unscaledDeltaTime;
        }
        saveLastCursorPos = posCursor;


        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneHandler.Instance.RestartScene();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            HintScript.Instance.PopHint("Voila t'es content Max? T'as encore tout cassé?",5);
        }


        //UI
        if (UiCrossHair.Instance != null)
        {

            UiCrossHair.Instance.UpdateCrossHair(isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition);
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }

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
                    foreach (var button in buttonMenuScripts)
                    {
                        button.UpdatePos(false);
                    }
                }
                break;
            case menustate.mainmenu:
                foreach (var button in buttonMenuScripts)
                {
                    if (button.gameObject.activeSelf)
                    {
                        button.UpdatePos(true);
                        bool mouseOver = button.CheckIfMouseOver(posCursor);
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
