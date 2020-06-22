using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBonus : MonoBehaviour,IBulletAffect
{

    [HideInInspector] public SpecialBonusManager manager = null;

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray shotRay)
    {
        if (manager != null)
        {
            manager.BonusDestroyed();
        }
        gameObject.SetActive(false);
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
    }
}
