﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
public class TriggerSender : MonoBehaviour
{
    [SerializeField]
    TriggerType typeTrigger = 0;

    [Tooltip("C'est un bug si ça s'affiche alors que c'est pas un type spawner, vivez avec <3")]
    [ShowIf("typeTrigger", TriggerType.Spawner), SerializeField]
    Spawner[] spawners = null;


    [ShowIf("typeTrigger", TriggerType.SlowMo), SerializeField]
    float slowMoPower = 0;
    [ShowIf("typeTrigger", TriggerType.SlowMo), SerializeField]
    float slowMoDuration = 0;


    [ShowIf("typeTrigger", TriggerType.Activation), SerializeField]
    bool isActivation = false;
    [ShowIf("typeTrigger", TriggerType.Activation), SerializeField]
    Activable affected = 0;


    [ShowIf("typeTrigger", TriggerType.Sound), SerializeField]
    string soundPlayed = "";

    [ShowIf("typeTrigger", TriggerType.Sound), SerializeField, Tooltip("ENTRE 0 ET 1 LE SON")]
    float volume = 1;


    [ShowIf("typeTrigger", TriggerType.Animator), SerializeField]
    Animator[] animated = null;


    [ShowIf("typeTrigger", TriggerType.Shake), SerializeField]
    float ShakeValue = 0;


    [ShowIf("typeTrigger", TriggerType.Boolean), SerializeField]
    string booleanName = "";
    [ShowIf("typeTrigger", TriggerType.Boolean), SerializeField]
    bool boolStateSet = true;


    [ShowIf("typeTrigger", TriggerType.EnemyFollow), SerializeField]
    float timeGoTo = .1f;
    [ShowIf("typeTrigger", TriggerType.EnemyFollow), SerializeField]
    float timeGoBack = .1f;
    [ShowIf("typeTrigger", TriggerType.EnemyFollow), SerializeField]
    bool lockedAfterTransition = false;
    [ShowIf("typeTrigger", TriggerType.EnemyFollow), SerializeField, DisableIf("lockedAfterTransition")]
    float followDuration = 0;


    [SerializeField]
    float timeBeforeStart = 0;

    bool timerStarted = false;
    Transform enemyFollow = null;

    void OnTriggerEnter(Collider other)
    {
        if(typeTrigger == TriggerType.EnemyFollow)
        {
            enemyFollow = other.transform;
        }
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
                TriggerUtil.TriggerSpawners(timeBeforeStart, spawners);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.SlowMo:
                TriggerUtil.TriggerSlowMo(timeBeforeStart, slowMoDuration, slowMoPower);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Activation:
                TriggerUtil.TriggerActivation(timeBeforeStart, affected, isActivation);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Sound:
                TriggerUtil.TriggerSound(timeBeforeStart, soundPlayed, volume);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Animator:
                TriggerUtil.TriggerAnimators(timeBeforeStart, animated);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Shake:
                TriggerUtil.TriggerShake(timeBeforeStart, ShakeValue);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Boolean:
                TriggerUtil.TriggerBooleanSequence(timeBeforeStart, booleanName, boolStateSet);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.EnemyFollow:
                TriggerUtil.TriggerFollowTarget(timeBeforeStart, enemyFollow, timeGoTo, timeGoBack, lockedAfterTransition, followDuration);
                break;
            default:
                break;
        }

    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        foreach (Spawner spawner in spawners)
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
        Boolean = 6,
        EnemyFollow = 7,
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
