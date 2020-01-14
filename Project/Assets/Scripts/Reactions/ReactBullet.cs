using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReactBullet
{
    public static void PushFromHit(Rigidbody objRigid, Vector3 position, float force, float radius)
    {
        objRigid.AddExplosionForce(force, position, radius);
        ReactGravity<DataEntity>.DoSpin(objRigid.GetComponent<Rigidbody>());
    }
}
