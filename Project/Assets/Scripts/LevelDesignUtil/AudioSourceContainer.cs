using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceContainer : MonoBehaviour
{
    [SerializeField]
    AudioSource[] audioSources = null;

    private float TimeScaleMultiplier = 1;
    private void Awake()
    {
        TimeScaleMultiplier = 0.5f + Time.timeScale / 2;
    }

    private void Update()
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.pitch = 0.5f + Time.timeScale / 2;
        }

    }

}
