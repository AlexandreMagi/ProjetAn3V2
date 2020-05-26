using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerKillerObjectChanger : MonoBehaviour
{
    [SerializeField] KillerObject affectedKillerObject = null;
    [SerializeField] bool playerKillState = true;

    private void OnTriggerEnter(Collider other)
    {
        if (affectedKillerObject != null)
            affectedKillerObject.countsAsPlayerKill = playerKillState;
    }
}
