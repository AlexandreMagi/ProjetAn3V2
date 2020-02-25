﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerObject : MonoBehaviour
{
    float timeBeforeEndOfMulti = 0;

    [SerializeField]
    bool killEnemyEqualsDamage = false;

    [SerializeField]
    bool countsAsPlayerKill = true;


    public void Update()
    {
        if(timeBeforeEndOfMulti > 0)
        {
            timeBeforeEndOfMulti -= Time.unscaledDeltaTime;
            if(timeBeforeEndOfMulti <= 0)
            {
                timeBeforeEndOfMulti = 0;
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.EnvironmentKill, Vector3.zero);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IEntity otherEnemy = other.GetComponent<IEntity>();
        if (other.GetComponent<IEntity>() != null && other.GetComponent<Player>() == null && other.GetComponent<Prop>() == null)
        {
            
            if (killEnemyEqualsDamage)
            {
                if (other.GetComponent<Swarmer>() != null)
                    Player.Instance.TakeDamage(other.GetComponent<Swarmer>().GetDamage());
            }

            if (countsAsPlayerKill)
            {
                otherEnemy.TakeDamage(999999);
                timeBeforeEndOfMulti = 0.3f;
            }
            else
            {
                otherEnemy.ForceKill();
            }

        }
    }

}
