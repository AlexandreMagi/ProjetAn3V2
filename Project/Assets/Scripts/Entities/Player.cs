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

    public void SetLifeTo(int life)
    {
        health = life;
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

    protected override void Die()
    {
        Main.Instance.TriggerGameOverSequence();
    }
    
    public void DieForReal()
    {
        Destroy(this);
    }

    protected override void Start()
    {
        base.Start();
        entityData = entityData as DataPlayer;
        armor = entityData.armor;
        //Debug.Log("Must update Life");
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
        if (health > 0)
        {
            CameraHandler.Instance.AddShake(value / (entityData.armor + entityData.maxHealth) * entityData.damageShakeMultiplier * (armor > 0 ? entityData.damageScaleShieldMultiplier : entityData.damageScaleLifeMultiplier));
            TimeScaleManager.Instance.AddStopTime(entityData.stopTimeAtDammage);
            bool armorJustBroke = false;
            if (value >= armor)
            {
                if (!godMode)
                {
                    if (armor > 0)
                    {
                        UiDamageHandler.Instance.ShieldBreak();
                        CameraHandler.Instance.AddShake(entityData.shakeAtArmorDestruction);
                        GameObject renderingCam = CameraHandler.Instance.renderingCam.gameObject;
                        FxManager.Instance.PlayFx(entityData.shakeAtArmorFx, renderingCam.transform.position, renderingCam.transform.rotation);
                        TimeScaleManager.Instance.AddStopTime(entityData.stopTimeAtShieldBreak);
                        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DamageOnArmor, transform.position);

                        value -= armor;
                        armor = 0;
                        armorJustBroke = true;
                    }

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
                //Debug.Log("Must update Life");
                //UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
                //UiLifeBar.Instance.UpdateLifeDisplay((health - value) / entityData.maxHealth, health - value);

                if(armor <= 0)
                {
                    //Debug.Log("no armor");

                    float damageToHealth = 0;
                    if (value > 0 && !armorJustBroke)
                    {
                        damageToHealth = Mathf.Floor(entityData.startHealth / 5);
                        health -= damageToHealth;
                        //Debug.Log("hp damage");
                    
                    }

                    //UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
                    //UiLifeBar.Instance.UpdateLifeDisplay((health - damageToHealth) / entityData.maxHealth, health - damageToHealth);

                    if (armor < 0)
                    {
                        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DamageOnLifeBar, transform.position);

                        if (health <= 20)
                        {
                            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.SuperLowHp, transform.position);
                        }
                        else if (health <= 50)
                        {
                            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.LowHp, transform.position);
                        }
                    }

                    //Si hp < 5%
                    if (health <= entityData.startHealth / 20)
                    {
                        this.Die();

                    }

                
                }

                UiLifeBar.Instance.PlayerTookDamage(armor, health);
            }
        }
    }

    public float getArmor()
    {
        return armor;
    }

    public void GainArmor(float value)
    {
        armor += value;
        UiLifeBar.Instance.AddArmor(value);

        if(armor > 300)
        {
            armor = 300;
        }
        //UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);

    }
}
