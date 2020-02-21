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

    [SerializeField] DataWeapon dataWeapon = null;
    float currentChargePurcentage = 0;

    private bool playerCanShoot = true;

    bool canClickOnButton = true;

    [SerializeField] GameObject rootLogo = null;
    [SerializeField] GameObject rootHome = null;
    [SerializeField] GameObject rootMainMenu = null;

    IRCameraParser arduinoTransmettor;
    bool isArduinoMode = true;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.A)) isArduinoMode = !isArduinoMode;

        if (arduinoTransmettor == null)
        {
            arduinoTransmettor = IRCameraParser.Instance;
        }
        Vector3 posCursor = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneHandler.Instance.RestartScene();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            currentState = menustate.home;
            GetComponent<Animator>().SetTrigger("GoHome");
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
                if (CheckIfShoot()) SkipToHome();
                break;
            case menustate.home:
                if (CheckIfShoot())
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
                if (CheckIfShoot() && canClickOnButton) Click(posCursor);
                break;
            default:
                break;
        }

    }


    public void GoBackToMenu()
    {
        rootLogo.SetActive(false);
        rootHome.SetActive(true);
        rootMainMenu.SetActive(false);
    }

    public bool CheckIfShoot()
    {
        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotHeld) : Input.GetKey(KeyCode.Mouse0)) && playerCanShoot)
        {
            InputHold();
        }
        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) && playerCanShoot)
        {
            InputUp();
            return true;
        }
        return false;
    }


    public float GetChargeValue()
    {
        return currentChargePurcentage;
    }

    void InputHold()
    {
        if (currentChargePurcentage < 1)
        {
            currentChargePurcentage += (dataWeapon.chargeSpeedIndependantFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / dataWeapon.chargeTime;
            if (currentChargePurcentage > 1)
            {
                UiCrossHair.Instance.JustFinishedCharging();
                currentChargePurcentage = 1;
            }
        }
    }

    void InputUp()
    {
        DataWeaponMod currentWeaponMod = null;
        if (currentChargePurcentage == 1) currentWeaponMod = dataWeapon.chargedShot;
        else currentWeaponMod = dataWeapon.baseShot;

        UiCrossHair.Instance.PlayerShot(currentWeaponMod.shootValueUiRecoil, currentChargePurcentage == 1);
        currentChargePurcentage = 0;
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
    public Vector3 GetCursorPos()
    {
        return isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
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
