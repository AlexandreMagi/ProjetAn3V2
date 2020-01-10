using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTrigger : Entity, IBulletAffect
{
    ShootTriggerManager parentManager = null;

    bool isTriggered = false;

    [SerializeField]
    bool keepsCombo = true;

    [SerializeField]
    string sSoundPlayed = "";
    [SerializeField]
    float fSoundVolume = 1;

    protected override void Start()
    {
        parentManager = this.transform.GetComponentInParent<ShootTriggerManager>();
    }


    //Stimulus reactions
    #region StimulusBullet
    public void OnHit(DataWeaponMod mod = null)
    {
        if (!isTriggered)
        {
            isTriggered = true;
            //if (keepsCombo) C_ComboManager.Instance.MaintainCombo();

            if (parentManager != null)
                parentManager.OnEventSent();

            GetComponent<MeshRenderer>().enabled = false;

            
            if (gameObject.transform.tag == "EnvironnementTrigger")
            {
                FxManager.Instance.PlayFx("VFX_EnvironnementTrigger", this.transform.position, Quaternion.identity);
            }
            else
            {
                FxManager.Instance.PlayFx("VFX_EnvironnementTrigger", this.transform.position, Quaternion.identity);
                CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, sSoundPlayed, false, fSoundVolume);
            }


            Destroy(this);
        }

    }

    public void OnHitShotGun()
    {
        
    }

    public void OnHitSingleShot()
    {
        
    }

    public void OnBulletClose()
    {
       
    }

    public void OnCursorClose()
    {
        
    }

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
