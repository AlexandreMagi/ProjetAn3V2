﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Main : MonoBehaviour
{
    [SerializeField]
    private bool playerCanOrb = true;
    [SerializeField]
    private bool playerCanReload = true;
    [SerializeField]
    private bool playerCanZeroG = true;
    [SerializeField]
    private bool playerCanPerfectReload = true;
    [SerializeField]
    private bool playerCanShoot = true;
    [SerializeField]
    private bool playerCanShotgun = true;

    bool playerCouldShoot = false;
    bool playerCouldShotgun = false;
    bool playerCouldReload = false;
    bool playerCouldPerfectReload = false;
    bool playerCouldOrb = false;
    bool playerCouldZeroG = false;

    [HideInInspector] public bool overrideUiCrosshairInterdictionGraph = false;

    private bool playerUsedToHaveOrb = false;

    [HideInInspector] public bool playerInLeaderboard = false;

    public bool PlayerCanShoot { get { return playerCanShoot; } }
    public bool PlayerCanShotgun { get { return playerCanShotgun; } }
    public bool PlayerCanOrb { get { return playerCanOrb; } }
    public bool PlayerCanReload { get { return playerCanReload; } }
    public bool PlayerCanPerfectReload { get { return playerCanPerfectReload; } }
    public bool PlayerCanZeroG { get { return playerCanZeroG; } }

    private string sequenceCheat = "";
    private bool sequenceSkipMode = false;

    private float timeLeftForRaycastCursor;
    private float timeTickCursor = .2f;

    IRCameraParser arduinoTransmettor;

    [SerializeField]
    int startWithCameraNumber = 0;

    [SerializeField]
    DataDifficulty difficultyData = null;

    bool playerResedAlready = false;

    [SerializeField]
    bool autoReloadOnNoAmmo = false;

    bool hasJumpedCam = false;

    [HideInInspector]
    public bool GameEnded = false;
    public bool lockSceneChange = false;

    [SerializeField]
    bool isArduinoMode = false;

    [SerializeField]
    BoxCollider[] aiWalls = null;


    AudioSource hSoundHandlerMainMusic = null;

    bool saveIfPlayerCouldReload = true;


    // --- Variables choix final
    bool lastChoiceForPlayer = false;
    [SerializeField] float timeBeforeChoice = 2;
    float timeRemainingBeforeChoice = 0;
    [SerializeField] float timeAfterChoice = 1;
    [SerializeField] float timerBeforeGameOver = 10;
    float timeRemainingBeforeGameOver = 10;
    lastChanceButton buttonMouseOver = null;
    float buttonMouseOverLerpSpeed = 8;
    float buttonMouseOverScale = 1.2f;
    float buttonNotMouseOverScale = 1f;
    float buttonOtherMouseOverScale = 0.8f;
    float timeBeforeChoiceDone = 0;
    int choiceMade = -1;

    public float TimeRemainingBeforeGameOver { get { return timeRemainingBeforeGameOver; } }

    [HideInInspector] public List<string> TitlesUnlocked = new List<string>();

    public static Main Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Remettre la musique");
        //hSoundHandlerMainMusic = CustomSoundManager.Instance.PlaySound("Drone_Ambiant", "Music",null, 0.5f, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (arduinoTransmettor == null)
        {
            arduinoTransmettor = IRCameraParser.Instance;
        }

        if (!hasJumpedCam && startWithCameraNumber != 0)
        {
            hasJumpedCam = true;
            SequenceHandler.Instance.SkipToSequence(startWithCameraNumber);
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

        //SHOOT
        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isGravityUp) : Input.GetKeyUp(KeyCode.Mouse1)))
        {
            if (playerCanOrb)
            {
                if (!Weapon.Instance.GravityOrbInput())
                    UIOrb.Instance.cantOrb();
            }
            else UIOrb.Instance.cantOrb();
        }
        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isGravityHeld) : Input.GetKey(KeyCode.Mouse1)) && playerCanOrb)
            Weapon.Instance.displayOrb = true;
        else
            Weapon.Instance.displayOrb = false;


        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotHeld) : Input.GetKey(KeyCode.Mouse0)) && playerCanShoot && playerCanShotgun)
        {
            Weapon.Instance.InputHold();
        }

        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) && playerCanShoot)
        {
            Weapon.Instance.InputUp(GetCursorPos());
        }
        if (!playerCanShoot) Weapon.Instance.CanNotShoot();


        if (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) UILeaderboard.Instance.PlayerClicked();

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
        //DEBUG
        if (Input.GetKeyDown(KeyCode.A))
        {
            isArduinoMode = !isArduinoMode;
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            UiCrossHair.Instance.StopWaitFunction();
            playerCanOrb = true;
            playerCanReload = true;
            playerCanZeroG = true;
            playerCanPerfectReload = true;
            playerCanShoot = true;
            playerCanShotgun = true;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log($"Current sequence index :{SequenceHandler.Instance.GetCurrentSequenceIndex()}");
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SequenceHandler.Instance.NextSequence(true);
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Player.Instance.TakeDamage(34);
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneHandler.Instance.RestartScene(.3f, true);
           CustomSoundManager.Instance.PlaySound("RestartSound", "UI",1);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            SceneHandler.Instance.ChangeScene("MenuScene",.3f, true);
            CustomSoundManager.Instance.PlaySound("RestartSound", "UI",1);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            this.sequenceSkipMode = !this.sequenceSkipMode;
            Debug.Log($"Sequence skip : {sequenceSkipMode}");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            Player.Instance.SetLifeTo(1);
            Player.Instance.GainArmor(-9999);
            Player.Instance.TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            Player.Instance.SetGod();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Cheat, Vector3.zero, null, 50);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Cheat, Vector3.zero, null, -50);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            TimeScaleManager.Instance.AddSlowMo(0.8f, 5);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            HintScript.Instance.PopHint("Veuillez vous approcher de l'écran s'il vous plait !", 5);
        }
        /*if (Input.GetKeyDown(KeyCode.F))
        {
            HintScript.Instance.PopHint("Merci d'avoir joué à Death Live !", 5);
        }*/
        if (Input.GetKeyDown(KeyCode.O))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            playerCanOrb = !playerCanOrb;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedCheatCode);
            Debug.Log(LeaderboardManager.Instance.GetHighestScore().score);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
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
            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                if(sequenceCheat != "")
                {
                    int sequenceToGo = int.Parse(sequenceCheat);

                    sequenceCheat = "";
                    SequenceHandler.Instance.SkipToSequence(sequenceToGo);
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

        TimeScaleManager.Instance.AccelGame(Input.GetKey(KeyCode.H), 5);

        //if (playerCanShoot && (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotDown) : Input.GetKeyUp(KeyCode.Mouse0)) && Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)
        //{
        //    if (Weapon.Instance.ReloadValidate())
        //        Weapon.Instance.ReloadingInput();
        //}

        if (Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)Weapon.Instance.ReloadingInput();

        if (saveIfPlayerCouldReload && !playerCanReload)
        {
            Weapon.Instance.EndReload(true, false);
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
        CheckIfGoBackToMenu();

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
                foreach (lastChanceButton button in lastChanceButton.allButtons)
                {
                    if (button != null && button.enabled)
                    {
                        if (button.CheckIfMouseOver())
                        {
                            button.baseScale = Mathf.Lerp(button.baseScale, buttonMouseOverScale, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                            button.CvsGroup.alpha = Mathf.Lerp(button.CvsGroup.alpha,1, Time.unscaledDeltaTime * buttonMouseOverLerpSpeed);
                            buttonMouseOver = button;
                            aButtonIsMouseOvered = true;
                            button.AnimateIfMouseOver();
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
                    }
                }
                if (!aButtonIsMouseOvered) buttonMouseOver = null;
            }

            if (timeRemainingBeforeGameOver > 0)
                timeRemainingBeforeGameOver -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeGameOver < 0)
            {
                timeRemainingBeforeGameOver = 0;
                ValidateEndGameChoice(2);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) ReviveChoice();
            if (Input.GetKeyDown(KeyCode.RightArrow)) VoteChoice();

            if (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotDown) : Input.GetKeyDown(KeyCode.Mouse0))
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
    }

    public void InitLeaderboard()
    {
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
    }

    public void EndGame(LeaderboardData playerData)
    {

        LeaderboardManager.Instance.SubmitScoreToLeaderboard(playerData.name, playerData.score, playerData.title);

        MetricsGestionnary.Instance.SaveMetrics();

        SceneHandler.Instance.ChangeScene("MenuScene", .3f, true);
        CustomSoundManager.Instance.PlaySound("RestartSound", "UI", 1);
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

        PublicManager.Instance.LoseRawViewer(Mathf.RoundToInt(difficultyData.malusScoreAtChoosedRevive * PublicManager.scoreMultiplier));
        Main.Instance.EndReviveSituation(true, bonusFromRez);
        lastChoiceForPlayer = false;
        EndGameChoice.Instance.EndChoice();
    }

    public void VoteChoice()
    {
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
    //[SerializeField] private float distanceCheckIfInput = 0.03f;
    float timerCheckInput = 0;
    [SerializeField] private float timeBeforeGoBackToStart = 10;
    private float timerGoBack = 5;
    private Vector3 saveLastCursorPos = Vector3.zero;
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


            if (/*currDist > distanceCheckIfInput || */!GameEnded)
                timerGoBack = timeBeforeGoBackToStart;

            else
            {
                if (timerGoBack < checkInputEvery)
                {
                    if (!lockSceneChange)
                    {
                        InitLeaderboard();
                        lockSceneChange = true;
                        //SceneHandler.Instance.ChangeScene("MenuScene", .3f, true);
                    }
                }
                else
                    timerGoBack -= checkInputEvery;
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

    public void FinalChoice()
    {
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
            timeRemainingBeforeChoice = timeBeforeChoice;
            timeRemainingBeforeGameOver = timerBeforeGameOver;
            TimeScaleManager.Instance.AddStopTime(5000);

            float trueChance = GetCurrentChacesOfSurvival();
            if (trueChance > difficultyData.maxChanceOfSurvival) trueChance = difficultyData.maxChanceOfSurvival;
            EndGameChoice.Instance.SetupChoice(Mathf.RoundToInt(difficultyData.malusScoreAtChoosedRevive * PublicManager.scoreMultiplier), Mathf.RoundToInt(trueChance));
        }
        else
        {
            DoGameOver();
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

            CustomSoundManager.Instance.PlaySound("Crowd_Cheer", "UI", 0.5f);
            CustomSoundManager.Instance.PlaySound("Bell_Up", "UI", 1);
            DoResurrection(bonusFromRez);
            playerResedAlready = true;
            if (PostprocessManager.Instance != null) PostprocessManager.Instance.SetupSaturation(0, 0.5f);
        }
        else
        {
            CustomSoundManager.Instance.PlaySound("Crowd_Boo", "UI", 0.2f);
            CustomSoundManager.Instance.PlaySound("Bell_Down", "UI", 1);
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

        //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "GameOver_Sound", false, 1);
        CustomSoundManager.Instance.PlaySound("GameOver_Sound", "UI", 1);
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

    public Vector3 GetCursorPos()
    {
        Vector2 returnedValue = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
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
}
