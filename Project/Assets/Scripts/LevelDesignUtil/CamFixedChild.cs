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

    #region bulletAffect
    public void OnBulletClose() { }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        if (parentScript != null)
        {
            CustomSoundManager.Instance.PlaySound(gameObject, sound, false, soundVolume, 0.3f, 0, true);
            parentScript.hitByBullet();
        }
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage)
    {
        if (parentScript != null)
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.DamageFixedCam, gameObject.transform.position);
            CustomSoundManager.Instance.PlaySound(gameObject, sound, false, soundVolume, 0.3f, 0, true);
            parentScript.hitByBullet();
        }
    }

    public void OnHitShotGun(DataWeaponMod mod) {

        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod) { }
    #endregion
}
