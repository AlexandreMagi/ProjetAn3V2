using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoWallBreak : MonoBehaviour, IBulletAffect
{
    Rigidbody rb;
    SphereCollider shCollider;
    MeshCollider meCollider;

    bool canBeAffectedByGravity = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        shCollider = GetComponent<SphereCollider>();
        meCollider = GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 && canBeAffectedByGravity)
        {
            if (rb.isKinematic != false)
                rb.isKinematic = false;
        }
    }

    void setStatic()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            if (hit.distance <= 0.5f)
            {
                canBeAffectedByGravity = false;
                rb.isKinematic = true;
                rb.useGravity = false;
                shCollider.enabled = false;
            }
            else
                return;

        }
    }

    bool setState()
    {
        if (setState() == true)
        {
            canBeAffectedByGravity = false;
            rb.isKinematic = true;
            rb.useGravity = false;
            shCollider.enabled = false;
        }
        else if (setState() == false)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            shCollider.enabled = true;
        }

        return setState();
    }

    public void OnBulletClose()
    {
    }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage)
    {
        if (canBeAffectedByGravity)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            shCollider.enabled = true;
            rb.AddExplosionForce(1000, gameObject.transform.position, 100);
            Invoke("setStatic", 5);
        }
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
    }
}
