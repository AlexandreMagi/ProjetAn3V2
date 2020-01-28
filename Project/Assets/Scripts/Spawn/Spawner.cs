﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

    [Header("Spawn at range")]
    [SerializeField]
    bool isRangedSpawner = false;

    [SerializeField, ShowIf("isRangedSpawner")]
    float rangeRadius = 0f;



    protected virtual void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (isRangedSpawner)
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(this.transform.position, this.transform.up, rangeRadius);
        }
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

            if (fTimer > 1 / spawnerType.fEnnemiPerSecond && enemiesSpawned <= spawnerType.iNbEnemiesSpawnable)
            {
                enemiesSpawned++;

                fTimer -= 1 / spawnerType.fEnnemiPerSecond;

                SpawnEnemy();

                if (enemiesSpawned >= spawnerType.iNbEnemiesSpawnable)
                {
                    spawnEnabled = false;
                }
            }
        }
    }

    protected virtual GameObject SpawnEnemy()
    {
        
        if (spawnEnabled)
        {
            GameObject spawnedEnemy;
            if (entDataToGive is DataSwarmer)
            {
                spawnedEnemy = SwarmerPullHandler.Instance.GetSwarmer(entDataToGive);
            }
            else
            {
                spawnedEnemy = Instantiate(spawnerType.EnnemiPrefab);
                //Debug.Log(spawnedEnemy);
                if (entDataToGive != null) spawnedEnemy.GetComponent<IEntity>().SetDataEnt(entDataToGive);
            }

            spawnedEnemy.transform.SetParent(this.transform, false);

            if (isRangedSpawner)
            {
                float randomX = Random.Range(-rangeRadius, rangeRadius);
                float randomZ = Random.Range(-rangeRadius, rangeRadius);
                spawnedEnemy.transform.position = transform.position + new Vector3(randomX, 0, randomZ);
            }
            else
            {
                spawnedEnemy.transform.position = transform.position;
            }
           


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
        
        return null;
    }

    public void ChildDied()
    {
        if(!isLimited)
            enemiesSpawned--;
    }

}
