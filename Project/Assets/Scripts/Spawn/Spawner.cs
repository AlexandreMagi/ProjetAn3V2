﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    protected float fTimer = 0;

    [SerializeField]
    protected DataSpawner spawnerType = null;

    [SerializeField]
    protected bool isLimited = false;

    protected float enemiesSpawned = 0;

    [SerializeField]
    protected bool spawnEnabled = false;



    protected virtual void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }

    public void StartSpawn()
    {
        spawnEnabled = true;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (spawnEnabled)
        {
            fTimer += Time.deltaTime;
            if (fTimer > 1 / spawnerType.fEnnemiPerSecond && spawnerType.iNbEnemiesSpawnable >= this.transform.childCount)
            {
                fTimer -= 1 / spawnerType.fEnnemiPerSecond;

                SpawnEnemy();
            }
        }
    }

    protected virtual GameObject SpawnEnemy()
    {
        GameObject spawnedEnemy = Instantiate(spawnerType.EnnemiPrefab);
        spawnedEnemy.transform.position = transform.position;
        spawnedEnemy.transform.SetParent(this.transform);
        //spawnedEnemy.transform.localScale= new Vector3(1, 1, 1);
        if (spawnerType.bIsImpulseSpawn)
        {
            spawnedEnemy.GetComponent<Rigidbody>().AddForce(spawnerType.v3Direction * spawnerType.fImpulseForce);
        }

        if (isLimited)
        {
            enemiesSpawned++;

            if (enemiesSpawned >= spawnerType.iNbEnemiesSpawnable)
            {
                spawnEnabled = false;
            }
        }

        return spawnedEnemy;
    }

}
