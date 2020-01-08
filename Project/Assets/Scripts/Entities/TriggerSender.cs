using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShowWhenAttribute;

public class TriggerSender : MonoBehaviour
{
    [SerializeField]
    TriggerType typeTrigger = 0;

    [Tooltip("C'est un bug si ça s'affiche alors que c'est pas un type spawner, vivez avec <3")]
    [ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Spawner), SerializeField]
    Spawner[] spawners = null;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.SlowMo)]
    float slowMoPower = 0;
    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.SlowMo)]
    float slowMoDuration = 0;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Activation)]
    bool isActivation = false;
    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Activation)]
    Activable affected = 0;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Sound)]
    string soundPlayed = "";

    [Tooltip("ENTRE 0 ET 1 LE SON")]
    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Sound)]
    float volume = 1;

    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Animator)]
    Animator[] animated = null;

    [SerializeField]
    float tTimeBeforeStart = 0;

    bool timerStarted = false;


    [SerializeField, ShowWhen("typeTrigger", Condition.Equals, (int)TriggerType.Shake)]
    float ShakeValue = 0;

    void OnTriggerStay(Collider other)
    {
        StartTrigger();
    }

    void StartTrigger()
    {
        if (!timerStarted)
        {
            timerStarted = true;
            DoTrigger();

        }

    }

    void DoTrigger()
    {
        switch (typeTrigger)
        {
            case TriggerType.Spawner:
                TriggerUtil.TriggerSpawners(tTimeBeforeStart, spawners);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.SlowMo:
                TriggerUtil.TriggerSlowMo(tTimeBeforeStart, slowMoDuration, slowMoPower);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Activation:
                TriggerUtil.TriggerActivation(tTimeBeforeStart, affected, isActivation);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Sound:
                TriggerUtil.TriggerSound(tTimeBeforeStart, soundPlayed, volume);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Animator:
                TriggerUtil.TriggerAnimators(tTimeBeforeStart, animated);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Shake:
                TriggerUtil.TriggerShake(tTimeBeforeStart, ShakeValue);
                this.gameObject.SetActive(false);
                break;

            default:
                break;
        }

    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        foreach (C_SpawnerTrigger spawner in spawners)
        {
            Gizmos.DrawLine(this.transform.position, spawner.transform.position);
        }

    }

    public enum TriggerType
    {
        Spawner = 0,
        Animator = 1,
        SlowMo = 2,
        Activation = 3,
        Sound = 4,
        Shake = 5,
        Other = 9
    }

    public enum Activable
    {
        BaseWeapon = 0,
        Orb = 1,
        Both = 2,
        Other = 9
    }
}
