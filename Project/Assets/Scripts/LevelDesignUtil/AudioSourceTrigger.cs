using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceTrigger : MonoBehaviour
{
    [SerializeField]
    bool isActiveOnTrigger = true;
    [SerializeField]
    bool desactivateMusic = false;
    [SerializeField]
    bool desactivateWind = false;

    AudioSourceManager aud;
    private void Start()
    {
        aud = GetComponentInParent<AudioSourceManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isActiveOnTrigger)
            aud.SetAudioSource(true);
        else if (!isActiveOnTrigger)
            aud.SetAudioSource(false);

        if (desactivateMusic) Main.Instance.CutMusic();

        if (desactivateWind && WindSoundHandler.Instance != null)
        {
            WindSoundHandler.Instance.Cut();
            CustomSoundManager.Instance.PlaySound("Crowd_Cheer", "Effect", 1f);
        }
    }
}
