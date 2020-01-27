﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Main : MonoBehaviour
{
    private bool playerCanOrb = true;
    private bool playerCanShoot = true;

    private string sequenceCheat = "";
    private bool sequenceSkipMode = false;

    [SerializeField]
    int startWithCameraNumber = 0;

    [SerializeField]
    DataDifficulty difficultyData;

    bool playerResedAlready = false;

    [SerializeField]
    bool autoReloadOnNoAmmo = false;

    bool hasJumpedCam = false;


    public static Main Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasJumpedCam && startWithCameraNumber != 0)
        {
            SequenceHandler.Instance.SkipToSequence(startWithCameraNumber);
            hasJumpedCam = true;
        }

        //SHOOT
        if (Input.GetKeyDown(KeyCode.Mouse1) && playerCanOrb)
        {
            Weapon.Instance.GravityOrbInput();
        }
        if (Input.GetKey(KeyCode.Mouse0) && playerCanShoot)
        {
            Weapon.Instance.InputHold();
        }
        if(Input.GetKeyDown(KeyCode.Mouse0) && Weapon.Instance.GetBulletAmmount().x == 0 && autoReloadOnNoAmmo)
        {
            Weapon.Instance.ReloadValidate();
            Weapon.Instance.ReloadingInput();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && playerCanShoot)
        {
            Weapon.Instance.InputUp(Input.mousePosition);
        }

        //CAM
        CameraHandler.Instance.DecalCurrentCamRotation(Input.mousePosition);

        //UI
        if (UiCrossHair.Instance != null)
        {
            UiCrossHair.Instance.UpdateCrossHair(Input.mousePosition);
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
        if (Input.GetKeyDown(KeyCode.N))
        {
            SequenceHandler.Instance.NextSequence();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            SceneHandler.Instance.RestartScene(0);
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
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.BonusOnRespawn, null, 50 );
        }

        if(sequenceSkipMode)
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

        if (Input.GetKeyDown(KeyCode.E))
        {
        }



    }

    public void SetControlState(TriggerSender.Activable control, bool state)
    {

        if (control == TriggerSender.Activable.BaseWeapon || control == TriggerSender.Activable.Both)
        {
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
        if (difficultyData.playerCanReraise || !playerResedAlready)
        {
            //Le joueur est mort. Sa survie dépendra du ratio du public.
            float initialPublic = PublicManager.Instance.GetInitialViewers();
            float currentPublic = PublicManager.Instance.GetNbViewers();
            float growthValue = PublicManager.Instance.GetGrowthValue();

            // Debug.Log($"{initialPublic} {currentPublic} {growthValue}");

            float chancesOfSurvival = (currentPublic / (initialPublic + growthValue * (float)difficultyData.difficulty) / (float)difficultyData.difficulty);
            float trueChance = chancesOfSurvival * difficultyData.maxChanceOfSurvival;
            float bonusFromRez = 0;

           

            if (trueChance > difficultyData.maxChanceOfSurvival)
            {
                bonusFromRez = trueChance - difficultyData.maxChanceOfSurvival;

                trueChance = difficultyData.maxChanceOfSurvival;
            }

            int publicChoice = Random.Range(0, 101);

            Debug.Log($"Required : {trueChance} -- Chance : {publicChoice}");

            if (publicChoice < trueChance)
            {
                DoResurrection(bonusFromRez);
                playerResedAlready = true;
            }
            else
            {
                DoGameOver();
            }
        }
        
    }

    private void DoGameOver()
    {
        Debug.Log("Public chose... DEATH");
    }

    private void DoResurrection(float bonus)
    {
        Debug.Log("Public chose... LIFE");

        Player.Instance.SetLifeTo(1);
        Player.Instance.GainArmor(difficultyData.armorOnRaise + bonus * difficultyData.armorOnRaiseBonus / (int)difficultyData.difficulty);

        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DeathAndRespawn);

        if(bonus > 0)
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.BonusOnRespawn, null, bonus);
        }
        
    }

   
}
