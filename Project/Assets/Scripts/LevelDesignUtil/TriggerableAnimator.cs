using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerableAnimator : MonoBehaviour
{
    [SerializeField]
    List<string> vfxNames = null;

    [SerializeField]
    List<Transform> vfxPositions = null;
    int currentVFX = 0;


    /// <summary>
    /// Must be called only from the Animator. These can be stacked. Plays the said vfx at the precised position.
    /// </summary>
    void OnKeyFrameEvent()
    {
        if(currentVFX < vfxNames.Count && currentVFX < vfxPositions.Count)
        {
            FxManager.Instance.PlayFx(vfxNames[currentVFX], vfxPositions[currentVFX].position, vfxPositions[currentVFX].rotation);
            currentVFX++;
        }
       
    }
}
