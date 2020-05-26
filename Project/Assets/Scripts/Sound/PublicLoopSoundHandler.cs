using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicLoopSoundHandler : MonoBehaviour
{
    public static PublicLoopSoundHandler Instance { get; private set; }
    void Awake() { Instance = this; }

    AudioSource publicAudioSource = null;
    float volume = 0;
    float lastVolume = 0;
    float aimedVolume = 0;
    float timeTransition = 1;

    void Start()
    {
        if (CustomSoundManager.Instance != null && CameraHandler.Instance != null) publicAudioSource = CustomSoundManager.Instance.PlaySound("SE_LoopCrowdCheer", "PublicAmbiant", CameraHandler.Instance.renderingCam.transform, 0.2f, true);
        if (publicAudioSource != null) publicAudioSource.volume = 0;
    }

    void Update()
    {
        volume = Mathf.MoveTowards(volume, aimedVolume, Mathf.Abs(aimedVolume - lastVolume) * Time.deltaTime/ timeTransition);
        if (publicAudioSource != null) publicAudioSource.volume = volume;
    }

    public void ChangePublicVolume (float volumeAimed, float time)
    {
        if (publicAudioSource != null) lastVolume = publicAudioSource.volume;
        aimedVolume = volumeAimed;
        timeTransition = time;
    }

}
