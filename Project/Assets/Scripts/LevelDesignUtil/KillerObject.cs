using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerObject : MonoBehaviour
{
    float timeBeforeEndOfMulti = 0;

    public void Update()
    {
        if(timeBeforeEndOfMulti > 0)
        {
            timeBeforeEndOfMulti -= Time.unscaledDeltaTime;
            if(timeBeforeEndOfMulti <= 0)
            {
                timeBeforeEndOfMulti = 0;
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.EnvironmentKill);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        IEntity otherEnemy = other.GetComponent<IEntity>();
        if (other.GetComponent<IEntity>() != null)
        {
            otherEnemy.TakeDamage(999999);
            timeBeforeEndOfMulti = 0.3f;
        }
    }

}
