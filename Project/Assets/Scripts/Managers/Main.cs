using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public bool TCActivated = true;

    [SerializeField] bool startInWaitScreen = false;

    public bool playerCanOrb = true;
    public bool playerCanReload = true;
    public bool playerCanZeroG = true;
    public bool playerCanPerfectReload = true;
    public bool playerCanShoot = true;
    public bool playerCanShotgun = true;

    // C'est la faute de max ces variable
    bool playerCanOrbWaitScreenSave = false;
    bool playerCanReloadWaitScreenSave = false;
    bool playerCanZeroGWaitScreenSave = false;
    bool playerCanPerfectReloadWaitScreenSave = false;
    bool playerCanShootWaitScreenSave = false;
    bool playerCanShotgunWaitScreenSave = false;

    bool playerCouldShoot = false;
    bool playerCouldShotgun = false;
    bool playerCouldReload = false;
    bool playerCouldPerfectReload = false;
    bool playerCouldOrb = false;
    bool playerCouldZeroG = false;

    bool inWaitScreen = true;

    [HideInInspector] public bool overrideUiCrosshairInterdictionGraph = false;

    private bool playerUsedToHaveOrb = false;

    [HideInInspector] public bool playerInLeaderboard = false;

    private string sequenceCheat = "";
    private bool sequenceSkipMode = false;

    private float timeLeftForRaycastCursor;
    private float timeTickCursor = .2f;

    IRCameraParser arduinoTransmettor;

    [SerializeField]
    int startWithCameraNumber = 0;

    [SerializeField]
    DataDifficulty difficultyData = null;

    public bool playerResedAlready = false;

    [SerializeField]
    bool autoReloadOnNoAmmo = false;

    bool hasJumpedCam = false;

    [HideInInspector]
    public bool GameEnded = false;
    public bool lockSceneChange = false;

    [SerializeField]
    bool isArduinoMode = false;
    public bool IsArduinoMod { get { return isArduinoMode; } }

    [SerializeField]
    BoxCollider[] aiWalls = null;

    [SerializeField]
    AudioMixer mainMixer = null;


    AudioSource hSoundHandlerMainMusic = null;

    bool saveIfPlayerCouldReload = true;


    // --- Variables choix final
    bool lastChoiceForPlayer = false;
    [SerializeField] float timeBeforeChoice = 2;
    float timeRemainingBeforeChoice = 0;
    [SerializeField] float timeAfterChoice = 1;
    [SerializeField] float timerBeforeGameOver = 10;
    float timeRemainingBeforeGameOver = 10;
    int lastTimeRemainingBeforeGameOver = 10;
    lastChanceButton buttonMouseOver = null;
    float buttonMouseOverLerpSpeed = 8;
    float buttonMouseOverScale = 1.2f;
    float buttonNotMouseOverScale = 1f;
    float buttonOtherMouseOverScale = 0.8f;
    float timeBeforeChoiceDone = 0;
    int choiceMade = -1;
    [SerializeField] float timeBeforeChoiceSecurity = 2;
    float timeRemainingBeforeChoiceSecurity = 2;

    public float TimeRemainingBeforeGameOver { get { return timeRemainingBeforeGameOver; } }

    [HideInInspector] public List<string> TitlesUnlocked = new List<string>();


    [SerializeField] GameObject[] objectToChangeInLowQuality = null;


    [HideInInspector] public bool InDiorama = false;
    [HideInInspector] public bool GamePaused = false;
    bool wasInWaitScreen = false;

    [SerializeField] Color fogDefaultColor = new Color(108, 130, 137);

    bool enableDebugInputs = false;


    [Header("Skip Parameters")]
    [SerializeField] float timeMustStayHoldToSkip = 1;
    [SerializeField] float timeSkipButtonStayVisible = 2;
    float timeSkipButtonVisible = 0;
    bool inSkip = false;
    bool releasedInput = false;

    [SerializeField] float timeGoToLeaderboardAtGameOver = 5;
    float timeGoToLeaderboard = 0;


    [Header("Music Parameters")]
    [SerializeField] string musicLifeAndDeathChoice = "Music_TestChoixVieMort";
    [SerializeField] float musicLifeAndDeathChoiceVolume = 1;
    AudioSource lifeAndDeathAudioSource = null;

    public bool EnableComments = true;
    public float CommentAVolume = 2;
    public float CommentBVolume = 2;

    public static Main Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //Debug.Log("Remettre la musique");
        //hSoundHandlerMainMusic = CustomSoundManager.Instance.PlaySound("Drone_Ambiant", "Music",null, 0.5f, true);

        playerCouldShoot = playerCanShoot;
        playerCouldShotgun = playerCanShotgun;
        playerCouldReload = playerCanReload;
        playerCouldPerfectReload = playerCanPerfectReload;
        playerCouldOrb = playerCanOrb;
        playerCouldZeroG = playerCanZeroG;

        if (QualityHandler.Instance != null && !QualityHandler.Instance.isHighQuality) ChangeQuality(false);

        Invoke("UpdateArduino", 1);
        Invoke("UpdateWaitScreenStart", .5f);
        mainMixer.SetFloat("PitchAffectedVolume", 0);

    }

    public Color FogDefaultColor { get { return fogDefaultColor; } }

    public void UpdateArduino()
    {
        if (ARdunioConnect.Instance != null) isArduinoMode = ARdunioConnect.Instance.ArduinoIsConnected;
    }
    public void UpdateWaitScreenStart()
    {
        if (startInWaitScreen) SetupWaitScreenOn();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UiScoreBonusDisplay.Instance.MaybePlayCheer();
        }


        if (arduinoTransmettor == null)
        {
            arduinoTransmettor = IRCameraParser.Instance;
        }

        if (!hasJumpedCam && startWithCameraNumber != 0)
        {
            if (MusicHandler.Instance != null)
            {
                MusicHandler.Instance.PlayMusic(0, MusicHandler.Musics.none, 0, 1, 0, 0, .3f, true, false);
                //MusicHandler.Instance.PlayMusic(0,MusicHandler.Musics.introPreLastStage, 0, 1, 0, 0, .3f, true, false);
                //MusicHandler.Instance.PlayMusic(0,MusicHandler.Musics.preLastStage, 0, 0, 0, 0, .3f, false, true);
            }
            hasJumpedCam = true;
            SetupWaitScreenOff();
            UiCrossHair.Instance.StopWaitFunction();
            playerCanOrb = true;
            playerCanReload = true;
            playerCanZeroG = true;
            playerCanPerfectReload = true;
            playerCanShoot = true;
            playerCanShotgun = true;
            UiCrossHair.Instance.UpdateCursorUnlocks();
            Player.Instance.GainArmorOverTime(0, 100, 100);
            Invoke("SkipToSequence", .2f);
        }

        ////SHOOT
        //if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isGravityDown) : Input.GetKeyDown(KeyCode.Mouse1)))
        //{
        //    if (playerCanOrb)
        //    {
        //        if (!Weapon.Instance.GravityOrbInput())
        //            UIOrb.Instance.cantOrb();
        //    }
        //    else UIOrb.Instance.cantOrb();
        //}

        if (isArduinoMode && Input.GetKeyDown(KeyCode.Mouse0)) isArduinoMode = false;
        if (!isArduinoMode && (arduinoTransmettor && arduinoTransmettor.isShotUp)) isArduinoMode = true;

        if(Weapon.Instance != null)
        {
            //SHOOT
            if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isGravityUp) : Input.GetKeyUp(KeyCode.Mouse1)))
            {
                if (playerCanOrb)
                {
                    if (!Weapon.Instance.GravityOrbInput() && !inWaitScreen)
                        UIOrb.Instance.cantOrb();
                }
                else if (!inWaitScreen)UIOrb.Instance.cantOrb();
            }
            if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isGravityHeld) : Input.GetKey(KeyCode.Mouse1)) && playerCanOrb)
                Weapon.Instance.displayOrb = true;
            else
                Weapon.Instance.displayOrb = false;


            if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotHeld) : Input.GetKey(KeyCode.Mouse0)) && playerCanShoot && playerCanShotgun)
            {
                Weapon.Instance.InputHold(GetCursorPos());
            }
            else
            {
                Weapon.Instance.InputUnHold();
            }

            if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) && playerCanShoot)
            {
                Weapon.Instance.InputUp(GetCursorPos());
            }
            if (!playerCanShoot) Weapon.Instance.CanNotShoot();


            // ------------------------- SKIP BUTTON ---------------------------------------------------------------------------------------------------------------
            if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) && inWaitScreen && !inSkip)
            {
                if (FastForwardButton.Instance != null && releasedInput)
                {
                    FastForwardButton.Instance.Pop();
                    timeSkipButtonVisible = timeSkipButtonStayVisible;
                }
            }
            if (timeSkipButtonVisible > 0)
            {
                timeSkipButtonVisible -= Time.unscaledDeltaTime;
                if (timeSkipButtonVisible < 0)
                {
                    if (FastForwardButton.Instance != null)
                    {
                        FastForwardButton.Instance.Depop();
                    }
                }
            }
            if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotHeld) : Input.GetKey(KeyCode.Mouse0)) && inWaitScreen && !inSkip)
            {
                if (FastForwardButton.Instance != null && releasedInput)
                {
                    FastForwardButton.Instance.Pop();
                    timeSkipButtonVisible = timeSkipButtonStayVisible;
                    if (FastForwardButton.Instance.InputHold(timeMustStayHoldToSkip))
                    {
                        FastForwardButton.Instance.Depop();
                        inSkip = true;
                    }
                }
            }
            else if (inWaitScreen && !inSkip && FastForwardButton.Instance != null)
            {
                if (FastForwardButton.Instance != null)
                {
                    FastForwardButton.Instance.InputUnHold(timeMustStayHoldToSkip);
                    releasedInput = true;
                }
            }
            // ------------------------- SKIP BUTTON END ---------------------------------------------------------------------------------------------------------------

            if (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) UILeaderboard.Instance.PlayerClicked();
        }

        //CAM
        Vector3 posCursor = GetCursorPos();
        if ((posCursor.x < IRCameraParser.Instance.iResolutionX && posCursor.x > 0 && posCursor.y < IRCameraParser.Instance.iResolutionY && posCursor.y > 0)
            || (posCursor.x < Screen.width && posCursor.x > 0 && posCursor.y < Screen.height && posCursor.y > 0))
            CameraHandler.Instance.DecalCamWithCursor(posCursor);



        //UI
        if (UiCrossHair.Instance != null)
        {

            UiCrossHair.Instance.UpdateCrossHair(GetCursorPos());
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.visible = true;
        }
        else
        {
            Cursor.visible = false;
        }

        #region Debug

        if (Input.GetKeyDown(KeyCode.V))
        {
            enableDebugInputs = !enableDebugInputs;
            if (DebugDisplayScript.Instance != null) DebugDisplayScript.Instance.SetDebugVisible(enableDebugInputs);
        }


        //DEBUG
        if (Input.GetKeyDown(KeyCode.A) && enableDebugInputs)
        {
            isArduinoMode = !isArduinoMode;
        }

        if (Input.GetKeyDown(KeyCode.X) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            Debug.Log("EXPLOSION");
            ExplosionFromPlayer(30, 0, 500, 0, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.C) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            UiCrossHair.Instance.StopWaitFunction();
            playerCanOrb = true;
            playerCanReload = true;
            playerCanZeroG = true;
            playerCanPerfectReload = true;
            playerCanShoot = true;
            playerCanShotgun = true;
        }

        if (Input.GetKeyDown(KeyCode.B) && enableDebugInputs)
        {
            if (QualityHandler.Instance != null) ChangeQuality (!QualityHandler.Instance.isHighQuality);
        }

        if (Input.GetKeyDown(KeyCode.N) && enableDebugInputs)
        {
            SequenceHandler.Instance.NextSequence(true);
            //MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
        }

        if (Input.GetKeyDown(KeyCode.D) && enableDebugInputs)
        {
            Player.Instance.TakeDamage(34);
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
        }

        if (Input.GetKeyDown(KeyCode.Y) && enableDebugInputs)
        {
            mainMixer.SetFloat("GameVolume", 0);
            SceneHandler.Instance.RestartScene(.3f, true);
            CustomSoundManager.Instance.PlaySound("RestartSound", "EndGame", 1);
        }

        if (Input.GetKeyDown(KeyCode.U) && enableDebugInputs)
        {
            mainMixer.SetFloat("GameVolume", 0);
            SceneHandler.Instance.ChangeScene("MenuScene",.3f, true);
            CustomSoundManager.Instance.PlaySound("RestartSound", "EndGame", 1);
        }

        //if (Input.GetKeyDown(KeyCode.W) && enableDebugInputs)
        //{
        //    EasterEggHandler.Instance.UnlockAllBonusAtNextGame();
        //}
        if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftControl)&& enableDebugInputs)
        {
            EasterEggHandler.Instance.UnlockAllBonusAtNow();
            EasterEggHandler.Instance.EnableAllBonusAtNow();
        }
        if (Input.GetKeyDown(KeyCode.W) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && enableDebugInputs)
        {
            EasterEggHandler.Instance.DisableAllBonus();
        }

        if (Input.GetKeyDown(KeyCode.P) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            this.sequenceSkipMode = !this.sequenceSkipMode;
            Debug.Log($"Sequence skip : {sequenceSkipMode}");
            sequenceCheat = "";
        }

        if (Input.GetKeyDown(KeyCode.K) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            Player.Instance.SetLifeTo(1);
            Player.Instance.GainArmor(-9999);
            Player.Instance.TakeDamage(1);
            CustomSoundManager.Instance.PlaySound("SE_Trap_Death", "UI", 2);
        }

        if (Input.GetKeyDown(KeyCode.G) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            Player.Instance.SetGod();
        }

        if (Input.GetKeyDown(KeyCode.Z) && enableDebugInputs)
        {
            SetupWaitScreenOn();
        }

        if (Input.GetKeyUp(KeyCode.Z) && enableDebugInputs)
        {
            SetupWaitScreenOff();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Cheat, Vector3.zero, null, 20000);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Cheatbad, Vector3.zero, null, 20000);
        }

        if (Input.GetKeyDown(KeyCode.S) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            TimeScaleManager.Instance.AddSlowMo(0.8f, 5);
        }
        if (Input.GetKeyDown(KeyCode.E) && enableDebugInputs)
        {
            HintScript.Instance.PopHint("Veuillez vous approcher de l'écran s'il vous plait !", 5);
        }

        if (Input.GetKeyDown(KeyCode.T) && enableDebugInputs) PostprocessManager.Instance.setChroma(!PostprocessManager.Instance.Chroma);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (GamePaused)
            {
                CameraHandler.Instance.EndDiorama();
                InDiorama = false;
                SetupWaitScreenOff(true);
                TimeScaleManager.Instance.RemoveStopTime();
                GamePaused = false;
            }
            else
            {
                SetupWaitScreenOn(true);
                TimeScaleManager.Instance.AddStopTime(5000);
                GamePaused = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && enableDebugInputs)
        {
            if (GamePaused)
            {
                if (InDiorama)
                {
                    CameraHandler.Instance.EndDiorama();
                    InDiorama = false;
                }
                else
                {
                    CameraHandler.Instance.TriggerAnimDiorama();
                    InDiorama = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L) && enableDebugInputs)
        {
            if (CheatDisplayHandler.Instance != null) CheatDisplayHandler.Instance.ChangeCheatDisplay();
        }

        if (Input.GetKeyDown(KeyCode.O) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            playerCanOrb = !playerCanOrb;
        }
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    Debug.Log(LeaderboardManager.Instance.GetHighestScore().score);
        //}
        if (Input.GetKeyDown(KeyCode.M) && enableDebugInputs)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            if (!playerInLeaderboard)
                InitLeaderboard();
            else
                UILeaderboard.Instance.NextScreen();
        }

        if (sequenceSkipMode)
        {
            #region Numeric inputs
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                sequenceCheat += "0";
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                sequenceCheat += "1";
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                sequenceCheat += "2";
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                sequenceCheat += "3";
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                sequenceCheat += "4";
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                sequenceCheat += "5";
            }
            if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                sequenceCheat += "6";
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                sequenceCheat += "7";
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                sequenceCheat += "8";
            }
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                sequenceCheat += "9";
            }
            #endregion
            if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && enableDebugInputs)
            {
                if(sequenceCheat != "")
                {
                    if (PlaneShakeManager.Instance!=null)
                        PlaneShakeManager.Instance.activated = false;

                    if (CameraHandler.Instance != null)
                        CameraHandler.Instance.ChangeNoiseSettings(0, .1f);

                    int sequenceToGo = int.Parse(sequenceCheat);

                    if (MusicHandler.Instance != null)
                    {
                        MusicHandler.Instance.PlayMusic(0,MusicHandler.Musics.none, 0, 1, 0, 0, .3f, true, false);
                        //MusicHandler.Instance.PlayMusic(0,MusicHandler.Musics.introPreLastStage, 0, 1, 0, 0, .3f, true, false);
                        //MusicHandler.Instance.PlayMusic(0,MusicHandler.Musics.preLastStage, 0, 0, 0, 0, .3f, false, true);
                    }

                    sequenceCheat = "";
                    SequenceHandler.Instance.SkipToSequence(sequenceToGo);
                    SetupWaitScreenOff();
                    UiCrossHair.Instance.StopWaitFunction();
                    playerCanOrb = true;
                    playerCanReload = true;
                    playerCanZeroG = true;
                    playerCanPerfectReload = true;
                    playerCanShoot = true;
                    playerCanShotgun = true;
                    UiCrossHair.Instance.UpdateCursorUnlocks();
                    Player.Instance.GainArmorOverTime(0, 100, 100);
                    TriggerUtil.TriggerPlaneShake(false, 0);

                    TriggerSender[] AllTriggerSender = FindObjectsOfType<TriggerSender>();
                    for (int i = 0; i < AllTriggerSender.Length; i++)
                    {
                        if (AllTriggerSender[i].triggeredBySkip)
                            AllTriggerSender[i].ActivateGOAtSkip();
                    }
                }
                
            }
            
        }
        #endregion

        //RELOAD
        if (playerCanReload && (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isReloadDown) : Input.GetKeyDown(KeyCode.R)) && !playerInLeaderboard)
        {
            if (Weapon.Instance.ReloadValidate())
                Weapon.Instance.ReloadingInput();
        }

        TimeScaleManager.Instance.AccelGame(((Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.Q)) && enableDebugInputs) || inSkip, inSkip ? 5 : (Input.GetKey(KeyCode.H) ? 5 : 10));

        //if (playerCanShoot && (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotDown) : Input.GetKeyUp(KeyCode.Mouse0)) && Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)
        //{
        //    if (Weapon.Instance.ReloadValidate())
        //        Weapon.Instance.ReloadingInput();
        //}

        if (Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)Weapon.Instance.ReloadingInput();

        if (saveIfPlayerCouldReload && !playerCanReload)
        {
            Weapon.Instance.EndReload(playerCanPerfectReload, false);
        }
        saveIfPlayerCouldReload = playerCanReload;

        if (timeLeftForRaycastCursor <= timeTickCursor)
        {
            Ray cursorRay = CameraHandler.Instance.renderingCam.ScreenPointToRay(GetCursorPos());
            RaycastHit hit;
            Physics.Raycast(cursorRay, out hit, Mathf.Infinity);
            
            if(hit.collider != null)
            {
                IDetection detect = hit.collider.GetComponent<IDetection>();
                if (detect != null)
                {
                    detect.OnCursorClose(hit.point);
                }
                   


            }

            timeLeftForRaycastCursor = timeTickCursor;
        }
        else
        {
            timeLeftForRaycastCursor -= Time.deltaTime;
        }

        if (timeRemainingBeforeChoice > 0)
        {
            timeRemainingBeforeChoice -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeChoice < 0)
            {
                timeRemainingBeforeChoice = 0;
                CanDoLastChoice();
            }
        }

        if (lastChoiceForPlayer && !GameEnded)
        {
            if (lastChanceButton.allButtons != null && choiceMade == -1)
            {
                bool aButtonIsMouseOvered = false;
                bool malusButtonMouseOvered = false;
                foreach (lastChanceButton button in lastChanceButton.allButtons)
                {
                    if (button != null && button.enabled)
                    {
                        bool saveMouseOver = button.isMouseOvered;
                        if (button.CheckIfMouseOver())
                        {
                            button.baseScale = Mathf.Lerp(button.baseScale, buttonMouseOverScale, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                            button.CvsGroup.alpha = Mathf.Lerp(button.CvsGroup.alpha,1, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                            buttonMouseOver = button;
                            aButtonIsMouseOvered = true;
                            button.AnimateIfMouseOver();
                            if (button.ButtonType == lastChanceButton.typeOfButton.beg) malusButtonMouseOvered = true;
                        }
                        else
                        {
                            if (buttonMouseOver)
                            {
                                button.baseScale = Mathf.Lerp(button.baseScale, buttonOtherMouseOverScale, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                                button.CvsGroup.alpha = Mathf.Lerp(button.CvsGroup.alpha, 0.2f, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                            }
                            else
                            {
                                button.baseScale = Mathf.Lerp(button.baseScale, buttonNotMouseOverScale, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                                button.CvsGroup.alpha = Mathf.Lerp(button.CvsGroup.alpha, 0.7f, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                            }
                            button.unanimateIfNoMouseOver();
                        }
                        if (saveMouseOver != button.isMouseOvered && CustomSoundManager.Instance != null)
                            CustomSoundManager.Instance.PlaySound("SE_MouseOverUI", "UI", null, 2, false, 1, 0, 0, 3);
                    }
                }
                if (!aButtonIsMouseOvered) buttonMouseOver = null;
                if (malusButtonMouseOvered) EndGameChoice.Instance.MalusButtonMouseOvered();
                else EndGameChoice.Instance.MalusButtonNotMouseOvered();
            }

            if (timeRemainingBeforeGameOver > 0)
            {
                timeRemainingBeforeGameOver -= Time.unscaledDeltaTime;
                if (Mathf.CeilToInt(timeRemainingBeforeGameOver) != lastTimeRemainingBeforeGameOver)
                {
                    lastTimeRemainingBeforeGameOver = Mathf.CeilToInt(timeRemainingBeforeGameOver);
                    CustomSoundManager.Instance.PlaySound("SE_Tick", "UI", 1);
                }
            }
            if (timeRemainingBeforeGameOver < 0)
            {
                timeRemainingBeforeGameOver = 0;
                ValidateEndGameChoice(2);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) ReviveChoice();
            if (Input.GetKeyDown(KeyCode.RightArrow)) VoteChoice();

            if (timeRemainingBeforeChoiceSecurity > 0)
                timeRemainingBeforeChoiceSecurity -= Time.unscaledDeltaTime;

            if (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotDown) : Input.GetKeyDown(KeyCode.Mouse0) && timeRemainingBeforeChoiceSecurity < 0)
            {
                foreach (var button in lastChanceButton.allButtons)
                {
                    int buttonClicked = button.Click();
                    if (buttonClicked != -1) ValidateEndGameChoice(buttonClicked);
                }
            }

        }

        if (timeBeforeChoiceDone > 0)
        {
            timeBeforeChoiceDone -= Time.unscaledDeltaTime;
            if (timeBeforeChoiceDone < 0)
            {
                timeBeforeChoiceDone = 0;
                DoWhatPlayerChoosed(choiceMade);
            }
        }

        if (timeGoToLeaderboard > 0)
        {
            timeGoToLeaderboard -= Time.unscaledDeltaTime;
            if (timeGoToLeaderboard <= 0)
                InitLeaderboard();
        }
#if !UNITY_EDITOR
        CheckIfGoBackToMenu();
#endif
    }

    public void ChangeQuality (bool high)
    {
        //RenderSettings.fog = high;
        if (QualityHandler.Instance != null) QualityHandler.Instance.SetupQuality(high);
        if (objectToChangeInLowQuality != null)
        {
            for (int i = 0; i < objectToChangeInLowQuality.Length; i++)
            {
                if (objectToChangeInLowQuality[i] != null) objectToChangeInLowQuality[i].SetActive(!objectToChangeInLowQuality[i].activeSelf);
            }
        }
    }

    public void InitLeaderboard()
    {
        Weapon.Instance.SetMinigun(false);
        SetupWaitScreenOn(true);
        TimeScaleManager.Instance.AddStopTime(5000);
        mainMixer.SetFloat("GameVolume", -80);
        CameraHandler.Instance.RemoveShake();
        MetricsGestionnary.Instance.EndMetrics();
        TitlesManager.Instance.CalculateScores();
        //TimeScaleManager.Instance.AddStopTime(5000);
        //Debug.Log("Ici on met le vrai score");
        //UILeaderboard.Instance.InitLeaderboard(Random.Range(0,200000));
        UILeaderboard.Instance.InitLeaderboard(PublicManager.Instance.GetNbViewers());
        playerCanShoot = true;
        playerCanReload = false;
        playerCanOrb = false;
        playerInLeaderboard = true;
        CustomSoundManager.Instance.PlaySound("Music_PassiveExplo", "Leaderboard", null, 1, true);
    }

    public void EndGame()
    {

        mainMixer.SetFloat("GameVolume", 0);
        //LeaderboardManager.Instance.SubmitScoreToLeaderboard(playerData.name, playerData.score, playerData.title);

        //Plus besoin des metrics. Donc bye bye
        //MetricsGestionnary.Instance.SaveMetrics();

        SceneHandler.Instance.ChangeScene("MenuScene", .3f, true);
        CustomSoundManager.Instance.PlaySound("RestartSound", "EndGame", 1);
    }

    void ValidateEndGameChoice(int choice)
    {
        if (timeBeforeChoiceDone == 0)
        {
            timeBeforeChoiceDone = timeAfterChoice;
            choiceMade = choice;
            foreach (lastChanceButton button in lastChanceButton.allButtons)
            {
                if (button != null && button.enabled)
                {
                    //button.DoAnim(choiceMade);
                }
            }
            EndGameChoice.Instance.AnimateEndOfChoice(choice == 1);
        }
    }

    void DoWhatPlayerChoosed(int choice)
    {
        switch (choice)
        {
            case 0:
                ReviveChoice();
                break;
            case 1:
                VoteChoice();
                break;
            default:
                EndGameChoice.Instance.EndChoice();
                DoGameOver();
                break;
        }
        choiceMade = -1;
        if (lifeAndDeathAudioSource != null && lifeAndDeathAudioSource.isPlaying)
        {
            lifeAndDeathAudioSource.Stop();
            lifeAndDeathAudioSource = null;
        }
    }

    public void ReviveChoice()
    {
        TimeScaleManager.Instance.Stop();

        float trueChance = GetCurrentChacesOfSurvival();
        float bonusFromRez = 0;
        if (trueChance > difficultyData.maxChanceOfSurvival)
        {
            bonusFromRez = trueChance - difficultyData.maxChanceOfSurvival;

            trueChance = difficultyData.maxChanceOfSurvival;
        }

        PublicManager.Instance.LoseRawViewer(Mathf.RoundToInt(difficultyData.malusScoreAtChoosedRevive * PublicManager.Instance.GetNbViewers()));
        Main.Instance.EndReviveSituation(true, bonusFromRez);
        lastChoiceForPlayer = false;
        EndGameChoice.Instance.EndChoice();

        if (EnableComments)
        {
            PlaySoundWithDelay("PresA_Beg_Mercy", "Comment", Main.Instance.CommentAVolume, 1);
            PlaySoundWithDelay("PresB_Beg_Mercy", "Comment", Main.Instance.CommentBVolume, 4.5f);
        }
    }

    public void PlaySoundWithDelay(string sound,string mixer, float volume, float delay)
    {
        StartCoroutine(PlaySoundWithDelayCoroutine(sound, mixer, volume, delay));
    }

    IEnumerator PlaySoundWithDelayCoroutine(string sound,string mixer, float volume, float delay)
    {
        if (delay!=0)
            yield return new WaitForSecondsRealtime(delay);
            CustomSoundManager.Instance.PlaySound(sound, mixer, volume);
        yield break;
    }

    public void VoteChoice()
    {
        if (EnableComments)
        {
            PlaySoundWithDelay("PresA_Vote_Public", "Comment", Main.Instance.CommentAVolume, 1);
            PlaySoundWithDelay("PresB_Vote_Public", "Comment", Main.Instance.CommentBVolume, 4.5f);
        }
        TriggerGameOverSequence();
        lastChoiceForPlayer = false;
        //EndGameChoice.Instance.EndChoice();
    }

    List<EndGameBonus> allEndGameBonus = new List<EndGameBonus>();
    public List<EndGameBonus> AllEndGameBonus { get { return allEndGameBonus; } }
    public void AddEndGameBonus(float currValue, float maxValue, string type, int addedScore, string title, string description, int spriteType = (int)DataProgressSprite.SpriteNeeded.Gladiator, string addedCharacter = "")
    {
        if (currValue >= maxValue) Main.Instance.TitlesUnlocked.Add(title);
        EndGameBonus newInstance = new EndGameBonus(currValue, maxValue, type, addedScore, title, description, spriteType, addedCharacter);
        allEndGameBonus.Add(newInstance);
        //UILeaderboard.Instance.AddMetricToDisplay(type, currValue.ToString() + addedCharacter, maxValue.ToString() + addedCharacter, currValue >= maxValue);
    }

    [SerializeField] private float checkInputEvery = 0.5f;
    [SerializeField] private float distanceCheckIfInput = 0.03f;
    float timerCheckInput = 0;
    [SerializeField] private float timeBeforeGoBackToMenu = 30;
    private float timerGoToMenu = 5;
    private Vector3 saveLastCursorPos = Vector3.zero;
    [SerializeField] private Text timeRestartText = null;

    bool PlayerIsMakingInput()
    {
        return (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isGravityUp) : Input.GetKeyUp(KeyCode.Mouse1)) ||
            (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isGravityHeld) : Input.GetKey(KeyCode.Mouse1)) ||
            (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotHeld) : Input.GetKey(KeyCode.Mouse0)) ||
            (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) ||
            (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isReloadDown) : Input.GetKeyDown(KeyCode.R));
    }

    void CheckIfGoBackToMenu()
    {
        Vector3 posCursor = GetCursorPos();
        timerCheckInput -= Time.unscaledDeltaTime;
        if (timerCheckInput < 0)
        {
            timerCheckInput += checkInputEvery;
            float currDist = Mathf.Sqrt(
                Mathf.Pow(Mathf.Abs(saveLastCursorPos.x - posCursor.x) / Screen.width, 2) +
                Mathf.Pow(Mathf.Abs(saveLastCursorPos.y - posCursor.y) / Screen.height, 2));

            if (currDist > distanceCheckIfInput || GameEnded || inWaitScreen || GamePaused || PlayerIsMakingInput() || playerInLeaderboard || lastChoiceForPlayer || Weapon.Instance.IsMinigun)
            {
                timerGoToMenu = timeBeforeGoBackToMenu;
                if (timeRestartText != null) timeRestartText.text = "";
            }
            else
            {
                if (timerGoToMenu < checkInputEvery)
                {
                    if (!lockSceneChange)
                    {
                        lockSceneChange = true;
                        SceneHandler.Instance.ChangeScene("MenuScene", .3f, true);
                    }
                }
                else
                {
                    timerGoToMenu -= checkInputEvery;
                    if (timerGoToMenu < timeBeforeGoBackToMenu / 2)
                    {
                        if (timeRestartText!=null)timeRestartText.text = "Game Restart in " + Mathf.FloorToInt(timerGoToMenu);
                    }
                }
            }

            saveLastCursorPos = posCursor;
        }
    }

    public void SetControlState(TriggerSender.Activable control, bool state)
    {

        if (control == TriggerSender.Activable.BaseWeapon || control == TriggerSender.Activable.Both)
        {
            playerCanShoot = state;
            if (!playerCouldShoot && state)
            {
                playerCouldShoot = true;
                if (UIUpgrade.Instance != null) UIUpgrade.Instance.PlayerGetAnUpgrade();
            }
        }

        if (control == TriggerSender.Activable.AutoReload) autoReloadOnNoAmmo = state;

        if (control == TriggerSender.Activable.PerfectReload)
        {
            playerCanPerfectReload = state;
            if (!playerCouldPerfectReload && state)
            {
                playerCouldPerfectReload = true;
                if (UIUpgrade.Instance != null) UIUpgrade.Instance.PlayerGetAnUpgrade();
            }
        }

        if (control == TriggerSender.Activable.Shotgun)
        {
            playerCanShotgun = state;
            if (!playerCouldShotgun && state)
            {
                playerCouldShotgun = true;
                if (UIUpgrade.Instance != null) UIUpgrade.Instance.PlayerGetAnUpgrade();
            }
        }

        if (control == TriggerSender.Activable.Reload)
        {
            playerCanReload = state;
            if (!playerCouldReload && state)
            {
                playerCouldReload = true;
                if (UIUpgrade.Instance != null) UIUpgrade.Instance.PlayerGetAnUpgrade();
            }
        }

        if (control == TriggerSender.Activable.ZeroG)
        {
            playerCanZeroG = state;
            if (!playerCouldZeroG && state)
            {
                playerCouldZeroG = true;
                if (UIUpgrade.Instance != null) UIUpgrade.Instance.PlayerGetAnUpgrade();
            }
        }

        if (control == TriggerSender.Activable.Orb || control == TriggerSender.Activable.Both)
        {
            if (!playerCanOrb && state) UIOrb.Instance.ActivateOrb();
                playerCanOrb = state;

            if (!playerCouldOrb && state)
            {
                playerCouldOrb = true;
                if (UIUpgrade.Instance != null) UIUpgrade.Instance.PlayerGetAnUpgrade();
            }

            //FindObjectOfType<C_Ui>().CannotShoot(state);
        }
        if (state) UiCrossHair.Instance.UpdateCursorUnlocks();
    }

    public void CutMusic()
    {
        if (hSoundHandlerMainMusic != null)
            hSoundHandlerMainMusic.volume = 0;
    }

    public void PreventPlayerFromRevive()
    {
        playerResedAlready = true;
    }

    public void FinalChoice()
    {

        mainMixer.SetFloat("PitchAffectedVolume", -80);

        if (playerCanOrb)
        {
            playerUsedToHaveOrb = true;
        }

        overrideUiCrosshairInterdictionGraph = true;
        playerCanShoot = false;
        playerCanReload = false;
        playerCanOrb = false;


        if (PostprocessManager.Instance != null) PostprocessManager.Instance.SetupSaturation(-100, 1f);
        if (difficultyData.playerCanReraise || !playerResedAlready)
        {
            if (EnableComments)
            {
                PlaySoundWithDelay("PresA_Player_Down", "Comment", Main.Instance.CommentAVolume, .5f);
                PlaySoundWithDelay("PresB_Player_Down", "Comment", Main.Instance.CommentBVolume, 2.5f);
            }
            lifeAndDeathAudioSource = CustomSoundManager.Instance.PlaySound(musicLifeAndDeathChoice, "UI", musicLifeAndDeathChoiceVolume);
            timeRemainingBeforeChoice = timeBeforeChoice;
            timeRemainingBeforeChoiceSecurity = timeBeforeChoiceSecurity;
            timeRemainingBeforeGameOver = timerBeforeGameOver;
            TimeScaleManager.Instance.AddStopTime(5000);

            float trueChance = GetCurrentChacesOfSurvival();
            if (trueChance > difficultyData.maxChanceOfSurvival) trueChance = difficultyData.maxChanceOfSurvival;
            //Debug.Log(PublicManager.Instance.GetNbViewers() + " / " + difficultyData.malusScoreAtChoosedRevive);
            EndGameChoice.Instance.SetupChoice(Mathf.RoundToInt(difficultyData.malusScoreAtChoosedRevive * PublicManager.Instance.GetNbViewers()), Mathf.RoundToInt(trueChance), PublicManager.Instance.GetNbViewers());
        }
        else
        {
            DoGameOver();
        }


    }

    public void SetupWaitScreenOn (bool requestFromDiorama = false)
    {
        if (!inWaitScreen)
        {
            playerCanOrbWaitScreenSave = playerCanOrb;
            playerCanReloadWaitScreenSave = playerCanReload;
            playerCanZeroGWaitScreenSave = playerCanZeroG;
            playerCanPerfectReloadWaitScreenSave = playerCanPerfectReload;
            playerCanShootWaitScreenSave = playerCanShoot;
            playerCanShotgunWaitScreenSave = playerCanShotgun;

            playerCanOrb = false;
            playerCanReload = false;
            playerCanZeroG = false;
            playerCanPerfectReload = false;
            playerCanShoot = false;
            playerCanShotgun = false;
            UiCrossHair.Instance.WaitFunction();

            inWaitScreen = true;
            releasedInput = false;
        }
        else if (requestFromDiorama && inWaitScreen)
        {
            wasInWaitScreen = true;
        }

    }
    public void SetupWaitScreenOff(bool requestFromDiorama = false)
    {
        if (!Input.GetKey(KeyCode.Z))
        {
            if (inWaitScreen && !(requestFromDiorama && wasInWaitScreen))
            {
                playerCanOrb = playerCanOrbWaitScreenSave;
                playerCanReload = playerCanReloadWaitScreenSave;
                playerCanZeroG = playerCanZeroGWaitScreenSave;
                playerCanPerfectReload = playerCanPerfectReloadWaitScreenSave;
                playerCanShoot = playerCanShootWaitScreenSave;
                playerCanShotgun = playerCanShotgunWaitScreenSave;
                UiCrossHair.Instance.StopWaitFunction();
                if (FastForwardButton.Instance != null) FastForwardButton.Instance.Depop();
                inSkip = false;

                inWaitScreen = false;
                releasedInput = false;
            }
            wasInWaitScreen = false;
        }

    }

    void CanDoLastChoice()
    {
        lastChoiceForPlayer = true;
        timeRemainingBeforeGameOver = timerBeforeGameOver;
        EndGameChoice.Instance.ActivateChoice();

    }

    public void TriggerGameOverSequence()
    {

        if (difficultyData.playerCanReraise || !playerResedAlready)
        {

            // Debug.Log($"{initialPublic} {currentPublic} {growthValue}");
            float trueChance = GetCurrentChacesOfSurvival();

            float bonusFromRez = 0;

           

            if (trueChance > difficultyData.maxChanceOfSurvival)
            {
                bonusFromRez = trueChance - difficultyData.maxChanceOfSurvival;

                trueChance = difficultyData.maxChanceOfSurvival;
            }

            int publicChoice = Random.Range(0, 101);

            UiViewer.Instance.PlayerJustDied(publicChoice < trueChance, publicChoice, bonusFromRez, trueChance);

            //Debug.Log($"Required : {trueChance} -- Chance : {publicChoice}");
        }
        else
        {
            DoGameOver();
        }
        
    }

    public void EndReviveSituation(bool rez, float bonusFromRez)
    {

        if (rez)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.Resurrection);

            CustomSoundManager.Instance.PlaySound("Crowd_Cheer", "EndGame", 0.5f);
            CustomSoundManager.Instance.PlaySound("Bell_Up", "EndGame", 1);
            DoResurrection(bonusFromRez);
            playerResedAlready = true;
            if (PostprocessManager.Instance != null) PostprocessManager.Instance.SetupSaturation(0, 0.5f);
            mainMixer.SetFloat("PitchAffectedVolume", 0);
        }
        else
        {
            CustomSoundManager.Instance.PlaySound("Crowd_Boo", "EndGame", 0.2f);
            CustomSoundManager.Instance.PlaySound("Bell_Down", "EndGame", 1);
            DoGameOver();
        }
    }

    private void DoGameOver()
    {
        //Debug.Log("Public chose... DEATH");
        TimeScaleManager.Instance.AddStopTime(5000);
        Player.Instance.DieForReal();
        UiLifeBar.Instance.EndGame();
        GameEnded = true;
        timeGoToLeaderboard = timeGoToLeaderboardAtGameOver;

        //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "GameOver_Sound", false, 1);
        CustomSoundManager.Instance.PlaySound("GameOver_Sound", "EndGame", 1);

        if (EnableComments)
        {
            PlaySoundWithDelay("PresA_Game_Over", "Comment", Main.Instance.CommentAVolume, .5f);
            PlaySoundWithDelay("PresB_Game_Over", "Comment", Main.Instance.CommentBVolume, 3.8f);
        }
    }

    private void DoResurrection(float bonus)
    {
        overrideUiCrosshairInterdictionGraph = false;
        playerCanShoot = true;
        playerCanReload = true;
        if(playerUsedToHaveOrb) playerCanOrb = true;

        Player.Instance.SetLifeTo(Mathf.RoundToInt(Player.Instance.GetBaseValues().y / 5));

        float armorValueGain = difficultyData.armorOnRaise + bonus * difficultyData.armorOnRaiseBonus / (int)difficultyData.difficulty;
        armorValueGain = Mathf.Clamp(armorValueGain, 0, difficultyData.maxArmorRaise);
        Player.Instance.GainArmor(armorValueGain);
        Player.Instance.Revive();


        if(bonus > 0)
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.BonusOnRespawn, Vector3.zero, null, bonus);
        }
        
    }

    public void ExplosionFromPlayer(float explosionRadius, float explosionForce, float explosionDamage, float explosionStun, float explosionStunDuration, float explosionLiftValue, bool damageCamera = true)
    {

        Collider[] tHits = Physics.OverlapSphere(Player.Instance.transform.position, explosionRadius);

        TimeScaleManager.Instance.AddSlowMo(0.8f, 3);

        foreach (Collider hVictim in tHits)
        {
            if (hVictim.gameObject != this.gameObject)
            {
                IEntity entityVictim = hVictim.GetComponent<IEntity>();
                ISpecialEffects speAffect = hVictim.GetComponent<ISpecialEffects>();
                if (speAffect != null && hVictim.GetComponent<Player>() == null)
                    speAffect.OnExplosion(Player.Instance.transform.position, explosionForce, explosionRadius, explosionDamage, explosionStun, explosionStunDuration, explosionLiftValue, damageCamera);
            }
        }
        FxManager.Instance.PlayFx("VFX_ExplosionShooterBullet", Player.Instance.transform.position, Player.Instance.transform.rotation, explosionRadius);
        CustomSoundManager.Instance.PlaySound("Crowd_Cheer", "EndGame", 0.5f);
        CustomSoundManager.Instance.PlaySound("Bell_Up", "EndGame", 1);
    }

    public Vector3 GetCursorPos()
    {
        Vector2 returnedValue = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
        if (Weapon.Instance != null) returnedValue += Weapon.Instance.CursorImprecision;
        returnedValue = new Vector2(Mathf.Clamp(returnedValue.x, 0, Screen.width), Mathf.Clamp(returnedValue.y, 0, Screen.height));
        return returnedValue;
    }

    public float GetCurrentChacesOfSurvival()
    {
        float initialPublic = PublicManager.Instance.GetInitialViewers();
        float currentPublic = PublicManager.Instance.GetNbViewers() / PublicManager.scoreMultiplier;
        float growthValue = PublicManager.Instance.GetGrowthValue();

        // Debug.Log($"{initialPublic} {currentPublic} {growthValue}");

        return (currentPublic / (initialPublic + growthValue * (float)difficultyData.difficulty) / (float)difficultyData.difficulty) * difficultyData.maxChanceOfSurvival; ;
    }

    public void setAIWalls(bool state)
    {
        foreach(BoxCollider col in aiWalls)
        {
            col.enabled = state;
        }
    }

    private void SkipToSequence()
    {
        SequenceHandler.Instance.SkipToSequence(startWithCameraNumber);
    }
}
