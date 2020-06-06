using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmerAnimSound : MonoBehaviour
{
    public void PlayGruntSound()
    {
        AudioSource killAudioSource = CustomSoundManager.Instance.PlaySound("SE_Swarmer_Grunt", "Effect", null, .4f, false, 1, .3f, 0, 3);
        if (killAudioSource != null)
        {
            killAudioSource.spatialBlend = 1;
            killAudioSource.minDistance = 8;
            killAudioSource.transform.position = transform.position;

        }
    }
}
