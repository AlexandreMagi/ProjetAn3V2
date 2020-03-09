﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Main : MonoBehaviour
{
    [SerializeField]
    private bool playerCanOrb = true;
    private bool playerCanShoot = true;
    private bool playerUsedToHaveOrb = false;

    public bool PlayerCanOrb {get { return playerCanOrb; } }

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

    bool saveIfPlayerCouldShoot = true;

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
    public float TimeRemainingBeforeGameOver {  get { return timeRemainingBeforeGameOver; } }

    public static Main Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        //Debug.Log("Remettre la musique");
        hSoundHandlerMainMusic = CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Drone_Ambiant", true, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(arduinoTransmettor == null)
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


        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotHeld) : Input.GetKey(KeyCode.Mouse0)) && playerCanShoot)
        {
            Weapon.Instance.InputHold();
        }

        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) && playerCanShoot)
        {
            Weapon.Instance.InputUp(isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition);
        }


        //CAM
        Vector3 posCursor = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
        if ( (posCursor.x < IRCameraParser.Instance.iResolutionX && posCursor.x > 0 && posCursor.y < IRCameraParser.Instance.iResolutionY && posCursor.y > 0)
            || (posCursor.x < Screen.width && posCursor.x > 0 && posCursor.y < Screen.height && posCursor.y > 0))
            CameraHandler.Instance.DecalCamWithCursor(posCursor);
        


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

        #region Debug
        //DEBUG
        if (Input.GetKeyDown(KeyCode.A))
        {
            isArduinoMode = !isArduinoMode;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log($"Current sequence index :{SequenceHandler.Instance.GetCurrentSequenceIndex()}");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SequenceHandler.Instance.NextSequence(true);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Player.Instance.TakeDamage(34);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneHandler.Instance.RestartScene(.3f, true);
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "RestartSound", false, 1);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            SceneHandler.Instance.ChangeScene("MenuScene",.3f, true);
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "RestartSound", false, 1);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            this.sequenceSkipMode = !this.sequenceSkipMode;
            Debug.Log($"Sequence skip : {sequenceSkipMode}");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Player.Instance.SetLifeTo(1);
            Player.Instance.GainArmor(-9999);
            Player.Instance.TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Player.Instance.SetGod();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.BonusOnRespawn, Vector3.zero, null, 50);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.BonusOnRespawn, Vector3.zero, null, -50);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            HintScript.Instance.PopHint("Veuillez vous approcher de l'écran s'il vous plait !", 5);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            HintScript.Instance.PopHint("Merci d'avoir joué à Death Live !", 5);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            playerCanOrb = !playerCanOrb;
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
        if (playerCanShoot && (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isReloadDown) : Input.GetKeyDown(KeyCode.R)))
        {
            if (Weapon.Instance.ReloadValidate())
                Weapon.Instance.ReloadingInput();
        }

        //if (playerCanShoot && (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotDown) : Input.GetKeyUp(KeyCode.Mouse0)) && Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)
        //{
        //    if (Weapon.Instance.ReloadValidate())
        //        Weapon.Instance.ReloadingInput();
        //}

        if (Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)Weapon.Instance.ReloadingInput();

        if (!saveIfPlayerCouldShoot && playerCanShoot)
        {
            Weapon.Instance.EndReload(true);
        }
        saveIfPlayerCouldShoot = playerCanShoot;

        if (timeLeftForRaycastCursor <= timeTickCursor)
        {
            Ray cursorRay = CameraHandler.Instance.renderingCam.ScreenPointToRay(isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition);
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
            if (lastChanceButton.allButtons != null)
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

            timeRemainingBeforeGameOver -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeGameOver < 0)
            {
                timeRemainingBeforeGameOver = 0;
                EndGameChoice.Instance.EndChoice();
                DoGameOver();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) ReviveChoice();
            if (Input.GetKeyDown(KeyCode.RightArrow)) VoteChoice();

            if (isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotDown) : Input.GetKeyDown(KeyCode.Mouse0))
            {
                foreach (var button in lastChanceButton.allButtons)
                {
                    button.Click();
                }
            }

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

        PublicManager.Instance.LoseRawViewer(difficultyData.malusScoreAtChoosedRevive);
        Main.Instance.EndReviveSituation(true, bonusFromRez);
        lastChoiceForPlayer = false;
        EndGameChoice.Instance.EndChoice();
    }

    public void VoteChoice()
    {
        TriggerGameOverSequence();
        lastChoiceForPlayer = false;
        EndGameChoice.Instance.EndChoice();
    }


    [SerializeField] private float checkInputEvery = 0.5f;
    [SerializeField] private float distanceCheckIfInput = 0.03f;
    float timerCheckInput = 0;
    [SerializeField] private float timeBeforeGoBackToStart = 10;
    private float timerGoBack = 5;
    private Vector3 saveLastCursorPos = Vector3.zero;
    void CheckIfGoBackToMenu()
    {
        Vector3 posCursor = isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
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
                        lockSceneChange = true;
                        SceneHandler.Instance.ChangeScene("MenuScene", .3f, true);
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
            if (state) UiCrossHair.Instance.StopWaitFunction();
            else UiCrossHair.Instance.WaitFunction();

            playerCanShoot = state;
            //FindObjectOfType<C_Ui>().CannotShoot(state);
            
            /*
            if (state)
            {
                wMod.InputUp(GetControllerPos());
            }
            */
        }

        if (control == TriggerSender.Activable.Orb || control == TriggerSender.Activable.Both)
        {
            if (!playerCanOrb && state) UIOrb.Instance.ActivateOrb();
                playerCanOrb = state;
            //FindObjectOfType<C_Ui>().CannotShoot(state);
        }
    }

    public void CutMusic()
    {
        hSoundHandlerMainMusic.volume = 0;
    }

    public void FinalChoice()
    {
        if (playerCanOrb)
        {
            playerUsedToHaveOrb = true;
        }

        playerCanShoot = false;
        playerCanOrb = false;


        if (difficultyData.playerCanReraise || !playerResedAlready)
        {
            timeRemainingBeforeChoice = timeBeforeChoice;
            timeRemainingBeforeGameOver = timerBeforeGameOver;
            TimeScaleManager.Instance.AddStopTime(5000);

            float trueChance = GetCurrentChacesOfSurvival();
            if (trueChance > difficultyData.maxChanceOfSurvival) trueChance = difficultyData.maxChanceOfSurvival;
            EndGameChoice.Instance.SetupChoice(difficultyData.malusScoreAtChoosedRevive, Mathf.RoundToInt(trueChance));
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

            UiViewer.Instance.PlayerJustDied(publicChoice < trueChance, publicChoice, bonusFromRez);

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
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Crowd_Cheer", false, 0.5f);
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Bell_Up", false, 1);
            DoResurrection(bonusFromRez);
            playerResedAlready = true;
        }
        else
        {
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Crowd_Boo", false, 0.2f);
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Bell_Down", false, 1);
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

        CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "GameOver_Sound", false, 1);
    }

    private void DoResurrection(float bonus)
    {

        playerCanShoot = true;
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
        return isArduinoMode ? IRCameraParser.Instance.funcPositionsCursorArduino() : Input.mousePosition;
    }

    public float GetCurrentChacesOfSurvival()
    {
        float initialPublic = PublicManager.Instance.GetInitialViewers();
        float currentPublic = PublicManager.Instance.GetNbViewers();
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
