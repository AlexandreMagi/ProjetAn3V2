using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuMain : MonoBehaviour
{

    private static MenuMain _instance;
    public static MenuMain Instance { get { return _instance; } }
    void Awake() { _instance = this; }

    [HideInInspector]
    public List<ButtonMenuScript> buttonMenuScripts;

    [HideInInspector]
    public enum menustate { idle, intro, home, mainmenu }
    menustate currentState = menustate.idle;

    [SerializeField] private Vector2 scaleOver = new Vector2(1, 1.2f);
    [SerializeField] private float speedOver = 5;
    [SerializeField] private string sceneNameGoTo = "LD_03";
    [SerializeField] private float checkInputEvery = 0.5f;
    [SerializeField] private float distanceCheckIfInput = 0.05f;
    float timerCheckInput = 0;
    [SerializeField] private float timeBeforeGoBackToStart = 5;
    private float timerGoBack = 5;
    private Vector3 saveLastCursorPos = Vector3.zero;

    [SerializeField] DataWeapon dataWeapon = null;
    float currentChargePurcentage = 0;

    private bool playerCanShoot = true;

    bool canClickOnButton = true;

    [SerializeField] GameObject rootLogo = null;
    [SerializeField] GameObject rootHome = null;
    [SerializeField] GameObject rootMainMenu = null;

    IRCameraParser arduinoTransmettor;
    [SerializeField] bool isArduinoMode = true;

    [Header ("Bullet Holes")]
    // --- BUllet Holes
    [SerializeField] Transform rootBulletHole = null;
    [SerializeField] GameObject emptyUISquare = null;
    [SerializeField] Sprite spriteBulletHole = null;
    List<BulletHoleInstance> allBulletHole = new List<BulletHoleInstance>();
    [SerializeField] int basePullBulleltHole = 15;
    [SerializeField] float timeBulletHoleRemain = 3;
    [SerializeField] float timeBulletHoleFade = 1;
    [SerializeField] float bulletSize = 50;
    //[SerializeField] int shotGunNbBullet = 12;
    [SerializeField] float shotGunAddedSpread = 1200;


    [SerializeField] GameObject idleBornVideo = null;
    [SerializeField] float TimeBeforeGoRestart = 10;
    float timeRemainingBeforeRestart = 0;

    float timeRemainingBeforeChargeScene = 1;

    [SerializeField] HighQualityButton qualityButton = null;

    [HideInInspector] bool InLeaderboardScreen = false;
    [SerializeField] GameObject leaderboardScreen = null;
    [SerializeField] CanvasGroup leaderboardScreenCanvasGroup = null;
    [SerializeField] CanvasGroup leaderboardButtonGoToCanvasGroup = null;
    [SerializeField] CanvasGroup leaderboardButtonGoBackCanvasGroup = null;
    [SerializeField] LeaderboardAndCredits leaderboardAndCreditsHandler = null;
    bool neverBeenInLeaderboard = true;
    bool inLeaderboardTransition = false;

    [SerializeField] LeaderboardCreditsButton leaderboardAndCreditMenuButton = null;
    private void Start()
    {
        Time.timeScale = 1;
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Drone_Ambiant", true, 0.4f);
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Crowd_Idle", true, 0.2f);
        //CustomSoundManager.Instance.PlaySound("Drone_Ambiant", "MainMenu", null, .4f, true);
        MusicHandler.Instance.PlayMusic(0,MusicHandler.Musics.menu, 0, 0, 0, 1, .3f, true, true);
        CustomSoundManager.Instance.PlaySound("Crowd_Idle", "MainMenu", null, .2f, true);

        idleBornVideo.SetActive(true);
        for (int i = 0; i < basePullBulleltHole; i++)
        {
            BulletHoleInstance instance = CreateBulletHoleDecal();
            instance.go.SetActive(false);
            allBulletHole.Add(instance);
        }
        Invoke("UpdateArduino", 1);
    }

    public void UpdateArduino()
    {
        if (ARdunioConnect.Instance != null) isArduinoMode = ARdunioConnect.Instance.ArduinoIsConnected;
    }


    // Update is called once per frame
    void Update()
    {

        if (isArduinoMode && Input.GetKeyDown(KeyCode.Mouse0)) isArduinoMode = false;
        if (!isArduinoMode && (arduinoTransmettor && arduinoTransmettor.isShotUp)) isArduinoMode = true;

        if (Input.GetKeyDown(KeyCode.A)) isArduinoMode = !isArduinoMode;

        if (arduinoTransmettor == null)
        {
            arduinoTransmettor = IRCameraParser.Instance;
        }
        //Vector3 posCursor = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;

        CheckIfGoBacToMenu();

        /*if (timeRemainingBeforeChargeScene > 0 && Time.unscaledDeltaTime < 0.5f)
        {
            timeRemainingBeforeChargeScene -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeChargeScene < 0) { SceneHandler.Instance.PreLoadScene(sceneNameGoTo); }
        }    */

        if (Input.GetKeyDown(KeyCode.J) && !inLeaderboardTransition)
        {
            ButtonLeaderboardCredits();
        }


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
        HandleBulletInstances();
        switch (currentState)
        {
            case menustate.idle:
                if (CheckIfShoot())
                {
                    idleBornVideo.SetActive(false);
                    currentState = menustate.intro;
                    GetComponent<Animator>().SetTrigger("ReplayIntro");
                }
                //CustomSoundManager.Instance.Mute();
                break;
            case menustate.intro:
                //CustomSoundManager.Instance.UnMute();
                if (CheckIfShoot()) SkipToHome();
                break;
            case menustate.home:
                if (timeRemainingBeforeRestart > 0)
                {
                    timeRemainingBeforeRestart -= Time.unscaledDeltaTime;
                    if (timeRemainingBeforeRestart < 0)
                    {
                        SceneHandler.Instance.RestartScene();
                    }
                }
                if (CheckIfShoot())
                {
                    GetComponent<Animator>().SetTrigger("GoToMainMenu");
                    currentState = menustate.mainmenu;
                    //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_FirstValidate", false, 1);
                    CustomSoundManager.Instance.PlaySound("SE_FirstValidate", "MainMenu", 1);
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
                        bool mouseOver = button.CheckIfMouseOver(GetCursorPos());
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
                if (CheckIfShoot() && canClickOnButton) Click(GetCursorPos());
                break;
            default:
                break;
        }

    }

    public void ButtonLeaderboardCredits()
    {
        if (neverBeenInLeaderboard) leaderboardAndCreditsHandler.InitTab();
        neverBeenInLeaderboard = false;
        InLeaderboardScreen = !InLeaderboardScreen;
        //leaderboardScreen.SetActive(InLeaderboardScreen);
        StartCoroutine(leaderboardScreenAnim(InLeaderboardScreen, .3f));
        if (InLeaderboardScreen) leaderboardAndCreditsHandler.InitGraph();

    }

    IEnumerator leaderboardScreenAnim (bool pop, float timeTransition)
    {
        inLeaderboardTransition = true;
        if (pop) leaderboardScreen.SetActive(true);
        float completion = 0;
        if (timeTransition > 0)
        {
            while (completion < 1)
            {
                completion += Time.unscaledDeltaTime / timeTransition;
                completion = Mathf.Clamp01(completion);
                leaderboardScreenCanvasGroup.alpha = pop ? completion : (1 - completion);
                leaderboardButtonGoToCanvasGroup.alpha = pop ? (1 - completion) : completion;
                leaderboardButtonGoBackCanvasGroup.alpha = pop ? completion : (1 - completion);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            leaderboardScreenCanvasGroup.alpha = pop ? 1 : 0;
            leaderboardButtonGoToCanvasGroup.alpha = pop ? 0 : 1;
            leaderboardButtonGoBackCanvasGroup.alpha = pop ? 1 : 0;
        }
        if (!pop) leaderboardScreen.SetActive(false);
        inLeaderboardTransition = false;
        yield break;
    }

    public void GoBackToMenu()
    {
        rootLogo.SetActive(false);
        rootHome.SetActive(true);
        rootMainMenu.SetActive(false);
    }

    void InstanceBulletHoleDecal (Vector2 cursorPos, float randomPosAdded = 0)
    {
        BulletHoleInstance currBulletHole = null;
        for (int i = 0; i < allBulletHole.Count; i++)
        {
            if (allBulletHole[i].lifeTimeRemaining == 0)
            {
                currBulletHole = allBulletHole[i];
                currBulletHole.lifeTimeRemaining = timeBulletHoleRemain;
                currBulletHole.lifeTimeDesapearing = timeBulletHoleFade;
                currBulletHole.go.SetActive(true);
                break;
            }
        }
        if (currBulletHole == null)
            currBulletHole = CreateBulletHoleDecal();

        if (randomPosAdded != 0) cursorPos += new Vector2 (Random.Range(0f, randomPosAdded) * Mathf.Sign(Random.Range(-1f, 1f)), Random.Range(0f, randomPosAdded) * Mathf.Sign(Random.Range(-1f, 1f)));

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, cursorPos, this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
        currBulletHole.go.transform.position = transform.TransformPoint(pos);
        currBulletHole.go.transform.Rotate(Vector3.forward * Random.Range(0, 360));
    }

    BulletHoleInstance CreateBulletHoleDecal()
    {
        GameObject currGo = Instantiate(emptyUISquare, rootBulletHole);
        currGo.GetComponent<Image>().sprite = spriteBulletHole;
        BulletHoleInstance currInstance = new BulletHoleInstance(currGo.GetComponent<RectTransform>(), currGo.GetComponent<Image>(), bulletSize);
        allBulletHole.Add(currInstance);
        return currInstance;
    }

    void HandleBulletInstances()
    {
        foreach (var bulletHole in allBulletHole)
        {
            if (bulletHole.lifeTimeRemaining > 0)
            {
                bulletHole.lifeTimeRemaining -= Time.unscaledDeltaTime;
                if (bulletHole.lifeTimeRemaining < 0)
                {
                    bulletHole.lifeTimeRemaining = 0;
                    bulletHole.go.SetActive(false);
                }

                if (bulletHole.lifeTimeRemaining < bulletHole.lifeTimeDesapearing) bulletHole.img.color = new Color(1, 1, 1, bulletHole.lifeTimeRemaining / bulletHole.lifeTimeDesapearing);
                else bulletHole.img.color = Color.white;
            }
        }
    }

    void CheckIfGoBacToMenu()
    {
        Vector3 posCursor = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;

        if (CheckIfShoot() && currentState == menustate.mainmenu) timerGoBack = timeBeforeGoBackToStart;

        timerCheckInput -= Time.unscaledDeltaTime;
        if (timerCheckInput < 0)
        {
            timerCheckInput += checkInputEvery;
            float currDist = Mathf.Sqrt(
                Mathf.Pow(Mathf.Abs(saveLastCursorPos.x - posCursor.x) / Screen.width, 2) +
                Mathf.Pow(Mathf.Abs(saveLastCursorPos.y - posCursor.y) / Screen.height, 2));

            if (currDist > distanceCheckIfInput || currentState != menustate.mainmenu)
                timerGoBack = timeBeforeGoBackToStart;

            else
            {
                if (timerGoBack < checkInputEvery)
                {
                    if (InLeaderboardScreen) ButtonLeaderboardCredits();
                    currentState = menustate.home;
                    timeRemainingBeforeRestart = TimeBeforeGoRestart;
                    GetComponent<Animator>().SetTrigger("GoHome");
                }
                else
                    timerGoBack -= checkInputEvery;
            }
            saveLastCursorPos = posCursor;
        }
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
                //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Charged_Shotgun", false, 1f, 0.1f);
                CustomSoundManager.Instance.PlaySound("Charged_Shotgun", "MainMenu", 1);
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

        for (int i = 0; i < currentWeaponMod.bulletPerShoot; i++)
        {
            if (i == 0)
                InstanceBulletHoleDecal(GetCursorPos(), 0);
            else
                InstanceBulletHoleDecal(GetCursorPos(), currentWeaponMod.bulletImprecision * shotGunAddedSpread);
        }

        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, currentChargePurcentage == 1 ? "ShotgunShot_Better_wav" : "Sound_Shot", false, currentChargePurcentage == 1 ? 0.8f : 0.4f, 0.2f);
        CustomSoundManager.Instance.PlaySound(currentChargePurcentage == 1 ? "ShotgunShot_Better_wav" : "Sound_Shot", "MainMenu", null, currentChargePurcentage == 1 ? 0.8f : 0.4f,false,1,0.2f);
        
        UiCrossHair.Instance.PlayerShot(currentWeaponMod.shootValueUiRecoil, currentChargePurcentage == 1);
        currentChargePurcentage = 0;

    }

    void Click(Vector2 mousePosition)
    {
        bool leaderboardButtonClicked = false;
        if (!inLeaderboardTransition) leaderboardButtonClicked = leaderboardAndCreditMenuButton.Click();
        if (!InLeaderboardScreen && !leaderboardButtonClicked)
        {
            foreach (var button in buttonMenuScripts)
            {
                button.Click(mousePosition);
            }
            if (qualityButton != null)
            {
                qualityButton.Click();
            }
        }
    }

    public void setValueState (menustate value)
    {
        if (currentState != menustate.idle)
            currentState = value;
        if (value == menustate.home)
            timeRemainingBeforeRestart = TimeBeforeGoRestart;

    }

    public void GoToGame ()
    {
        if (qualityButton != null)
        {
            qualityButton.GoToGame();
        }
        canClickOnButton = false;
        //SceneHandler.Instance.AllowChangeToPreloadScene();
        SceneHandler.Instance.ChangeScene(sceneNameGoTo, 1,true);
    }

    public void QuitAppli ()
    {
        canClickOnButton = false;
        SceneHandler.Instance.QuitGame(0);
    }
    public Vector3 GetCursorPos()
    {
        Vector2 returnedValue = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
        returnedValue = new Vector2(Mathf.Clamp(returnedValue.x, 0, Screen.width), Mathf.Clamp(returnedValue.y, 0, Screen.height));
        return returnedValue;
    }

    void SkipToHome()
    {
        GetComponent<Animator>().SetTrigger("SkipToHome");
        currentState = menustate.home;
        timeRemainingBeforeRestart = TimeBeforeGoRestart;
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

public class BulletHoleInstance
{
    public float lifeTimeRemaining = 0;
    public float lifeTimeDesapearing = 0;
    public RectTransform rect = null;
    public GameObject go = null;
    public Image img = null;

    public BulletHoleInstance(RectTransform _rect, Image _img, float bulletSize)
    {
        rect = _rect;
        go = rect.gameObject;
        img = _img;

        rect.sizeDelta = bulletSize * Vector2.one;
    }

}
