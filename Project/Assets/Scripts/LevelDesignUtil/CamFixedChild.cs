﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFixedChild : MonoBehaviour, IBulletAffect, ISpecialEffects
{

    public FixedCameraScript parentScript;

    #region bulletAffect
    public void OnBulletClose() { }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        if(parentScript != null)
            parentScript.hitByBullet();
    }

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
        if (parentScript != null)
            parentScript.hitByBullet();
    }

    public void OnHitShotGun(DataWeaponMod mod) {

        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod) { }
    #endregion
}
