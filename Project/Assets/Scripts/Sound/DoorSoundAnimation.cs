using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSoundAnimation : MonoBehaviour
{
    public void PlayDoorSound()
    {
        CustomSoundManager.Instance.PlaySound("SE_Porte", "Effect", CameraHandler.Instance.renderingCam.transform, 0.3f);
    }
}
