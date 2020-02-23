/////////////////////////////////////////////////////////////////
/// CODE FAIT PAR MAX //////////////
////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

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

    bool IsSoundPlayed = false;

    bool callNextSequence = false;

    [SerializeField]
    bool isHintTrigger = false;
    [SerializeField, ShowIf("isHintTrigger")]
    int baseShootCountBeforeFirstInt = 5;
    [SerializeField, ShowIf("isHintTrigger")]
    int baseShootCountBeforeSecondInt = 15;

    [SerializeField]
    Image firstUI;
    [SerializeField]
    Image secondUI;

    int baseShootCountIncrement;

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
        }

        if (isHintTrigger)
        {
            firstUI.enabled = false;
            secondUI.enabled = false;
        }

        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("MakeAction");

        if (isHintTrigger)
        {
            baseShootCountIncrement++;

            if (baseShootCountIncrement == baseShootCountBeforeFirstInt)
            {
                firstUI.enabled = true;
                Debug.Log("First hint");
            }
            else if (baseShootCountIncrement == baseShootCountBeforeSecondInt)
            {
                firstUI.enabled = false;
                secondUI.enabled = true;
                Debug.Log("Second hint");
            }
        }
    }

    public void OnBulletClose()
    {
        
    }
}
