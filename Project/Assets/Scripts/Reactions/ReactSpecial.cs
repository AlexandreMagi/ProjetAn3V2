using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactSpecial<T, T2> where T : DataEntity where T2 : DataEnemy
{
    //Pushing mechanic
    public static void DoProject(Rigidbody rb, Vector3 explosionOrigin, float explosionForce, float explosionRadius, float liftValue = 0)
    {
        rb.AddExplosionForce(explosionForce, explosionOrigin, explosionRadius, liftValue);
    }

    public static void DoExplosionDammage(Entity<T> obj, Vector3 explosionOrigin, float explosionDamage, float explosionRadius)
    {
        float distance = Vector3.Distance(obj.transform.position, explosionOrigin);
        float dammage = 0;
        if (distance < explosionRadius)
        {
            dammage = explosionDamage - (distance * explosionDamage / explosionRadius);
        }
        obj.TakeDamage(dammage);
    }

    public static void DoExplosionStun(Entity<T> obj, Vector3 explosionOrigin, float explosionStun,float explosionStunDuration, float explosionRadius)
    {
        float distance = Vector3.Distance(obj.transform.position, explosionOrigin);
        float stun = 0;
        if (distance < explosionRadius)
        {
            stun = explosionStun - (distance * explosionStun / explosionRadius);
        }
        if (obj is Enemy<T2>)
        {
            Enemy<T2> enemiVar = obj as Enemy<T2>;
            enemiVar.AddStun(stun, explosionStunDuration);
        }
    }

}
