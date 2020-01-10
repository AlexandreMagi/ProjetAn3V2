using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    float currentStunLevel = 0;
    float timeRemaingingStun = 0;
    bool isStun = false;
    private DataEnemy enemyData;

    protected Transform target;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enemyData = entityData as DataSwarmer;
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

}
