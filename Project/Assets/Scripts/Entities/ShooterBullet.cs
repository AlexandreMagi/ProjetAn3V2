using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterBullet : Entity<DataShooterBullet>, IGravityAffect, IBulletAffect, ISpecialEffects
{
    GameObject owner;
    float bulletSpeed = 0;
    float bulletRotationSpeed = 0;
    Vector3 randomCurve = Vector3.one;
    Vector3 posStart;
    Vector3 posAtLastFrame;
    Quaternion rotAtLastFrame;
    GameObject target = null;
    Transform transformPosEnd;
    Vector3 posEnd = Vector3.zero;
    GameObject dummyIndicator = null;
    [SerializeField]
    GameObject bulletMesh = null;

    float amplitudeMissile = 1;
    bool onGravity = false;
    bool isShot = false;
    float amplitudeShake = 0.3f;
    float shakeSpeedLerp = 5;

    float timerEActivateCollider = 0;
    bool canCollideWithOthersBullet = false;

    int team = 0;

    Rigidbody rbBody;

    bool hasExploded = false;
    GameObject circlePrefab;

    protected override void Start()
    {
    }

    public void OnCreation(GameObject _target, Vector3 EnnemiPos, float Amplitude, DataShooterBullet _bulletSettings, int _team, GameObject prop)
    {
        entityData = _bulletSettings as DataShooterBullet;
        health = entityData.startHealth;
        owner = prop;

        target = _target;
        transformPosEnd = _target.transform;
        posEnd = transformPosEnd.position;
        posStart = EnnemiPos;
        amplitudeMissile = Amplitude;
        bulletSpeed = entityData.bulletSpeed + Random.Range(-entityData.randomSpeedAdded, entityData.randomSpeedAdded);
        bulletRotationSpeed = entityData.rotationSpeed * Mathf.Sign(Random.Range(-1f, 1f));

        dummyIndicator = new GameObject();
        dummyIndicator.transform.position = posStart;
        dummyIndicator.transform.LookAt (transformPosEnd, Vector3.up);
        randomCurve = new Vector3(Random.Range(entityData.randomFrom.x, entityData.randomTo.x), Random.Range(entityData.randomFrom.y, entityData.randomTo.y), Random.Range(entityData.randomFrom.z, entityData.randomTo.z));
        if (entityData.randomRotationAtStart)
            dummyIndicator.transform.Rotate(0, 0, Random.Range(0,360));

        rbBody = GetComponent<Rigidbody>();

        team = _team;
        TeamsManager.Instance.RegistertoTeam(transform, team);

        circlePrefab = UiShooterCircle.Instance.CreateShooterCircle(_bulletSettings.circlePrefab);
        UiShooterCircle.Instance.MoveShooterCircle(circlePrefab, transform);
        circlePrefab.transform.localScale = Vector3.one * entityData.circleScale.Evaluate(0) * entityData.circleScaleMultiplier;
        circlePrefab.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.red, 0);

    }

    // Update is called once per frame
    void Update()
    {

        if (timerEActivateCollider < entityData.timeBeforeCollisionAreActived)
        {
            timerEActivateCollider += Time.deltaTime;
        } 
        else if (!canCollideWithOthersBullet)
        {
            canCollideWithOthersBullet = true;
        }

        if (dummyIndicator != null && !onGravity)
        {
            dummyIndicator.transform.Translate(0, 0, bulletSpeed * Time.deltaTime, Space.Self);
            float MaxDistance = Vector3.Distance(posEnd, posStart);
            float Curr = Vector3.Distance(posStart, dummyIndicator.transform.position);

            dummyIndicator.transform.Rotate(0, 0, Time.deltaTime * entityData.bulletRotation.Evaluate(Curr / MaxDistance) * entityData.rotationSpeed);
            transform.rotation = dummyIndicator.transform.rotation;
            transform.position = dummyIndicator.transform.position;
            bulletMesh.transform.position = Vector3.Lerp(bulletMesh.transform.position, dummyIndicator.transform.position + new Vector3(Random.Range(-amplitudeShake, amplitudeShake), Random.Range(-amplitudeShake, amplitudeShake), Random.Range(-amplitudeShake, amplitudeShake)), Time.deltaTime* shakeSpeedLerp);

            float ValueMax = entityData.bulletTrajectory.Evaluate(Curr / MaxDistance) * amplitudeMissile;
            transform.Translate(randomCurve * ValueMax, Space.Self);

            Vector3 relativePos = transform.position - posAtLastFrame;
            if (Time.timeScale > 0)
            {
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                transform.rotation = rotation;
                rotAtLastFrame = transform.rotation;
            }
            else transform.rotation = rotAtLastFrame;

            posAtLastFrame = transform.position;

            CameraHandler.Instance.AddShake(entityData.shakeIdle * Time.deltaTime, transform.position);

            if (Curr / MaxDistance >= 1)
            {
                //HitBullet();
                //KillBullet();
                TakeDamage(100);
            }

            if (target != null)
            {
                UiShooterCircle.Instance.MoveShooterCircle(circlePrefab, transform);

                //circlePrefab.transform.position = target.transform.position + Vector3.Normalize(transform.position - transformPosEnd.position) * 0.5f;
                //circlePrefab.transform.LookAt(target.transform, Vector3.up);
                circlePrefab.transform.localScale = Vector3.one * entityData.circleScale.Evaluate(Curr / MaxDistance) * entityData.circleScaleMultiplier;
                circlePrefab.GetComponent<Image>().color = Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance));
            }
            bulletMesh.GetComponent<MeshRenderer>().material.SetColor ("_BaseColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));
            bulletMesh.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));


            GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void HitBullet()
    {
        FxManager.Instance.PlayFx(entityData.fxExplosion, transform.position, Quaternion.identity, entityData.explosionRadius);

        Collider[] tHits = Physics.OverlapSphere(this.transform.position, entityData.explosionRadius);

        foreach (Collider hVictim in tHits)
        {
            if (hVictim.gameObject != this.gameObject && !(hVictim.gameObject == owner && !onGravity))
            {
                if(hVictim.gameObject == owner)
                {
                    owner.GetComponent<Shooter>().OnHitByOwnBullet();
                }

                IEntity entityVictim = hVictim.GetComponent<IEntity>();
                if(entityVictim != null && (hVictim.GetComponent<Player>() == null || !isShot))
                {
                    entityVictim.OnAttack(entityData.spriteToDisplayShield, entityData.spriteToDisplayLife);
                }
                
                ISpecialEffects speAffect = hVictim.GetComponent<ISpecialEffects>();
                if (speAffect != null && (hVictim.GetComponent<Player>() == null || !isShot))
                {
                    speAffect.OnExplosion(this.transform.position, entityData.explosionForce, entityData.explosionRadius, entityData.explosionDamage, entityData.explosionStun, entityData.explosionStunDuration, entityData.liftValue);
                    //hVictim.gameObject.GetComponent<C_BulletAffected>().OnBulletHit(bullet.BulletDammage, bullet.StunValue, bullet.BulletName);
                    //hVictim.gameObject.GetComponent<C_BulletAffected>().OnSoloHitPropulsion(transform.position, bullet.ForceAppliedOnImpact, bullet.BulletName);
                }

                //Vendetta preparation
                if (this.owner != null && hVictim.GetComponent<Player>() != null)
                {
                    PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.VendettaPrepare, transform.position, this.owner.GetComponent<Shooter>());
                }
            }
        }
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Explosion", false, 0.7f);

    }

    protected override void Die()
    {
        if (!hasExploded)
        {
            hasExploded = true;
            HitBullet();
            KillBullet();
            base.Die();
        }
    }

    public void KillBullet()
    {
        ShootTrigger triggerShoot = GetComponent<ShootTrigger>();

        if(triggerShoot != null)
        {
            triggerShoot.OnHit(null, Vector3.zero);
        }

        CameraHandler.Instance.AddShake(entityData.shakeAtImpact, transform.position);
        TimeScaleManager.Instance.AddStopTime(entityData.stopTimeAtImpact);


        Destroy(dummyIndicator);
        Destroy(circlePrefab);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //ISpecialEffects speAffect = collision.transform.GetComponent<ISpecialEffects>();
        //if (speAffect!=null || collision.gameObject.GetComponent<ShooterBullet>() && canCollideWithOthersBullet)
        //{

        //if (entityData.layerAffected == (entityData.layerAffected | (1 << collision.gameObject.layer)))
        //{
        if(!(collision.gameObject == owner && !onGravity))
        {
            //FxManager.Instance.PlayFx("VFX_ExplosionShooterBullet", transform.position, Quaternion.identity, entityData.explosionRadius);
            //speAffect.OnExplosion(bullet.bulletDammage, bullet.forceAppliedOnImpact, bullet.stunValue);
            TakeDamage(100);
            Debug.Log(collision.gameObject.name);
        }
           
            //HitBullet();
            //KillBullet();
        //}
        //}
    }

    #region Gravity

    public void OnGravityDirectHit()
    {
        onGravity = true;
        rbBody.useGravity = true;
        ReactGravity<DataSwarmer>.DoFreeze(rbBody);
        Destroy(circlePrefab);
    }

    public void OnPull(Vector3 position, float force)
    {
        onGravity = true;
        rbBody.useGravity = true;
        ReactGravity<DataEntity>.DoPull(rbBody, position, force, false);
        Destroy(circlePrefab);
    }

    public void OnRelease()
    {
        ReactGravity<DataSwarmer>.DoUnfreeze(rbBody);
    }

    public void OnHold()
    {
        onGravity = true;
        Destroy(circlePrefab);
        // Nothing
    }

    public void OnZeroG()
    {
        ReactGravity<DataSwarmer>.DoSpin(rbBody);
    }

    public void OnFloatingActivation(float GForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool independantFromTimeScale)
    {
        ReactGravity<DataSwarmer>.DoPull(rbBody, Vector3.up.normalized + this.transform.position, GForce, false);

        ReactGravity<DataSwarmer>.DoFloat(rbBody, timeBeforeActivation, isSlowedDownOnFloat, floatTime, independantFromTimeScale);
    }

    #endregion
    #region Bullet Affected

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
        isShot = true;
        TakeDamage(100);
        //HitBullet();
        //KillBullet();
    }

    public void OnHitShotGun()
    {
        // Nothing
    }

    public void OnHitSingleShot()
    {
        // Nothing
    }

    public void OnBulletClose()
    {
        // Nothing
    }

    public void OnCursorClose()
    {
        // Nothing
    }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        ReactSpecial<DataShooterBullet, DataShooterBullet>.DoProject(rbBody, explosionOrigin, explosionForce, explosionRadius, liftValue);
        ReactSpecial<DataShooterBullet, DataShooterBullet>.DoExplosionDammage(this, explosionOrigin, explosionDamage, explosionRadius);
    }

    #endregion

}
