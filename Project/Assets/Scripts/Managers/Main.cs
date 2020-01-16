using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private bool playerCanOrb = true;
    private bool playerCanShoot = true;

    private string sequenceCheat = "";
    private bool sequenceSkipMode = false;

    public static Main Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        //SHOOT
        if (Input.GetKeyDown(KeyCode.Mouse1) && playerCanOrb)
        {
            Weapon.Instance.GravityOrbInput();
        }
        if (Input.GetKey(KeyCode.Mouse0) && playerCanShoot)
        {
            Weapon.Instance.InputHold();
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

        //DEBUG
        if (Input.GetKeyDown(KeyCode.N))
        {
            SequenceHandler.Instance.NextSequence();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SceneHandler.Instance.RestartScene(0);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            this.sequenceSkipMode = !this.sequenceSkipMode;
            Debug.Log($"Sequence skip : {sequenceSkipMode}");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Player.Instance.SetGod();
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
}
