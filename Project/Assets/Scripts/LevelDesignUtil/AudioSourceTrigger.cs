﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceTrigger : MonoBehaviour
{
    [SerializeField]
    bool isActiveOnTrigger = true;

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
    }
}
