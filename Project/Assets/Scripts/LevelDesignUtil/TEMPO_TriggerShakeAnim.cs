using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPO_TriggerShakeAnim : MonoBehaviour
{
    [Header("Shake")]
    [SerializeField]
    float shakeForce = 10f;

    [SerializeField]
    float shakeDuration = 0.5f;

    [Header("Sound")]
    [SerializeField]
    string[] soundName = null;

    [SerializeField]
    float[] soundVolume = null;

    int soundIndex;

    void TriggerShake()
    {
        CameraHandler.Instance.AddShake(shakeForce, shakeDuration);
    }

    void Sound()
    {
        CustomSoundManager.Instance.PlaySound(gameObject, soundName[soundIndex], false, soundVolume[soundIndex], 0.3f, 0, true);

        soundIndex++;
    }

}
