using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBonus : MonoBehaviour,IBulletAffect
{

    [HideInInspector] public SpecialBonusManager manager = null;
    [SerializeField] EasterEggHandler.SpecialBonusType bonusType = EasterEggHandler.SpecialBonusType.juggernaut;
    [SerializeField] string fxWhenDie = null;

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray shotRay)
    {
        if (manager != null)
        {
            switch (bonusType)
            {
                case EasterEggHandler.SpecialBonusType.juggernaut:
                    EasterEggHandler.Instance.JuggernautUnlockedNextGame = true;
                    break;
                case EasterEggHandler.SpecialBonusType.aikant:
                    EasterEggHandler.Instance.AikantUnlockedNextGame = true;
                    break;
                case EasterEggHandler.SpecialBonusType.fanfaron:
                    EasterEggHandler.Instance.FanfaronUnlockedNextGame = true;
                    break;
                default:
                    break;
            }
            manager.BonusDestroyed(bonusType);
        }
        UIEasterEggHandler.Instance.TriggerVisualDisplayEasterEgg(bonusType);
        FxManager.Instance.PlayFx(fxWhenDie, transform.position, Quaternion.identity);
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
