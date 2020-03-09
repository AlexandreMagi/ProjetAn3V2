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

    bool destroyed = false;

    #region bulletAffect
    public void OnBulletClose() { }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        Destroyed();
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage)
    {
        Destroyed();
    }

    public void OnHitShotGun(DataWeaponMod mod) {

        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod) { }

    void Destroyed()
    {
        if (parentScript != null && !destroyed)
        {
            CustomSoundManager.Instance.PlaySound(gameObject, sound, false, soundVolume, 0.3f, 0, true);
            parentScript.hitByBullet();
            destroyed = true;
        }
    }

    #endregion
}
