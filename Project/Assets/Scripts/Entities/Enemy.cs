using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity, IDetection
{
    float currentStunLevel = 0;
    float timeRemaingingStun = 0;
    bool isStun = false;
    private DataEnemy enemyData;

    List<Transform> enemies;
    float distanceToClosest;
    Transform possibleTarget;

    protected Transform target;


    public virtual void OnMovementDetect()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDangerDetect()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDistanceDetect(Transform target, float distance)
    {
        throw new System.NotImplementedException();
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enemyData = entityData as DataSwarmer;

        StartCoroutine(CheckForTargets());
    }


    protected void AddStun(float ammount, float stunDuration)
    {
        if (!isStun)
        {
            currentStunLevel += ammount;
            if (currentStunLevel > enemyData.stunResistanceJauge)
            {
                IsStun(stunDuration);
            }
        }
    }

    protected virtual void IsStun(float stunDuration)
    {
        currentStunLevel = 0;
        timeRemaingingStun = stunDuration;
        isStun = true;
    }

    protected virtual void Update()
    {
        if (timeRemaingingStun > 0)
        {
            timeRemaingingStun -= Time.deltaTime;
            if (timeRemaingingStun <= 0)
            {
                isStun = false;
            }
        }
    }

    IEnumerator CheckForTargets()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(.5f);

            //Recherche de cible à attaquer
            enemies = TeamsManager.Instance.GetAllEnemiesFromTeam(this.enemyData.team, new int[]{2});
            if (enemies.Count > 0)
            {
                distanceToClosest = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(enemies[0].position.x, enemies[0].position.z));
                possibleTarget = enemies[0];

                if (enemies.Count > 1)
                {
                    for (int i = 1; i < enemies.Count; i++)
                    {
                        float distanceTemp = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(enemies[i].position.x, enemies[i].position.z));
                        if (distanceTemp < distanceToClosest)
                        {
                            distanceToClosest = distanceTemp;
                            possibleTarget = enemies[i];
                        }
                    }
                }

                OnDistanceDetect(possibleTarget, distanceToClosest);
            }
        }

    }

}
