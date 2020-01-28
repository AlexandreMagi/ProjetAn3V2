using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity<DataPlayer>, ISpecialEffects
{
    bool godMode = false;
    float armor = 0;
  //  private DataPlayer playerData;

    public static Player Instance{get; private set;}

    public void SetGod()
    {
        this.godMode = !this.godMode;
    }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        ReactSpecial<DataPlayer, DataSwarmer>.DoExplosionDammage(this, explosionOrigin, explosionDamage, explosionRadius);
    }

    public Vector2 GetBaseValues()
    {
        return new Vector2(entityData.armor, entityData.maxHealth);
    }

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        entityData = entityData as DataPlayer;
        armor = entityData.armor;
        Debug.Log("Must update Life");
        //UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
        //UiLifeBar.Instance.UpdateLifeDisplay(health / entityData.maxHealth, health);
    }

    protected virtual void Update()
    {
    }

    public override void OnAttack(DataUiTemporarySprite dataSpriteShield, DataUiTemporarySprite dataSpriteLife)
    {
        UiDamageHandler.Instance.AddSprite(dataSpriteShield, dataSpriteLife);
    }

    public override void TakeDamage(float value)
    {
        CameraHandler.Instance.AddShake(value / (entityData.armor + entityData.maxHealth) * entityData.damageShakeMultiplier * (armor > 0 ? entityData.damageScaleShieldMultiplier : entityData.damageScaleLifeMultiplier));
        TimeScaleManager.Instance.AddStopTime(entityData.stopTimeAtDammage);
        if (value >= armor)
        {
            if (!godMode)
            {
                if (armor > 0)
                {
                    UiDamageHandler.Instance.ShieldBreak();
                    CameraHandler.Instance.AddShake(entityData.shakeAtArmorDestruction);
                    GameObject renderingCam = CameraHandler.Instance.RenderingCam;
                    FxManager.Instance.PlayFx(entityData.shakeAtArmorFx, renderingCam.transform.position, renderingCam.transform.rotation);
                    TimeScaleManager.Instance.AddStopTime(entityData.stopTimeAtShieldBreak);
                }
                value -= armor;
                armor = 0;
            }
           
        }
        else
        {
            if (!godMode)
            {
                armor -= value;
                value = 0;
            }
        }
        if (!godMode)
        {
            Debug.Log("Must update Life");
            //UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
            //UiLifeBar.Instance.UpdateLifeDisplay((health - value) / entityData.maxHealth, health - value);

            if(armor < 0)
            {
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DamageOnLifeBar);

                if(health <= 20)
                {
                    PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.SuperLowHp);
                }
                else if(health <= 50)
                {
                    PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.LowHp);
                }
            }

            if(health < 0)
            {
                Main.Instance.TriggerGameOverSequence();
            }

            base.TakeDamage(value);
            UiLifeBar.Instance.PlayerTookDamage(armor, health);
        }
        
    }

    public float getArmor()
    {
        return armor;
    }

    public void GainArmor(float value)
    {
        armor += value;
        //UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
    }
}
