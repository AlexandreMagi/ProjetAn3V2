using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCollisonDetection : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particlesSystemSplatter = null;

    [SerializeField]
    ParticleDecalPool splatDecalPool = null;

    ParticleSystem _particleSystem;

    List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(_particleSystem, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            splatDecalPool.ParticleHit(collisionEvents[i]);
            if (particlesSystemSplatter != null)
                EmitFromSplatter(collisionEvents[i]);
        }

    }

    void EmitFromSplatter(ParticleCollisionEvent particleCollisionEvent)
    {
        particlesSystemSplatter.transform.position = particleCollisionEvent.intersection;
        particlesSystemSplatter.transform.rotation = particleCollisionEvent.normal.magnitude != 0 ?Quaternion.LookRotation(particleCollisionEvent.normal) : Quaternion.identity;

        particlesSystemSplatter.Emit(1);
    }
}
