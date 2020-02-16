using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrbHitDetection : MonoBehaviour
{
    private float hitTime;
    private Material mat;

    bool asHit = false;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            mat.SetVector("_HitPosition", transform.InverseTransformPoint(contact.point));
            hitTime = 5;
            mat.SetFloat("_HitTime", hitTime);
            Invoke("AfterHit", 0.1f);
        }
    }

    void AfterHit()
    {
        asHit = true;
    }

    private void Update()
    {
        if (asHit)
        {
            if (hitTime >= 0)
            {
                hitTime -= Time.deltaTime;
                mat.SetFloat("_HitTime", hitTime);
            }
            else
            {
                asHit = false;
            }
        }
    }
}
