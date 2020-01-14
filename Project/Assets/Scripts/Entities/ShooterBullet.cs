using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBullet : Entity<DataShooterBullet>, IGravityAffect, IBulletAffect, ISpecialEffects
{

    float bulletSpeed = 0;
    float bulletRotationSpeed = 0;
    Vector3 RandomCurve = Vector3.one;
    Vector3 vPosStart;
    Vector3 PosAtLastFrame;
    GameObject target = null;
    Transform vPosEnd;
    Vector3 posEnd = Vector3.zero;
    GameObject hDummyIndicator = null;
    [SerializeField]
    GameObject hMesh = null;
    [SerializeField]
    GameObject hCircle = null;

    float fAmplitudeMissile = 1;
    bool bOnGravity = false;
    float AmplitudeShake = 0.3f;
    float ShakeSpeedLerp = 5;

    float fTimerEActivateCollider = 0;
    bool bCanCollideWithOthersBullet = false;

    int team = 0;

    Rigidbody rbBody;

    bool hasExploded = false;

    protected override void Start()
    {

    }

    public void OnCreation(GameObject _target, Vector3 EnnemiPos, float Amplitude, DataShooterBullet _bulletSettings, int _team)
    {
        entityData = _bulletSettings as DataShooterBullet;
        health = entityData.startHealth;

        target = _target;
        vPosEnd = _target.transform;
        posEnd = vPosEnd.position;
        vPosStart = EnnemiPos;
        fAmplitudeMissile = Amplitude;
        bulletSpeed = entityData.bulletSpeed + Random.Range(-entityData.randomSpeedAdded, entityData.randomSpeedAdded);
        bulletRotationSpeed = entityData.rotationSpeed * Mathf.Sign(Random.Range(-1f, 1f));

        hDummyIndicator = new GameObject();
        hDummyIndicator.transform.position = vPosStart;
        hDummyIndicator.transform.LookAt (vPosEnd, Vector3.up);
        RandomCurve = new Vector3(Random.Range(entityData.randomFrom.x, entityData.randomTo.x), Random.Range(entityData.randomFrom.y, entityData.randomTo.y), Random.Range(entityData.randomFrom.z, entityData.randomTo.z));
        if (entityData.randomRotationAtStart)
            hDummyIndicator.transform.Rotate(0, 0, Random.Range(0,360));

        rbBody = GetComponent<Rigidbody>();

        team = _team;
        TeamsManager.Instance.RegistertoTeam(transform, team);
        hCircle = Instantiate(hCircle);

    }

    // Update is called once per frame
    void Update()
    {

        if (fTimerEActivateCollider < entityData.timeBeforeCollisionAreActived)
        {
            fTimerEActivateCollider += Time.deltaTime;
        } 
        else if (!bCanCollideWithOthersBullet)
        {
            bCanCollideWithOthersBullet = true;
        }

        if (hDummyIndicator != null && !bOnGravity)
        {
            hDummyIndicator.transform.Translate(0, 0, bulletSpeed * Time.deltaTime, Space.Self);
            float MaxDistance = Vector3.Distance(posEnd, vPosStart);
            float Curr = Vector3.Distance(vPosStart, hDummyIndicator.transform.position);

            hDummyIndicator.transform.Rotate(0, 0, Time.deltaTime * entityData.bulletRotation.Evaluate(Curr / MaxDistance) * entityData.rotationSpeed);
            transform.rotation = hDummyIndicator.transform.rotation;
            transform.position = hDummyIndicator.transform.position;
            hMesh.transform.position = Vector3.Lerp(hMesh.transform.position, hDummyIndicator.transform.position + new Vector3(Random.Range(-AmplitudeShake, AmplitudeShake), Random.Range(-AmplitudeShake, AmplitudeShake), Random.Range(-AmplitudeShake, AmplitudeShake)), Time.deltaTime* ShakeSpeedLerp);

            float ValueMax = entityData.bulletTrajectory.Evaluate(Curr / MaxDistance) * fAmplitudeMissile;
            transform.Translate(RandomCurve * ValueMax, Space.Self);

            Vector3 relativePos = transform.position - PosAtLastFrame;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = rotation;

            PosAtLastFrame = transform.position;

            CameraHandler.Instance.AddShake(entityData.shakeIdle * Time.deltaTime, transform.position);

            if (Curr / MaxDistance >= 1)
            {
                //HitBullet();
                //KillBullet();
                TakeDamage(100);
            }

            if (target != null)
            {
                hCircle.transform.position = target.transform.position + Vector3.Normalize(transform.position - vPosEnd.position) * 0.5f;
                hCircle.transform.localScale = Vector3.one * entityData.circleScale.Evaluate(Curr / MaxDistance) * entityData.circleScaleMultiplier;
                hCircle.transform.LookAt(target.transform, Vector3.up);
                hCircle.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));
            }
            hMesh.GetComponent<MeshRenderer>().material.SetColor ("_BaseColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));
            hMesh.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));


            GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void HitBullet()
    {
        //GameObject.FindObjectOfType<C_Fx>().ShooterBulletExplosion(this.transform.position, bullet.explosionRange * 1.3f);
        FxManager.Instance.PlayFx("VFX_ExplosionShooterBullet", transform.position, Quaternion.identity, entityData.explosionRadius);

        Collider[] tHits = Physics.OverlapSphere(this.transform.position, entityData.explosionRadius);

        foreach (Collider hVictim in tHits)
        {
            if (hVictim.gameObject != this.gameObject)
            {
                ISpecialEffects speAffect = hVictim.GetComponent<ISpecialEffects>();
                if (speAffect != null)
                {
                    speAffect.OnExplosion(this.transform.position, entityData.explosionForce, entityData.explosionRadius, entityData.explosionDamage, entityData.explosionStun, entityData.explosionStunDuration, entityData.liftValue);
                    //hVictim.gameObject.GetComponent<C_BulletAffected>().OnBulletHit(bullet.BulletDammage, bullet.StunValue, bullet.BulletName);
                    //hVictim.gameObject.GetComponent<C_BulletAffected>().OnSoloHitPropulsion(transform.position, bullet.ForceAppliedOnImpact, bullet.BulletName);
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
            triggerShoot.OnHit();
        }

        CameraHandler.Instance.AddShake(entityData.shakeAtImpact, transform.position);


        Destroy(hDummyIndicator);
        Destroy(hCircle);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //ISpecialEffects speAffect = collision.transform.GetComponent<ISpecialEffects>();
        //if (speAffect!=null || collision.gameObject.GetComponent<ShooterBullet>() && bCanCollideWithOthersBullet)
        //{

        //if (entityData.layerAffected == (entityData.layerAffected | (1 << collision.gameObject.layer)))
        //{
            FxManager.Instance.PlayFx("VFX_ExplosionShooterBullet", transform.position, Quaternion.identity, entityData.explosionRadius);
            //speAffect.OnExplosion(bullet.bulletDammage, bullet.forceAppliedOnImpact, bullet.stunValue);
            TakeDamage(100);
            //HitBullet();
            //KillBullet();
        //}
        //}
    }

    #region Gravity

    public void OnGravityDirectHit()
    {
        Debug.Log("ghghzg");
        bOnGravity = true;
        rbBody.useGravity = true;
        ReactGravity<DataSwarmer>.DoFreeze(rbBody);
        Destroy(hCircle);
    }

    public void OnPull(Vector3 position, float force)
    {
        bOnGravity = true;
        rbBody.useGravity = true;
        ReactGravity<DataEntity>.DoPull(rbBody, position, force, false);
        Destroy(hCircle);
    }

    public void OnRelease()
    {
        ReactGravity<DataSwarmer>.DoUnfreeze(rbBody);
    }

    public void OnHold()
    {
        // Nothing
    }

    public void OnZeroG()
    {
        ReactGravity<DataSwarmer>.DoSpin(rbBody);
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        ReactGravity<DataSwarmer>.DoPull(rbBody, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity<DataSwarmer>.DoFloat(rbBody, timeBeforeActivation, isSlowedDownOnFloat, tFloatTime, bIndependantFromTimeScale);
    }

    #endregion
    #region Bullet Affected

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
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
