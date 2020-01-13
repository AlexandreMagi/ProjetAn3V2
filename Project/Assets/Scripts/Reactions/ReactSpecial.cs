using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactSpecial
{
    //Pushing mechanic
    public static void DoProject(Entity obj, Vector3 explosionOrigin, float explosionForce, float explosionRadius, float liftValue = 0)
    {
        obj.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionOrigin, explosionRadius, liftValue);
    }

    public static void DoExplosionDammage(Entity obj, Vector3 explosionOrigin, float explosionDamage, float explosionRadius)
    {
        float distance = Vector3.Distance(obj.transform.position, explosionOrigin);
        float dammage = 0;
        if (distance < explosionRadius)
        {
            dammage = explosionDamage - (distance * explosionDamage / explosionRadius);
        }
        obj.TakeDamage(dammage);
    }

    public static void DoExplosionStun(Entity obj, Vector3 explosionOrigin, float explosionStun,float explosionStunDuration, float explosionRadius)
    {
        float distance = Vector3.Distance(obj.transform.position, explosionOrigin);
        float stun = 0;
        if (distance < explosionRadius)
        {
            stun = explosionStun - (distance * explosionStun / explosionRadius);
        }
        if (obj is Enemy)
        {
            Enemy enemiVar = obj as Enemy;
            enemiVar.AddStun(stun, explosionStunDuration);
        }
    }

}
