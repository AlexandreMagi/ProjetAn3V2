using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerableAnimator : MonoBehaviour
{
    [SerializeField]
    List<string> vfxNames;

    List<Transform> vfxPositions;
    int currentVFX = 0;

    void OnKeyFrameEvent()
    {
        if(currentVFX < vfxNames.Count && currentVFX < vfxPositions.Count)
        {
            FxManager.Instance.PlayFx(vfxNames[currentVFX], vfxPositions[currentVFX].position, vfxPositions[currentVFX].rotation);
            currentVFX++;
        }
       
    }
}
