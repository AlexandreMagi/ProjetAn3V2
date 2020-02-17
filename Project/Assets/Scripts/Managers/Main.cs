﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Main : MonoBehaviour
{
    private bool playerCanOrb = true;
    private bool playerCanShoot = true;

    public bool PlayerCanOrb {get { return playerCanOrb; } }

    private string sequenceCheat = "";
    private bool sequenceSkipMode = false;

    private float timeLeftForRaycastCursor;
    private float timeTickCursor = .2f;

    Transmition arduinoTransmettor;

    [SerializeField]
    int startWithCameraNumber = 0;

    [SerializeField]
    DataDifficulty difficultyData;

    bool playerResedAlready = false;

    [SerializeField]
    bool autoReloadOnNoAmmo = false;

    bool hasJumpedCam = false;

    [HideInInspector]
    public bool GameEnded = false;

    [SerializeField]
    bool isArduinoMode = false;

    public static Main Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(arduinoTransmettor == null)
        {
            arduinoTransmettor = Transmition.Instance;
        }

        if (!hasJumpedCam && startWithCameraNumber != 0)
        {
            hasJumpedCam = true;
            SequenceHandler.Instance.SkipToSequence(startWithCameraNumber);
        }

        //SHOOT
        if (Input.GetKeyDown(KeyCode.Mouse1) && playerCanOrb)
        {
            Weapon.Instance.GravityOrbInput();
        }
        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotHeld) : Input.GetKey(KeyCode.Mouse0)) && playerCanShoot)
        {
            Weapon.Instance.InputHold();
        }
        if((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotDown) : Input.GetKeyUp(KeyCode.Mouse0)) && Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)
        {
            Weapon.Instance.ReloadValidate();
            Weapon.Instance.ReloadingInput();
        }
        if ((isArduinoMode ? (arduinoTransmettor && arduinoTransmettor.isShotUp) : Input.GetKeyUp(KeyCode.Mouse0)) && playerCanShoot)
        {
            Weapon.Instance.InputUp(isArduinoMode ? Transmition.Instance.positions() : Input.mousePosition);
        }


        //CAM
        Vector3 posCursor = isArduinoMode ? Transmition.Instance.positions() : Input.mousePosition;
        if ( (posCursor.x < Transmition.Instance.iResolutionX && posCursor.x > 0 && posCursor.y < Transmition.Instance.iResolutionY && posCursor.y > 0)
            || (posCursor.x < Screen.width && posCursor.x > 0 && posCursor.y < Screen.height && posCursor.y > 0))
            CameraHandler.Instance.DecalCamWithCursor(posCursor);
        

        //UI
        if (UiCrossHair.Instance != null)
        {

            UiCrossHair.Instance.UpdateCrossHair(isArduinoMode ? Transmition.Instance.positions() : Input.mousePosition);
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

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneHandler.Instance.RestartScene(.3f, true);
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            Weapon.Instance.ReloadValidate();
            Weapon.Instance.ReloadingInput();
        }

        if(timeLeftForRaycastCursor <= timeTickCursor)
        {
            Ray cursorRay = CameraHandler.Instance.renderingCam.ScreenPointToRay(isArduinoMode ? Transmition.Instance.positions() : Input.mousePosition);
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
            playerCanOrb = state;
            //FindObjectOfType<C_Ui>().CannotShoot(state);
        }
    }

    public void TriggerGameOverSequence()
    {
        playerCanShoot = false;
        playerCanOrb = false;

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
        
    }

    public void EndReviveSituation(bool rez, float bonusFromRez)
    {

        if (rez)
        {
            DoResurrection(bonusFromRez);
            playerResedAlready = true;
        }
        else
        {
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
    }

    private void DoResurrection(float bonus)
    {

        playerCanShoot = true;
        playerCanOrb = true;

        Player.Instance.SetLifeTo(Mathf.RoundToInt(Player.Instance.GetBaseValues().y / 5));
        Player.Instance.GainArmor(difficultyData.armorOnRaise + bonus * difficultyData.armorOnRaiseBonus / (int)difficultyData.difficulty);
        Player.Instance.Revive();

        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DeathAndRespawn, Vector3.zero);

        if(bonus > 0)
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.BonusOnRespawn, Vector3.zero, null, bonus);
        }
        
    }

    public float GetCurrentChacesOfSurvival()
    {
        float initialPublic = PublicManager.Instance.GetInitialViewers();
        float currentPublic = PublicManager.Instance.GetNbViewers();
        float growthValue = PublicManager.Instance.GetGrowthValue();

        // Debug.Log($"{initialPublic} {currentPublic} {growthValue}");

        return (currentPublic / (initialPublic + growthValue * (float)difficultyData.difficulty) / (float)difficultyData.difficulty) * difficultyData.maxChanceOfSurvival; ;
    }
}
