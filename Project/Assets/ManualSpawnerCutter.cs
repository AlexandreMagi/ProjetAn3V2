using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualSpawnerCutter : MonoBehaviour
{
    Spawner[] listOfSpawnersToStop;

    float timeBeforeStart;

    public void cutSpawners()
    {
        TriggerUtil.TriggerSpawners(timeBeforeStart, listOfSpawnersToStop, false);
    }
}
