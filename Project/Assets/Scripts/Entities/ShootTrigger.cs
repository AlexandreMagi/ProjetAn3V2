using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTrigger : Entity<DataEntity>, IBulletAffect
{
    ShootTriggerManager parentManager = null;

    [SerializeField]
    public bool isCollectible = false;

    bool isTriggered = false;

    [SerializeField]
    float armorGiven = 0;

    //[SerializeField]
    //bool keepsCombo = true;

    [SerializeField]
    string soundPlayed = "ShootTriggerSound";
    [SerializeField]
    float soundVolume = 1;

    Collider thisCollider = null;

    protected override void Start()
    {
        parentManager = this.transform.GetComponentInParent<ShootTriggerManager>();
        thisCollider = GetComponent<Collider>();
    }


    //Stimulus reactions
    #region StimulusBullet
    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray rayShot)
    {
        if (!isTriggered)
        {
            //MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);

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
                FxManager.Instance.PlayFx("VFX_CollectiblesShoot", this.transform.position, Quaternion.identity);
                GetComponentInChildren<ParticleSystem>().Stop();
                //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, soundPlayed, false, soundVolume);
                CustomSoundManager.Instance.PlaySound(soundPlayed, "Effect", soundVolume);
            }


            if (GetComponent<DeleteAfterJPOKillsParticles>() != null)
                GetComponent<DeleteAfterJPOKillsParticles>().StopParticles();

            if (armorGiven != 0)
                Player.Instance.GainArmor(armorGiven);

            if (isCollectible)
            {
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Collectible, transform.position);
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ArmorPadDestroyed);
            }
                

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
