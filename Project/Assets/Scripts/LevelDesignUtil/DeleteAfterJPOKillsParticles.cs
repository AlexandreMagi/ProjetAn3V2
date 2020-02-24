using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterJPOKillsParticles : MonoBehaviour
{
    [SerializeField]
    ParticleSystem psToStop = null;

    public void StopParticles()
    {
        psToStop.Stop();
    }
}
