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

    public bool bIsImpulseSpawn = false;

    [ShowIf("bIsImpulseSpawn")]
    public Vector3 v3Direction = Vector3.zero;

    [ShowIf("bIsImpulseSpawn")]
    public float fImpulseForce = 0;
}
