using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceManager : MonoBehaviour
{

    public void SetAudioSource(bool isActiveOnTrigger)
    {
        if (isActiveOnTrigger)
        {
            AudioSource[] aud = GetComponents<AudioSource>();
            foreach (AudioSource a in aud)
            {
                a.enabled = true;
            }
        }else if (!isActiveOnTrigger)
        {
            AudioSource[] aud = GetComponents<AudioSource>();
            foreach (AudioSource a in aud)
            {
                a.enabled = false;
            }
        }
    }

}
