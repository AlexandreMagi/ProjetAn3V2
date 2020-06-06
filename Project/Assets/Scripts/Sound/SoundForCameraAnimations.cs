using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundForCameraAnimations : MonoBehaviour
{
    public void PlayStepSound()
    {
        CustomSoundManager.Instance.PlaySound("Step_0" + Random.Range(1, 5), "Player", 1);
    }
    public void PlayEffortSound()
    {
        CustomSoundManager.Instance.PlaySound("SE_WallJump", "Player", transform, 1, false, 1, 0.2f);
    }
}
