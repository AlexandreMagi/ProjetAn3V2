using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFixedChild : MonoBehaviour, IBulletAffect
{

    public FixedCameraScript parentScript;

    #region bulletAffect
    public void OnBulletClose() { }

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
        parentScript.hitByBullet();
    }

    public void OnHitShotGun() { }

    public void OnHitSingleShot() { }
    #endregion
}
