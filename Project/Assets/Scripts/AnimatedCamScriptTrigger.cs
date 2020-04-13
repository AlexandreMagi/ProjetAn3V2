using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedCamScriptTrigger : MonoBehaviour
{

    [SerializeField]
    Animator[] anim = null;

    [SerializeField]
    bool loopTimer = false;

    [SerializeField]
    float slowMoPower = 10;

    [SerializeField]
    float slowMoDuration = 10;


    public void AddSlowMo()
    {
        TimeScaleManager.Instance.AddSlowMo(slowMoPower, slowMoDuration);
    }

    public void TriggerAnim()
    {
        TriggerUtil.TriggerAnimators(0, anim,loopTimer);
    }
}
