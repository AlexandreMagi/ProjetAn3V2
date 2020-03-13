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
        UiLifeBar.Instance.UpdateCapsules(health);
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
        //Main.Instance.TriggerGameOverSequence();
        Main.Instance.FinalChoice();
    }
    
    public void DieForReal()
    {
        gameObject.SetActive(false);
        //Destroy(this);
    }

    protected override void Start()
    {
        base.Start();
        entityData = entityData as DataPlayer;
        armor = entityData.armor;
        TeamsManager.Instance.RegistertoTeam(transform, entityData.team);
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

    public Vector2 GetLifeValues()
    {
        return new Vector2(health, entityData.maxHealth);
    }

    public override void TakeDamage(float value)
    {
        if (health > 0)
        {
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "PlayerDamage", false, 1, 0.2f);
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
                        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DamageOnArmor, Vector3.zero);

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
                        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DamageOnLifeBar, Vector3.zero);

                        if (health <= 20)
                        {
                            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.SuperLowHp, Vector3.zero);
                        }
                        else if (health <= 50)
                        {
                            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.LowHp, Vector3.zero);
                        }
                    }

                    //Si hp < 5%
                    if (health <= entityData.startHealth / 20)
                    {
                        this.Die();
                        CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Death_ExportLong", false, 1);

                    }

                
                }

                UiDamageHandler.Instance.PlayerTookDammage();
                UiLifeBar.Instance.PlayerTookDamage(armor, health);
                PostprocessManager.Instance.SetupDepthOfField();
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
        //UiLifeBar.Instance.AddArmor(value);
        //UiLifeBar.Instance.UpdateArmor(armor);

        if(armor > entityData.armor)
        {
            armor = entityData.armor;
        }
        if (armor < 0)
            armor = 0;
        UiLifeBar.Instance.UpdateArmor(armor);


    }

    public void Revive()
    {
        Collider[] tHits = Physics.OverlapSphere(this.transform.position, entityData.respawnExplosionRadius);

        TimeScaleManager.Instance.AddSlowMo(0.8f, 5);

        foreach (Collider hVictim in tHits)
        {
            if (hVictim.gameObject != this.gameObject)
            {
                IEntity entityVictim = hVictim.GetComponent<IEntity>();
                if (entityVictim != null && (hVictim.GetComponent<Player>() == null))
                {
                    entityVictim.OnAttack(entityData.spriteToDisplayShield, entityData.spriteToDisplayLife);
                }

                ISpecialEffects speAffect = hVictim.GetComponent<ISpecialEffects>();
                if (speAffect != null && (hVictim.GetComponent<Player>() == null))
                {
                    speAffect.OnExplosion(this.transform.position, entityData.respawnExplosionForce, entityData.respawnExplosionRadius, entityData.respawnExplosionDamage, entityData.respawnExplosionStun, entityData.respawnExplosionStunDuration, entityData.respawnExplosionLiftValue);
                    //hVictim.gameObject.GetComponent<C_BulletAffected>().OnBulletHit(bullet.BulletDammage, bullet.StunValue, bullet.BulletName);
                    //hVictim.gameObject.GetComponent<C_BulletAffected>().OnSoloHitPropulsion(transform.position, bullet.ForceAppliedOnImpact, bullet.BulletName);
                }
            }
        }
        FxManager.Instance.PlayFx(entityData.respawnExplosionFx, transform.position, transform.rotation, entityData.respawnExplosionRadius);
        CameraHandler.Instance.RemoveShake();
    }

    public void DieForGood()
    {

    }
}
