using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesCollisonDetection : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particlesSystemSplatter = null;

    [SerializeField]
    ParticleDecalPool splatDecalPool;

    ParticleSystem particleSystem;

    List<ParticleCollisionEvent> collisionEvents;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleSystem, other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            splatDecalPool.ParticleHit(collisionEvents[i]);
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
