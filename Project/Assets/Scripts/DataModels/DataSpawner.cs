using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Spawner/DataSpawner")]
public class DataSpawner : ScriptableObject
{
    public float fEnnemiPerSecond = 0;

    public GameObject EnnemiPrefab = null;

    public int iNbEnemiesSpawnable = 0;

    public bool bIsImpulseSpawn = false;

    [ShowWhen("bIsImpulseSpawn")]
    public Vector3 v3Direction = Vector3.zero;

    [ShowWhen("bIsImpulseSpawn")]
    public float fImpulseForce = 0;
}
