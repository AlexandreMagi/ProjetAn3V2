﻿/////////////////////////////////////////////////////////////////
/// CODE FAIT PAR MAX //////////////
////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunTriggerShoot : MonoBehaviour, IBulletAffect
{

    [SerializeField]
    string soundPlayed = "";
    [SerializeField]
    float soundVolume = 1;
    [SerializeField]
    float delay = 0;

    [SerializeField]
    float forceApplied = 200;
    [SerializeField]
    float timerBeforeNextSequence = 0.5f;


    [SerializeField]
    int nbShootBeforeFirstHint = 3;
    [SerializeField]
    int nbShootBeforeSecondHint = 10;
    int nbShoot = 0;
    [SerializeField]
    float timeBeforeSecondHint = 10;
    bool timerStarted = false;
    bool secondHintPlayed = false;
    float timerBeforeSecondHint = 0;


    bool IsSoundPlayed = false;

    bool callNextSequence = false;

    void PlaySound()
    {
        if (soundPlayed != "")
        {
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, soundPlayed, false, soundVolume);
        }
    }

    void resetNextSequence()
    {
        callNextSequence = false;
    }

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
        //Debug.Log(mod);
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
        Destroy(GetComponent<Animator>());

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;

        rb.AddForce(new Vector3(forceApplied, 0, 0));
        HintScript.Instance.Depop();

        if (!IsSoundPlayed)
        {
            IsSoundPlayed = true;
            Invoke("PlaySound", delay);
        }

        if (!callNextSequence)
        {
            TriggerUtil.TriggerSequence(timerBeforeNextSequence);
            Invoke("resetNextSequence", 2f);
            callNextSequence = true;
            Destroy(this);
        }

        Weapon.Instance.OnShotGunHitTarget();
    }

    void Update()
    {
        if (timerStarted)
        {
            if (timerBeforeSecondHint > 0)
               timerBeforeSecondHint -= Time.unscaledDeltaTime;
            if (timerBeforeSecondHint < 0)
            {
                timerBeforeSecondHint = 0;
                if (!secondHintPlayed)
                {
                    secondHintPlayed = true;
                    DisplaySecondHint();
                }
            } 
        }
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("MakeAction");
        nbShoot++;
        if (nbShoot == 1)
        {
            timerBeforeSecondHint = timeBeforeSecondHint;
            timerStarted = true;
        }

        if (nbShoot == nbShootBeforeFirstHint)
            DisplayFirstHint();
        else if (nbShoot == nbShootBeforeSecondHint && !secondHintPlayed)
        {
            DisplaySecondHint();
            secondHintPlayed = true;
        }
    }

    void DisplayFirstHint()
    {
        HintScript.Instance.PopHint("Ça n'a pas l'air de marcher.", 4);
    }
    
    void DisplaySecondHint()
    {
        HintScript.Instance.Depop();
        Invoke("TrueDisplay", 3);
    }
    void TrueDisplay()
    {
        HintScript.Instance.ChangeFontSize(23);
        HintScript.Instance.PopHint("Maintiens la gachette appuyée pour charger ton tir !");
    }

    public void OnBulletClose()
    {
        
    }
}
