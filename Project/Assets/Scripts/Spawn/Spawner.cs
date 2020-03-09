using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Spawner : MonoBehaviour
{
    protected float fTimer = 0;

    [SerializeField]
    bool isPathedSpawner = false;

    [SerializeField, ShowIf("isPathedSpawner")]
    List<Pather> pathsToGive = null;

    [SerializeField, ShowIf("isPathedSpawner")]
    bool isRandomSpawner = true;

    [SerializeField, ShowIf("isPathedSpawner"), HideIf("isRandomSpawner")]
    List<int> numberOfSpawnsToChangePather = null;
    int currentPather = 0;
    int spawnedInCurrentPath = 0;

    [SerializeField, ShowIf("isPathedSpawner"), HideIf("isRandomSpawner")]
    bool loops = false;

    //ENTITY DATA
    [SerializeField]
    DataEntity entDataToGive = null;

    [Header("Data bullet changes")]
    [SerializeField]
    public bool switchesBulletData;

    [SerializeField, ShowIf("switchesBulletData")]
    public DataShooterBullet dataBulletToSwap;

    //SPAWNER DATA
    [SerializeField]
    protected DataSpawner spawnerType = null;

    [SerializeField]
    protected bool isLimited = false;

    protected float enemiesSpawned = 0;

    [SerializeField]
    protected bool spawnEnabled = false;

    [Header("Propulsion")]
    [SerializeField]
    protected bool isPropulsionSpawner = false;

    [SerializeField, ShowIf("isPropulsionSpawner")]
    protected Vector3 impulseDirection = Vector3.zero;

    [SerializeField, ShowIf("isPropulsionSpawner")]
    protected float impulseForce = 0;

    [Header("Spawn at range")]
    [SerializeField]
    bool isRangedSpawner = false;

    [SerializeField, ShowIf("isRangedSpawner")]
    float rangeRadius = 0f;



    protected virtual void Start()
    {
        this.GetComponent<MeshRenderer>().enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (isRangedSpawner)
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(this.transform.position, this.transform.up, rangeRadius);
        }
    }
#endif


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

            if (fTimer > 1 / spawnerType.fEnnemiPerSecond && enemiesSpawned < spawnerType.iNbEnemiesSpawnable)
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

            //Bullet changes
            if(entDataToGive is DataShooter && switchesBulletData && dataBulletToSwap != null)
            {
                Shooter shoot = spawnedEnemy.GetComponent<Shooter>();
                if(shoot)
                    shoot.SetBullets(dataBulletToSwap);
            }

            //Debug.Log(pathsToGive.Count);

            if (isPathedSpawner && pathsToGive.Count > 0)
            {
                int pathApplied;

                if (isRandomSpawner)
                {
                    if (pathsToGive.Count == 1)
                    {
                        pathApplied = 0;
                    }
                    else
                    {
                        pathApplied = Random.Range(0, pathsToGive.Count);
                    }
    
                }
                else
                {
                    if(spawnedInCurrentPath < numberOfSpawnsToChangePather[currentPather])
                    {
                        spawnedInCurrentPath++;
                        
                    }
                    else
                    {
                        if(currentPather < numberOfSpawnsToChangePather.Count - 1)
                        {
                            currentPather++;
                        }
                        else
                        {
                            if (loops)
                            {
                                currentPather = 0;
                            }
                        }
                    }
                    pathApplied = currentPather;
                }

                spawnedEnemy.GetComponent<Swarmer>().SetPathToFollow(pathsToGive[pathApplied]);
            }

            //spawnedEnemy.transform.localScale= new Vector3(1, 1, 1);
            if (isPropulsionSpawner)
            {
                spawnedEnemy.GetComponent<Rigidbody>().AddForce(impulseDirection * impulseForce);
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
