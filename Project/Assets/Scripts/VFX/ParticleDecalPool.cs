using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDecalPool : MonoBehaviour
{
    [SerializeField]
    int maxDecals = 100;

    [SerializeField]
    float decalSizeMin = 0.5f;

    [SerializeField]
    float decalSizeMax = 1.5f;

    int particleDecalDataIndex;

    ParticleDecalData[] particleData;

    ParticleSystem.Particle[] particles;

    ParticleSystem decalParticleSystem;
         
    void Start()
    {
        particles = new ParticleSystem.Particle[maxDecals];

        particleData = new ParticleDecalData[maxDecals];
        for (int i = 0; i < maxDecals; i++)
        {
            particleData[i] = new ParticleDecalData();
        }

        decalParticleSystem = GetComponent<ParticleSystem>();
    }

    public void ParticleHit(ParticleCollisionEvent particleCollisionEvent)
    {
        SetParticleData(particleCollisionEvent);
        DisplayParticles();
    }

    void SetParticleData (ParticleCollisionEvent particleCollisionEvent)
    {
        if (particleDecalDataIndex >= maxDecals)
        {
            particleDecalDataIndex = 0;
        }else
        {
            particleData[particleDecalDataIndex].position = particleCollisionEvent.intersection;

            Vector3 particleRotationEuler = particleCollisionEvent.normal.magnitude != 0 ? Quaternion.LookRotation(particleCollisionEvent.normal).eulerAngles : Quaternion.identity.eulerAngles;
            particleRotationEuler.z = Random.Range(0, 360);
            particleData[particleDecalDataIndex].rotation = particleRotationEuler;

            particleData[particleDecalDataIndex].size = new Vector3(Random.Range(decalSizeMin, decalSizeMax), Random.Range(decalSizeMin, decalSizeMax), Random.Range(decalSizeMin, decalSizeMax));

            particleData[particleDecalDataIndex].color = Color.white;

            particleDecalDataIndex++;
        }
    }

    void DisplayParticles()
    {
        for (int i = 0; i < particleData.Length; i++)
        {
            particles[i].position = particleData[i].position;
            particles[i].rotation3D = particleData[i].rotation;
            particles[i].startSize3D = particleData[i].size;
            particles[i].startColor = particleData[i].color;
        }

        decalParticleSystem.SetParticles(particles, particles.Length);
        decalParticleSystem.transform.rotation = Quaternion.Euler(0,0,0);
    }
}
