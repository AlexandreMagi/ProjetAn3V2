using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatOnCollision : MonoBehaviour
{
    ParticleSystem particleMain;

    [SerializeField]
    ParticleDecalPool dropletDecalPool;

    List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        particleMain = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(particleMain, other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            dropletDecalPool.ParticleHit(collisionEvents[i]);
        }
    }
}
