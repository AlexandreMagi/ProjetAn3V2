using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoWallBreakManager : MonoBehaviour, IBulletAffect
{
    [SerializeField]
    GameObject breakMesh;

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage)
    {
        gameObject.SetActive(false);
        breakMesh.SetActive(true);
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
    }
}
