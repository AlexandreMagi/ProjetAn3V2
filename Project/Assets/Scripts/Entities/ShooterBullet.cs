using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBullet : MonoBehaviour, IGravityAffect
{

    DataShooterBullet bullet = null;
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

    public void OnCreation(GameObject _target, Vector3 EnnemiPos, float Amplitude, DataShooterBullet bulletSettings)
    {
        target = _target;
        vPosEnd = _target.transform;
        posEnd = vPosEnd.position;
        vPosStart = EnnemiPos;
        fAmplitudeMissile = Amplitude;
        bullet = bulletSettings;
        bulletSpeed = bullet.bulletSpeed + Random.Range(-bullet.randomSpeedAdded, bullet.randomSpeedAdded);
        bulletRotationSpeed = bullet.rotationSpeed * Mathf.Sign(Random.Range(-1f, 1f));

        hDummyIndicator = new GameObject();
        hDummyIndicator.transform.position = vPosStart;
        hDummyIndicator.transform.LookAt (vPosEnd, Vector3.up);
        RandomCurve = new Vector3(Random.Range(bulletSettings.randomFrom.x, bulletSettings.randomTo.x), Random.Range(bulletSettings.randomFrom.y, bulletSettings.randomTo.y), Random.Range(bulletSettings.randomFrom.z, bulletSettings.randomTo.z));
        if (bulletSettings.randomRotationAtStart)
            hDummyIndicator.transform.Rotate(0, 0, Random.Range(0,360));


        hCircle = Instantiate(hCircle);

    }

    // Update is called once per frame
    void Update()
    {

        if (fTimerEActivateCollider < bullet.timeBeforeCollisionAreActived)
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

            hDummyIndicator.transform.Rotate(0, 0, Time.deltaTime * bullet.bulletRotation.Evaluate(Curr / MaxDistance) * bullet.rotationSpeed);
            transform.rotation = hDummyIndicator.transform.rotation;
            transform.position = hDummyIndicator.transform.position;
            hMesh.transform.position = Vector3.Lerp(hMesh.transform.position, hDummyIndicator.transform.position + new Vector3(Random.Range(-AmplitudeShake, AmplitudeShake), Random.Range(-AmplitudeShake, AmplitudeShake), Random.Range(-AmplitudeShake, AmplitudeShake)), Time.deltaTime* ShakeSpeedLerp);

            float ValueMax = bullet.bulletTrajectory.Evaluate(Curr / MaxDistance) * fAmplitudeMissile;
            transform.Translate(RandomCurve * ValueMax, Space.Self);

            Vector3 relativePos = transform.position - PosAtLastFrame;
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            transform.rotation = rotation;

            PosAtLastFrame = transform.position;

            //GameObject.FindObjectOfType<C_Camera>().AddShake(5 * Time.deltaTime);

            if (Curr / MaxDistance >= 1)
            {
                HitBullet();
                KillBullet();
            }

            if (target != null)
            {
                hCircle.transform.position = target.transform.position + Vector3.Normalize(transform.position - vPosEnd.position) * 0.5f;
                hCircle.transform.localScale = Vector3.one * bullet.circleScale.Evaluate(Curr / MaxDistance) * bullet.circleScaleMultiplier;
                hCircle.transform.LookAt(target.transform, Vector3.up);
                hCircle.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));
            }
            hMesh.GetComponent<MeshRenderer>().material.SetColor ("_BaseColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));
            hMesh.GetComponent<MeshRenderer>().material.SetColor ("_EmissionColor", Color.Lerp(Color.yellow, Color.red, (Curr / MaxDistance)));

        }
    }

    public void HitBullet()
    {
        //GameObject.FindObjectOfType<C_Fx>().ShooterBulletExplosion(this.transform.position, bullet.explosionRange * 1.3f);

        FxManager.Instance.PlayFx("VFX_ExplosionShooterBullet", transform.position, Quaternion.identity, bullet.explosionRadius);

        Collider[] tHits = Physics.OverlapSphere(this.transform.position, bullet.explosionRadius);

        foreach (Collider hVictim in tHits)
        {
            ISpecialEffects speAffect = hVictim.GetComponent<ISpecialEffects>();
            if (speAffect != null)
            {
                speAffect.OnExplosion(this.transform.position, bullet.explosionForce, bullet.explosionRadius, bullet.explosionDamage, bullet.explosionStun, bullet.explosionStunDuration, bullet.liftValue);
                //hVictim.gameObject.GetComponent<C_BulletAffected>().OnBulletHit(bullet.BulletDammage, bullet.StunValue, bullet.BulletName);
                //hVictim.gameObject.GetComponent<C_BulletAffected>().OnSoloHitPropulsion(transform.position, bullet.ForceAppliedOnImpact, bullet.BulletName);
            }
        }
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Explosion", false, 0.7f);

    }

    public void KillBullet()
    {
        ShootTrigger triggerShoot = GetComponent<ShootTrigger>();

        if(triggerShoot != null)
        {
            triggerShoot.OnHit();
        }

        Destroy(hDummyIndicator);
        Destroy(hCircle);
        Destroy(this.gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        ISpecialEffects speAffect = collision.transform.GetComponent<ISpecialEffects>();
        if (speAffect!=null || collision.gameObject.GetComponent<ShooterBullet>() && bCanCollideWithOthersBullet)
        {
            FxManager.Instance.PlayFx("VFX_ExplosionShooterBullet", transform.position, Quaternion.identity, bullet.explosionRadius);
            //speAffect.OnExplosion(bullet.bulletDammage, bullet.forceAppliedOnImpact, bullet.stunValue);
            HitBullet();
            KillBullet();
        }
    }

    public void OnGravityDirectHit()
    {
        throw new System.NotImplementedException();
    }

    public void OnPull(Vector3 position, float force)
    {
        bOnGravity = true;
        GetComponent<Rigidbody>().useGravity = true;
        Destroy(hCircle);
        ReactGravity.DoPullAsObject(this.gameObject, position, force, false);
    }

    public void OnRelease()
    {
        throw new System.NotImplementedException();
    }

    public void OnHold()
    {
        throw new System.NotImplementedException();
    }

    public void OnZeroG()
    {
        throw new System.NotImplementedException();
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        throw new System.NotImplementedException();
    }
}
