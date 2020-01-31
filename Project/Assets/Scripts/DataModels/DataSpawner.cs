using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Spawner/DataSpawner")]
public class DataSpawner : ScriptableObject
{
    public float fEnnemiPerSecond = 0;

    public GameObject EnnemiPrefab = null;

    public int iNbEnemiesSpawnable = 0;
}
