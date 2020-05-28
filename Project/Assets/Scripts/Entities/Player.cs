using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity<DataPlayer>, ISpecialEffects
{
    bool godMode = false;
    float armor = 0;

    float armorToGain = 0;
    float rateOfArmorGained = 10;
  //  private DataPlayer playerData;

    public static Player Instance{get; private set;}

    public bool nextAttackDoesntTriggerBreakShield = false;

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
        return new Vector2(entityData.maxArmor, entityData.maxHealth);
    }

    void Awake()
    {
        Instance = this;
    }

    protected override void Die()
    {
        //Main.Instance.TriggerGameOverSequence();
        MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.Death);

        Main.Instance.FinalChoice();
    }
    
    public void DieForReal()
    {
        MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.Death);
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
        if (armorToGain > 0)
        {
            float armorGained = Time.unscaledDeltaTime * rateOfArmorGained;
            if (armorGained > armorToGain) armorGained = armorToGain;
            GainArmor(armorGained);
            armorToGain -= armorGained;
            if (armorToGain < 0) armorToGain = 0;
            UiLifeBar.Instance.UpdateArmor(armor);
            //Debug.Log("armorGained = " + armorGained + " / " + armor);
        }
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
        if (health > 0 && (UILeaderboard.Instance == null || !UILeaderboard.Instance.InLeaderboard))
        {
            //Metrics
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.DamageTaken, value);


            CustomSoundManager.Instance.PlaySound("PlayerDamage", "PlayerUnpitched",null, 1,false,1,0.2f);
            CameraHandler.Instance.AddShake(value / (entityData.armor + entityData.maxHealth) * entityData.damageShakeMultiplier * (armor > 0 ? entityData.damageScaleShieldMultiplier : entityData.damageScaleLifeMultiplier));
            TimeScaleManager.Instance.AddStopTime(entityData.stopTimeAtDammage);
            TimeScaleManager.Instance.AddSlowMo(entityData.slowmoForceOnDamage,entityData.slowmoDurationOnDamage);
            bool armorJustBroke = false;
            if (value >= armor)
            {
                if (!godMode)
                {
                    if (armor > 0)
                    {

                        CustomSoundManager.Instance.PlaySound("Se_HeartBeat", "UI", 1);
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
                        if (damageToHealth > 0)
                            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.DamageTakenOnHealth);
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
                        CustomSoundManager.Instance.PlaySound("Death_ExportLong", "EndGame", 1);

                    }

                
                }

                UiDamageHandler.Instance.PlayerTookDammage();
                UiLifeBar.Instance.PlayerTookDamage(armor, health);
                if (PostprocessManager.Instance != null )PostprocessManager.Instance.SetupDepthOfField();
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

        if(armor > entityData.maxArmor)
        {
            armor = entityData.maxArmor;
        }
        if (armor < 0)
            armor = 0;
        UiLifeBar.Instance.UpdateArmor(armor);


    }

    public void GainArmorOverTime(float delay, float value, float rate)
    {
        StartCoroutine(GainArmorOverTimeCoroutine(delay, value, rate));
    }

    IEnumerator GainArmorOverTimeCoroutine(float delay, float value, float rate)
    {
        if (delay != 0) yield return new WaitForSeconds(delay);
        armorToGain = value;
        rateOfArmorGained = rate;
        yield break;
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
