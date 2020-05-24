using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSoundHandler : MonoBehaviour
{

    [SerializeField] string soundToPlayOnImpact = "";
    [SerializeField] float soundVolume = 1;
    [SerializeField] float soundRandomPitch = 0.2f;
    

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2)
        {
            AudioSource collisionAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayOnImpact, "Effect", transform, soundVolume, false, 1, soundRandomPitch);
            if (collisionAudioSource != null)
            {
                collisionAudioSource.spatialBlend = 1;
                collisionAudioSource.minDistance = 8;
            }

        }
    }
}
