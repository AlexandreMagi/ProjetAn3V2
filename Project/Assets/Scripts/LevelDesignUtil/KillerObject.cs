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


    [SerializeField]
    string soundToPlayAtStart = "Se_KillerObjectSound";
    [SerializeField]
    string soundToPlayAtKill = "Se_KillerObjectKillSound";

    AudioSource ambiantAudioSource = null;

    public void Start()
    {
        if (CustomSoundManager.Instance != null && soundToPlayAtStart != "") ambiantAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayAtStart, "Ambiant", transform, 2f, true);
        if (ambiantAudioSource != null) ambiantAudioSource.spatialBlend = 1;
    }

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

    void OnTriggerStay(Collider other)
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

    private void OnTriggerEnter(Collider other)
    {
        IEntity otherEnemy = other.GetComponent<IEntity>();
        if (other.GetComponent<IEntity>() != null && other.GetComponent<Player>() == null && other.GetComponent<Prop>() == null)
        {
            if (soundToPlayAtKill != "") 
            {
                AudioSource killAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayAtKill, "Ambiant", transform, 2f);
                if (killAudioSource!=null) killAudioSource.spatialBlend = 1;
            }
        }

    }

}
