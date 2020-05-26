using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSpawnerCutter : MonoBehaviour
{
    [SerializeField] Spawner[] listOfSpawnersToStop;

    [SerializeField] float timeBeforeStart;

    public void cutSpawners()
    {
        TriggerUtil.TriggerSpawners(timeBeforeStart, listOfSpawnersToStop, false);
    }
}
