/////////////////////////////////////////////////////////////////
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

        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("MakeAction");
    }

    public void OnBulletClose()
    {
        
    }
}
