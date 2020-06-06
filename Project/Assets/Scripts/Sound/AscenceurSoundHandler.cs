using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscenceurSoundHandler : MonoBehaviour
{

    [SerializeField] string soundToPlay = "SE_Ascenceur_Idle";
    float currentPitch = 0.5f;
    float aimedPitch = 0.5f;
    float timeToTransition = 0.5f;
    AudioSource sourceUsed = null;

    void Start()
    {
        sourceUsed = CustomSoundManager.Instance.PlaySound(soundToPlay, "Effect", CameraHandler.Instance.renderingCam.transform, 1, true, currentPitch);
        if (sourceUsed != null)
        {
            sourceUsed.spatialBlend = 1;
            sourceUsed.minDistance = 8;
            sourceUsed.transform.position = transform.position;
        }
    }

    void Update()
    {
        if (sourceUsed != null)
        {
            sourceUsed.transform.position = transform.position;
            sourceUsed.pitch = currentPitch;
            currentPitch = Mathf.MoveTowards(currentPitch, aimedPitch, Time.deltaTime / timeToTransition);
        }
    }

    public void PitchUp()
    {
        aimedPitch = 1f;
        timeToTransition = .3f;
    }

    public void PitchDown()
    {
        aimedPitch = 0.5f;
        timeToTransition = .3f;
    }
}
