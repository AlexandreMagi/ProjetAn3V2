using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerObject : MonoBehaviour
{
    float timeBeforeEndOfMulti = 0;

    [SerializeField]
    bool killEnemyEqualsDamage = false;

    [SerializeField]
    bool countsAsPlayerKill = true;

    [SerializeField]
    float shakeForceAtVictim = 0;
    [SerializeField]
    float shakeDurationAtVictim = 0;

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
                if (CameraHandler.Instance != null && shakeForceAtVictim > 0 && shakeDurationAtVictim > 0) CameraHandler.Instance.AddShake(shakeForceAtVictim, shakeDurationAtVictim);
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.EnvironmentalKill);
            }
            else
            {
                otherEnemy.ForceKill();
            }

        }
    }

}
