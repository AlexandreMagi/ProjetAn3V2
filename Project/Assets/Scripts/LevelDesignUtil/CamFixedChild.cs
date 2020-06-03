using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFixedChild : MonoBehaviour, IBulletAffect, ISpecialEffects
{

    public FixedCameraScript parentScript;

    [SerializeField]
    string sound = null;

    [SerializeField]
    float soundVolume = 1f;

    [SerializeField] BoxCollider camCollider = null;

    bool destroyed = false;

    #region bulletAffect
    public void OnBulletClose() { }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        Destroyed(null);
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray rayShot)
    {
        Destroyed(mod);
    }

    public void OnHitShotGun(DataWeaponMod mod) {

        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        //if (!Weapon.Instance.CheckIfModIsMinigun(mod))
        //    MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);
    }

    void Destroyed(DataWeaponMod mod)
    {
        if (parentScript != null && !destroyed)
        {
            camCollider.enabled = true;
            //CustomSoundManager.Instance.PlaySound(gameObject, sound, false, soundVolume, 0.3f, 0, true);
            CustomSoundManager.Instance.PlaySound(sound, "Effect", null, soundVolume, false, 1, 0.3f);
            parentScript.hitByBullet(mod);
            destroyed = true;
        }
    }

    #endregion
}
