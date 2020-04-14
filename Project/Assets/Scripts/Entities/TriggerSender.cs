using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;
using UnityEngine.Video;

public class TriggerSender : MonoBehaviour
{
    [SerializeField]
    TriggerType typeTrigger = 0;

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
    [ShowIf("typeTrigger", TriggerType.Animator), SerializeField]
    bool usesTimerBetweenAllAnims = false;

    [ShowIf("typeTrigger", TriggerType.Shake), SerializeField]
    float ShakeValue = 0;
    [ShowIf("typeTrigger", TriggerType.Shake), SerializeField]
    float ShakeDuration = 0;


    [ShowIf("typeTrigger", TriggerType.Boolean), SerializeField]
    string booleanName = "";
    [ShowIf("typeTrigger", TriggerType.Boolean), SerializeField]
    bool boolStateSet = true;


    [ShowIf("typeTrigger", TriggerType.EnemyFollow), SerializeField]
    float timeGoTo = .1f;
    [ShowIf("typeTrigger", TriggerType.EnemyFollow), SerializeField]
    float timeGoBack = .1f;
    [ShowIf("typeTrigger", TriggerType.EnemyFollow), SerializeField]
    float followDuration = 0;


    [ShowIf("typeTrigger", TriggerType.Light), SerializeField]
    Light[] lightsToAffect = null;
    [ShowIf("typeTrigger", TriggerType.Light), SerializeField]
    bool lightsState = false;


    [ShowIf("typeTrigger", TriggerType.VFX), SerializeField]
    ParticleSystem[] VFXToAffect = null;
    [ShowIf("typeTrigger", TriggerType.VFX), SerializeField]
    bool VFXState = false;

    //, ShowIf("typeTrigger", TriggerType.OutlineShader), ShowIf("typeTrigger", TriggerType.RevealLight), SerializeField]
    [ShowIf("typeTrigger", TriggerType.PlaneShake), SerializeField]
    bool activatePlaneShake = false;
    [ShowIf("typeTrigger", TriggerType.OutlineShader), SerializeField]
    bool activateOutlineShader = false;
    [ShowIf("typeTrigger", TriggerType.RevealLight), SerializeField]
    bool activateRevealLight = false;

    [ShowIf("typeTrigger", TriggerType.VideoTrigger), SerializeField]
    List<VideoPlayer> players = null;
    [ShowIf("typeTrigger", TriggerType.VideoTrigger), SerializeField]
    bool isPlay;
    [ShowIf("typeTrigger", TriggerType.VideoTrigger), SerializeField]
    bool isLoop;

    [ShowIf("typeTrigger", TriggerType.WhiteScreenEffect), SerializeField]
    float scaleMin = 0;
    [ShowIf("typeTrigger", TriggerType.WhiteScreenEffect), SerializeField]
    float scaleMax = 6;
    [ShowIf("typeTrigger", TriggerType.WhiteScreenEffect), SerializeField]
    float timeToScaleMax = 3;
    [ShowIf("typeTrigger", TriggerType.WhiteScreenEffect), SerializeField]
    float timeStayAtMax = 1;
    [ShowIf("typeTrigger", TriggerType.WhiteScreenEffect), SerializeField]
    float timeFadeAlpha = 2;
    [ShowIf("typeTrigger", TriggerType.WhiteScreenEffect), SerializeField]
    bool independentFromTimeScale = false;

    [Header("Swarmer activation")]
    [SerializeField]
    bool activatesAllAnimatingSwarmersOnTrigger = false;

    [SerializeField]
    List<Swarmer> swarmersToAlert = null;

    [SerializeField]
    float timeBeforeStart = 0;

    bool timerStarted = false;
    Transform enemyFollow = null;

    void OnTriggerEnter(Collider other)
    {
        if(typeTrigger == TriggerType.EnemyFollow && other.gameObject.layer == LayerMask.NameToLayer ("Camera"))
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
                TriggerUtil.TriggerAnimators(timeBeforeStart, animated, usesTimerBetweenAllAnims);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Shake:
                TriggerUtil.TriggerShake(timeBeforeStart, ShakeValue, ShakeDuration);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Boolean:
                TriggerUtil.TriggerBooleanSequence(timeBeforeStart, booleanName, boolStateSet);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.EnemyFollow:
                TriggerUtil.TriggerFollowTarget(timeBeforeStart, enemyFollow, timeGoTo, timeGoBack, followDuration);
                break;

            case TriggerType.Light:
                foreach(Light l in lightsToAffect)
                {
                    l.enabled = false;
                    LightHandler lh = l.GetComponent<LightHandler>();
                    if (lh)
                    {
                        lh.enabled = lightsState;
                    }
                }
                break;
            case TriggerType.VFX:
                TriggerUtil.TriggerVFX(timeBeforeStart, VFXToAffect, VFXState);
                break;
            case TriggerType.PlaneShake:
                TriggerUtil.TriggerPlaneShake(activatePlaneShake, timeBeforeStart);
                break;
            case TriggerType.OutlineShader:
                TriggerUtil.TriggerOutline(activateOutlineShader, timeBeforeStart);
                break;
            case TriggerType.RevealLight:
                TriggerUtil.TriggerRevealLight(activateRevealLight, timeBeforeStart);
                break;
            case TriggerType.WhiteScreenEffect:
                TriggerUtil.TriggerWhiteScreenEffect(timeBeforeStart, scaleMin, scaleMax, timeToScaleMax, timeStayAtMax, timeFadeAlpha, independentFromTimeScale);
                break;
            case TriggerType.VideoTrigger:
                TriggerUtil.TriggerVideo(timeBeforeStart, players, isPlay, isLoop);
                break;
            default:
                break;
        }

        if (activatesAllAnimatingSwarmersOnTrigger)
        {
            //Swarmer[] activeSwarmers = FindObjectsOfType<Swarmer>();

            foreach (Swarmer swarm in swarmersToAlert)
            {
                swarm.OnManualActivation();
            }
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
        Light = 8,
        Other = 9,
        VFX = 10,
        PlaneShake = 11,
        OutlineShader = 12,
        RevealLight = 13,
        WhiteScreenEffect = 14,
        VideoTrigger = 15
    }

    public enum Activable
    {
        BaseWeapon = 0,
        Orb = 1,
        Both = 2,
        Reload = 3,
        AutoReload = 4,
        Other = 9
    }
}
