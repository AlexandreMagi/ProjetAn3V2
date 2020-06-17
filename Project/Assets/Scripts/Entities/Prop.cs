﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Entity<DataProp>, IGravityAffect, IBulletAffect, ISpecialEffects
{
    bool isAirbone = false;
    float timePropel = .5f;
    float elapsedTime = 0;

    bool canDieVFX = true;

    Rigidbody rb;

    [HideInInspector] public bool isAffectedByGravity = false;

    [SerializeField] string soundToPlayOnImpact = "";
    [SerializeField] float soundVolume = 1;
    [SerializeField] float soundRandomPitch = 0.2f;

    [SerializeField] string soundToPlayWhenDie = "";
    [SerializeField] float soundVolumeWhenDie = 1;
    [SerializeField] float soundRandomPitchWhenDie = 0.2f;

    [SerializeField] float minDistanceToPlayStepSound = 10;

    float timeRemainginBeforeCanPlayImpactSound = 5;

  //  DataProp propData;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        entityData = entityData as DataProp;
    }

    protected override void Die()
    {
        if (canDieVFX)
        {
            canDieVFX = false;
            InstantiateExplosion();
            if (soundToPlayWhenDie != "" && (CameraHandler.Instance == null ||CameraHandler.Instance.GetDistanceWithCam(transform.position) < minDistanceToPlayStepSound))
            {
                AudioSource deathAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayWhenDie, "Effect", null, soundVolumeWhenDie, false, 2, soundRandomPitchWhenDie, 0,3);
                if (deathAudioSource != null)
                {
                    deathAudioSource.spatialBlend = 1;
                    deathAudioSource.minDistance = 8;
                    deathAudioSource.transform.position = transform.position;
                }
            }
            //if (GetComponent<DeathBodyPart>() != null)
            //    Weapon.Instance.JustDestroyedBodyPart(transform.position);
        }
        DeathBodyPart bodyPart = transform.GetComponent<DeathBodyPart>();
        if (bodyPart == null)
            base.Die();
        else
        {
            TeamsManager.Instance.RemoveFromTeam(this.transform, this.entityData.team);
            CameraHandler.Instance.AddShake(entityData.shakeOnDie, entityData.shakeOnDieTime);
            bodyPart.Depop();
        }
    }

    #region Gravity
    public void OnGravityDirectHit()
    {
        ReactGravity<DataProp>.DoFreeze(rb);
        isAffectedByGravity = true;
    }

    public void OnZeroGRelease()
    {
        //Nothing happens on hold
    }

    public void OnHold()
    {
        //Nothing happens on hold
    }

    public void OnPull(Vector3 origin, float force, bool isReppelForce = false, Vector3? normalReppel = null)
    {
        ReactGravity<DataProp>.DoPull(rb, origin, force, isAirbone, isReppelForce, normalReppel);
        isAffectedByGravity = true;
    }


    void InstantiateExplosion()
    {
        //if (entityData.fracturedProp != null)
        //{
        //    GameObject fract;
        //    fract = Instantiate(entityData.fracturedProp, transform);
        //    fract.transform.parent = null;

        //    Rigidbody[] rb = fract.GetComponentsInChildren<Rigidbody>();
        //    foreach (Rigidbody rbs in rb)
        //    {
        //        rbs.AddExplosionForce(entityData.fracturedForceOnDie * 10, rbs.transform.position, 10);
        //    }
        //}
        if (DeadBodyPartManager.Instance != null) DeadBodyPartManager.Instance.RequestPop(entityData.fractureType, transform.position, transform.up * .5f);
        FxManager.Instance.PlayFx(entityData.fxPlayedOnDestroy, transform.position, Quaternion.identity);
    }

    public void OnRelease()
    {
        ReactGravity<DataProp>.DoUnfreeze(rb);
    }

    public void OnZeroG()
    {
        //ReactGravity.DoSpin(this);
    }

    public void SetTimerToRelease(float timeSent) { Invoke("CompleteRelease", timeSent + 2.5f ); }
    void CompleteRelease() { isAffectedByGravity = false; }


    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        //Die();
        ReactGravity<DataProp>.DoPull(rb, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity<DataProp>.DoFloat(rb, timeBeforeActivation, isSlowedDownOnFloat, floatTime, bIndependantFromTimeScale);

    }
    #endregion

    #region Bullet
    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray rayShot)
    {
        TakeDamage(mod.bullet.damage);
        ReactBullet.PushFromHit(this.GetComponent<Rigidbody>(), position, 2400, 5);
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
        
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        if (!Weapon.Instance.CheckIfModIsMinigun(mod))
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);
    }

    public void OnBulletClose()
    {
       
    }

    public void OnCursorClose()
    {
       
    }
    #endregion

    protected virtual void FixedUpdate()
    {
        //Check for airbone and makes it spin if in the air
        if (isAirbone)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timePropel)
            {
                //ReactGravity.DoSpin(this);

                //Check si touche le sol
                elapsedTime = 0;
                if (Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 1f))
                {
                    isAirbone = false;
                }
            }
        }

        if (timeRemainginBeforeCanPlayImpactSound >= 0) timeRemainginBeforeCanPlayImpactSound -= Time.deltaTime;
    }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0, bool damageCamera = true)
    {
        ReactSpecial<DataProp, DataSwarmer>.DoProject(rb, explosionOrigin, explosionForce, explosionRadius, liftValue);
        ReactSpecial<DataProp, DataSwarmer>.DoExplosionDammage(this, explosionOrigin, explosionDamage, explosionRadius);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2 && soundToPlayOnImpact != "" && timeRemainginBeforeCanPlayImpactSound < 0 && (CameraHandler.Instance == null || CameraHandler.Instance.GetDistanceWithCam(transform.position) < minDistanceToPlayStepSound))
        {
            AudioSource collisionAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayOnImpact, "Effect", null, soundVolume, false, 0.3f, soundRandomPitch,0,12);
            if (collisionAudioSource != null)
            {
                collisionAudioSource.spatialBlend = 1;
                collisionAudioSource.minDistance = 8;
                collisionAudioSource.transform.position = transform.position;
            }
                
        }
    }


}
