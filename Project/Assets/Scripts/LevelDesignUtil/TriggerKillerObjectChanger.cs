using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerKillerObjectChanger : MonoBehaviour
{
    [SerializeField] KillerObject affectedKillerObject = null;
    [SerializeField] bool playerKillState = true;
    [SerializeField] float timerBeforeAction = 0;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(TimerBeforceAction());
    }

    IEnumerator TimerBeforceAction()
    {
        yield return new WaitForSeconds(timerBeforeAction);
        Action();
        yield break;
    }

    void Action()
    {
        if (affectedKillerObject != null)
            affectedKillerObject.countsAsPlayerKill = playerKillState;
    }
}
