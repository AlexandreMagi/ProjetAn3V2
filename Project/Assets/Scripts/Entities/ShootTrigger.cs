using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTrigger : Entity<DataEntity>, IBulletAffect
{
    ShootTriggerManager parentManager = null;

    [SerializeField]
    bool isCollectible = false;

    bool isTriggered = false;

    //[SerializeField]
    //bool keepsCombo = true;

    //[SerializeField]
    //string soundPlayed = "";
    //[SerializeField]
    //float soundVolume = 1;

    Collider thisCollider = null;

    protected override void Start()
    {
        parentManager = this.transform.GetComponentInParent<ShootTriggerManager>();
        thisCollider = GetComponent<Collider>();
    }


    //Stimulus reactions
    #region StimulusBullet
    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
        if (!isTriggered)
        {
            isTriggered = true;
            //if (keepsCombo) C_ComboManager.Instance.MaintainCombo();

            if (parentManager != null)
                parentManager.OnEventSent();

            GetComponent<MeshRenderer>().enabled = false;
            thisCollider.enabled = false;

            
            if (gameObject.transform.tag == "EnvironnementTrigger")
            {
                FxManager.Instance.PlayFx("VFX_EnvironnementTrigger", this.transform.position, Quaternion.identity);
            }
            else
            {
                FxManager.Instance.PlayFx("VFX_EnvironnementTrigger", this.transform.position, Quaternion.identity);
                //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, soundPlayed, false, soundVolume);
            }


            if (GetComponent<DeleteAfterJPOKillsParticles>() != null)
                GetComponent<DeleteAfterJPOKillsParticles>().StopParticles();

            if (isCollectible)
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Collectible, transform.position);

            Destroy(this);
        }

    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        
    }

    public void OnBulletClose()
    {
       
    }

    public void OnCursorClose()
    {
        
    }

    #endregion
}
