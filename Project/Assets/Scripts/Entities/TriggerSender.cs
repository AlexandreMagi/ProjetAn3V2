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
    [ShowIf("typeTrigger", TriggerType.Spawner), SerializeField]
    bool spawnerState = true;


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
    [ShowIf("typeTrigger", TriggerType.Sound), SerializeField]
    string soundMixer = "Effect";
    [ShowIf("typeTrigger", TriggerType.Sound), SerializeField]
    bool soundLoop = false;
    [ShowIf("typeTrigger", TriggerType.Sound), SerializeField, Tooltip("ENTRE 0 ET 1 LE SON")]
    float volume = 1;

    
    [ShowIf("typeTrigger", TriggerType.PublicVolume), SerializeField, Tooltip("ENTRE 0 ET 1 LE SON")]
    float volumeAimed = 1;
    [ShowIf("typeTrigger", TriggerType.PublicVolume), SerializeField]
    float volumeTimeTransition = 1;


    [ShowIf("typeTrigger", TriggerType.Animator), SerializeField]
    Animator[] animated = null;
    [ShowIf("typeTrigger", TriggerType.Animator), SerializeField]
    bool usesTimerBetweenAllAnims = false;
    [ShowIf("typeTrigger", TriggerType.Animator), SerializeField]
    float animationWaitTimer = 0;
    [ShowIf("typeTrigger", TriggerType.Animator), SerializeField]
    bool isMeshReplacer = false;
    [ShowIf("typeTrigger", TriggerType.Animator), ShowIf("isMeshReplacer"), SerializeField]
    Mesh meshForTheCollider = null;
    [ShowIf("typeTrigger", TriggerType.Animator), ShowIf("isMeshReplacer"), SerializeField]
    MeshCollider colliderToReplace = null;
    [ShowIf("typeTrigger", TriggerType.Animator), SerializeField]
    bool isMultipleAnimationTrigger = false;
    [ShowIf("typeTrigger", TriggerType.Animator), ShowIf("isMultipleAnimationTrigger"), SerializeField]
    AnimationClip[] animatonClipsToUse = null;
    [ShowIf("typeTrigger", TriggerType.Animator), ShowIf("isMultipleAnimationTrigger"), Tooltip("Le 0e délai est timeBeforeStart. Le 1er délai est entre l'anim 1 et 2"), SerializeField]
    float[] delaysBetweenTriggers = null;

    [ShowIf("typeTrigger", TriggerType.Parenting), SerializeField]
    Transform _child = null;
    [ShowIf("typeTrigger", TriggerType.Parenting), SerializeField]
    Transform _parent = null;

    [ShowIf("typeTrigger", TriggerType.ChangeSwarmerGravity), SerializeField]
    Swarmer[] swarmerAffectedByGravityChange = null;
    [ShowIf("typeTrigger", TriggerType.ChangeSwarmerGravity), SerializeField]
    bool mustIgnoreGravity = false;


    [ShowIf("typeTrigger", TriggerType.Shake), SerializeField]
    bool isStopShake = false;
    [ShowIf("typeTrigger", TriggerType.Shake), HideIf("isStopShake"), SerializeField]
    float ShakeValue = 0;
    [ShowIf("typeTrigger", TriggerType.Shake), HideIf("isStopShake"), SerializeField]
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
    [ShowIf("typeTrigger", TriggerType.Light), SerializeField]
    bool lightChangesColor = false;
    [ShowIf("typeTrigger", TriggerType.Light), ShowIf("lightChangesColor"), SerializeField]
    Color colorOfLight = Color.white;

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
    bool isPlay = false;
    [ShowIf("typeTrigger", TriggerType.VideoTrigger), SerializeField]
    bool isLoop = false;


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


    [ShowIf("typeTrigger", TriggerType.GameObjectActivation), SerializeField]
    bool isActivationGameObject = false;
    [ShowIf("typeTrigger", TriggerType.GameObjectActivation), SerializeField]
    List<GameObject> objectsToChange = null;
    [ShowIf("typeTrigger", TriggerType.GameObjectActivation)]
    public bool triggeredBySkip = false;


    [ShowIf("typeTrigger", TriggerType.Damage), SerializeField]
    int damages = 0;
    [ShowIf("typeTrigger", TriggerType.Damage), SerializeField]
    bool ignoreDamageEvent = false;

    [ShowIf("typeTrigger", TriggerType.InstantKill), SerializeField]
    bool preventRevive = true;

    [ShowIf("typeTrigger", TriggerType.SwarmerAnimation), SerializeField]
    SwarmerProceduralAnimation.AnimSwarmer animationToCall = SwarmerProceduralAnimation.AnimSwarmer.reset;

    [ShowIf("typeTrigger", TriggerType.SwarmerAnimation), SerializeField]
    List<Swarmer> swarmersToAnimate = null;


    [ShowIf("typeTrigger", TriggerType.Value), SerializeField]
    float valueStart = 0;
    [ShowIf("typeTrigger", TriggerType.Value), SerializeField]
    float valueEnd = 0;
    [ShowIf("typeTrigger", TriggerType.Value), SerializeField]
    float valueTransitionDuration = 0;
    [ShowIf("typeTrigger", TriggerType.Value), SerializeField]
    List<Renderer> meshAffecteds = null;
    [ShowIf("typeTrigger", TriggerType.Value), SerializeField]
    string shaderValueName = "";
    [ShowIf("typeTrigger", TriggerType.Value), SerializeField]
    bool isSwarmer = false;

    [ShowIf("typeTrigger", TriggerType.NearClipChanger), SerializeField]
    Camera cameraToChange = null;
    [ShowIf("typeTrigger", TriggerType.NearClipChanger), SerializeField]
    float newNearClip = 10;

    [ShowIf("typeTrigger", TriggerType.Fog), SerializeField]
    float fogEndValueAimed = 0;
    [ShowIf("typeTrigger", TriggerType.Fog), SerializeField]
    float fogTimeTransition = 1;
    [ShowIf("typeTrigger", TriggerType.Fog), SerializeField]
    bool overrideFogColor = false;
    [ShowIf("typeTrigger", TriggerType.Fog), ShowIf("overrideFogColor"), SerializeField]
    Color fogColorAimed = Color.white;
    [ShowIf("typeTrigger", TriggerType.Fog), SerializeField]
    float fogColorTimeTransition = 1;

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

    public void ActivateGOAtSkip()
    {
        if (typeTrigger == TriggerType.GameObjectActivation && isActivationGameObject) TriggerUtil.TriggerGameObjectActivation(0, objectsToChange, isActivationGameObject);
    }

    void DoTrigger()
    {
        switch (typeTrigger)
        {
            case TriggerType.Spawner:
                TriggerUtil.TriggerSpawners(timeBeforeStart, spawners, spawnerState);
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
                TriggerUtil.TriggerSound(timeBeforeStart, soundPlayed, soundMixer, volume, soundLoop);
                this.gameObject.SetActive(false);
                break;

            case TriggerType.Animator:
                if (isMultipleAnimationTrigger) TriggerUtil.TriggerAnimators(timeBeforeStart, animated, animatonClipsToUse, delaysBetweenTriggers);
                else TriggerUtil.TriggerAnimators(timeBeforeStart, animated, usesTimerBetweenAllAnims, animationWaitTimer);
                if (isMeshReplacer && colliderToReplace != null && meshForTheCollider != null)
                {
                    colliderToReplace.sharedMesh = meshForTheCollider;
                }

                this.gameObject.SetActive(false);
                break;

            case TriggerType.Shake:
                TriggerUtil.TriggerShake(timeBeforeStart, ShakeValue, ShakeDuration, isStopShake);
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
                TriggerUtil.TriggerLights(timeBeforeStart, lightsToAffect, lightsState, lightChangesColor, colorOfLight);
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
            case TriggerType.GameObjectActivation:
                TriggerUtil.TriggerGameObjectActivation(timeBeforeStart, objectsToChange, isActivationGameObject);
                break;
            case TriggerType.Damage:
                TriggerUtil.TriggerDamage(timeBeforeStart, damages, ignoreDamageEvent);
                break;
            case TriggerType.SwarmerAnimation:
                TriggerUtil.TriggerAnimationOnSwarmers(timeBeforeStart, animationToCall, swarmersToAnimate);
                break;
            case TriggerType.Value:
                TriggerUtil.TriggerValue(timeBeforeStart, valueStart, valueEnd, valueTransitionDuration, meshAffecteds, shaderValueName, isSwarmer);
                break;
            case TriggerType.NearClipChanger:
                TriggerUtil.TriggerNearClipChange(timeBeforeStart, cameraToChange, newNearClip);
                break;
            case TriggerType.Other:
                break;
            case TriggerType.PublicVolume:
                TriggerUtil.TriggerPublicSound(timeBeforeStart, volumeAimed, volumeTimeTransition);
                this.gameObject.SetActive(false);
                break;
            case TriggerType.Fog:
                TriggerUtil.TriggerFog(timeBeforeStart, fogEndValueAimed, fogTimeTransition, overrideFogColor, fogColorAimed, fogColorTimeTransition);
                break;
            case TriggerType.GameEnder:
                TriggerUtil.TriggerEndOfGame(timeBeforeStart);
                break;
            case TriggerType.InstantKill:
                TriggerUtil.TriggerInstantKill(timeBeforeStart, preventRevive);
                break;
            case TriggerType.Parenting:
                TriggerUtil.TriggerParenting(timeBeforeStart, _child, _parent);
                break;
            case TriggerType.ChangeSwarmerGravity:
                TriggerUtil.TriggerChangeSwarmerGravity(timeBeforeStart, swarmerAffectedByGravityChange, mustIgnoreGravity);
                break;
            default:
                break;
        }

        if (activatesAllAnimatingSwarmersOnTrigger)
        {
            //Swarmer[] activeSwarmers = FindObjectsOfType<Swarmer>();
            TriggerUtil.TriggerSwarmers(timeBeforeStart, swarmersToAlert);
        }
    }

    void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        foreach (Spawner spawner in spawners)
        {
            if (spawner!=null)
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
        VideoTrigger = 15,
        GameObjectActivation = 16,
        Damage = 17,
        SwarmerAnimation = 18,
        Value = 19,
        NearClipChanger = 20,
        PublicVolume = 21,
        Fog = 22,
        GameEnder = 23,
        InstantKill = 24,
        Parenting = 25,
        ChangeSwarmerGravity = 26
    }

    public enum Activable
    {
        BaseWeapon = 0,
        Orb = 1,
        Both = 2,
        Reload = 3,
        AutoReload = 4,
        PerfectReload = 5,
        Shotgun = 6,
        ZeroG = 7,
        Other = 9
    }
}
