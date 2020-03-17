using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderMaskPosition : MonoBehaviour, IBulletAffect
{
    Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray rayShot)
    {
        mat.SetVector("_HitPosition", transform.InverseTransformPoint(position));
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
    }
}
