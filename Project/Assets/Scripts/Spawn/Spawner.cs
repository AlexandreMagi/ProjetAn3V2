﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    protected float fTimer = 0;

    [SerializeField]
    bool isPathedSpawner = false;

    [SerializeField]
    Pather pathToGive = null;

    [SerializeField]
    DataEntity entDataToGive = null;

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
        if (isLimited)
        {
            enemiesSpawned++;

            if (enemiesSpawned >= spawnerType.iNbEnemiesSpawnable)
            {
                spawnEnabled = false;
            }

            else
            {
                GameObject spawnedEnemy;
                if (entDataToGive is DataSwarmer)
                {
                    spawnedEnemy = SwarmerPullHandler.Instance.GetSwarmer(entDataToGive);
                }
                else
                {
                    spawnedEnemy = Instantiate(spawnerType.EnnemiPrefab);
                    if (entDataToGive != null) spawnedEnemy.GetComponent<Entity<DataEntity>>().SetData(entDataToGive);
                }

                spawnedEnemy.transform.SetParent(this.transform, false);
                spawnedEnemy.transform.position = transform.position;


                if (isPathedSpawner && pathToGive != null)
                {
                    spawnedEnemy.GetComponent<Swarmer>().SetPathToFollow(pathToGive);
                }

                //spawnedEnemy.transform.localScale= new Vector3(1, 1, 1);
                if (spawnerType.bIsImpulseSpawn)
                {
                    spawnedEnemy.GetComponent<Rigidbody>().AddForce(spawnerType.v3Direction * spawnerType.fImpulseForce);
                }

                return spawnedEnemy;
            }
        }

        return null;
    }

}
