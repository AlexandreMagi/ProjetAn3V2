using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupprNextLdTuto : MonoBehaviour, IBulletAffect
{
    [SerializeField]
    int baseShootCountBeforeFirstInt = 5;
    [SerializeField]
    int baseShootCountBeforeSecondInt = 15;

    int baseShootCountIncrement;

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        Debug.Log("BasicHit");

        baseShootCountIncrement++;

        if (baseShootCountIncrement == baseShootCountBeforeFirstInt)
        {
            Debug.Log("First hint");
        }
        else if (baseShootCountIncrement == baseShootCountBeforeSecondInt)
        {
            Debug.Log("Second hint");
        }
    }
}
