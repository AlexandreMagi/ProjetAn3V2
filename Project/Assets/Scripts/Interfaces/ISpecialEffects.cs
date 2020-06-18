using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecialEffects
{
    void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0, bool damageCamera = true);

}
