using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBonus : MonoBehaviour,IBulletAffect
{

    [HideInInspector] public SpecialBonusManager manager = null;
    [SerializeField] EasterEggHandler.SpecialBonusType bonusType = EasterEggHandler.SpecialBonusType.juggernaut;

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray shotRay)
    {
        if (manager != null)
        {
            manager.BonusDestroyed(bonusType);
        }
        Debug.Log("Jouer FX ici");
        gameObject.SetActive(false);
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        if (!Weapon.Instance.CheckIfModIsMinigun(mod))
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);
    }
}
