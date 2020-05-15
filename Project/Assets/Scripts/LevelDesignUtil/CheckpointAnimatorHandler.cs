using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointAnimatorHandler : MonoBehaviour
{

    [SerializeField] Animator animatorThatReceiveTriggers = null;
    [SerializeField] string[] triggersToCall = null;
    [SerializeField] float[] triggersTimers = null;
    [SerializeField] bool playOnlyOnce = true;
    bool canPlay = true;

    private void OnTriggerEnter(Collider other)
    {
        if (canPlay)
        {
            if (playOnlyOnce) canPlay = false;
            for (int i = 0; i < triggersToCall.Length; i++)
            {
                StartCoroutine(CallTrigger(Animator.StringToHash(triggersToCall[i]), (i < triggersTimers.Length) ? triggersTimers[i] : 0));
            }
        }
    }

    IEnumerator CallTrigger(int trigger, float timer)
    {
        if (timer > 0) yield return new WaitForSeconds(timer);
        if (animatorThatReceiveTriggers != null)
            animatorThatReceiveTriggers.SetTrigger(trigger);
        yield break;
    }

}
