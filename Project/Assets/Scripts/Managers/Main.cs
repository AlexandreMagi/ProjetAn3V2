using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    private bool playerCanOrb = true;
    private bool playerCanShoot = true;

    public static Main Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
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




        //DEBUG
        if (Input.GetKeyDown(KeyCode.N))
        {
            SequenceHandler.Instance.NextSequence();
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
