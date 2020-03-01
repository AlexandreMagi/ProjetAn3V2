using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoWallBreak : MonoBehaviour, IBulletAffect
{

    MeshRenderer renderer;
    Rigidbody rb;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage)
    {
        renderer.enabled = true;
        rb.isKinematic = false;
        rb.AddForce(-position.normalized*100);
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
    }
}
