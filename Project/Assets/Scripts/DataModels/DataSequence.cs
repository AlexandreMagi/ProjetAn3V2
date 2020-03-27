using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sequences/DataSequence")]
public class DataSequence : ScriptableObject
{
    [Header("Transition vers la séquence")]
    public string camTargetName;

    public CinemachineBlendDefinition.Style animationStyle;

    public float animationTime;

    public bool cutsSlowMoOnEnd;

    [Header("Steps")]

    public bool isShortStep = false;
    [ShowIf("isShortStep")]
    public AnimationCurve shortStepCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [ShowIf("isShortStep")]
    public float shortStepAmplitude = -1;

    [HideIf("isShortStep")]
    public bool modifySteps = false;
    [ShowIf("modifySteps")]
    public float modifierFrequenceCamStep = 1;
    [ShowIf("modifySteps")]
    public bool modifyStepsCurve = false;
    [ShowIf("modifySteps"), ShowIf("modifyStepsCurve")]
    public AnimationCurve modifiedStepCurve = null;

    [Header("Animated Cam")]
    public AnimationClip animToPlay = null;

    [Header("Look At")]
    public Transform lookAtObject = null;
    [ShowIf("lookAtObject", null)]
    public float transitionToTime = 1;
    [ShowIf("lookAtObject", null)]
    public float transitionBackTime = 1;
    [ShowIf("lookAtObject", null)]
    public float lookAtTime = 1;
    [ShowIf("lookAtObject", null)]
    public bool cutLookAtOnEndOfSequence = false;
    [ShowIf("lookAtObject", null), PropertyRange(0f,1f)]
    public float weightLookAt = 1;
    [ShowIf("lookAtObject", null), PropertyRange(0f,1f)]
    public float weightRemoveRotLookAt = 1;

    //BOOLEAN SEQUENCES
    [Header("Boolean sequence")]
    public bool isAffectedByBooleanSequence;

    [ShowIf("isAffectedByBooleanSequence")]
    public string booleanSequenceName;

    [ShowIf("isAffectedByBooleanSequence")]
    public bool booleanSequenceStateRequired;


    //TYPES
    [Header("Séquence paramètres")]

    [EnumToggleButtons]
    public SequenceType sequenceType;

    //ENEMIES SETTINGS
    [ShowIf("sequenceType", SequenceType.KillEnnemies)]
    public int enemiesToKillInSequence;

    [ShowIf("sequenceType", SequenceType.KillEnnemies)]
    public bool acceptsBufferKill;

    [ShowIf("sequenceType", SequenceType.KillEnnemies)]
    public float timeBeforeNextSequenceOnKills;

    [ShowIf("sequenceType", SequenceType.KillEnnemies)]
    public bool activatesSpawnersDuringSequence = false;

    [ShowIf("sequenceType", SequenceType.KillEnnemies), ShowIf("activatesSpawnersDuringSequence")]
    public float numberOfKillsRequiredToPop = 0;

    [ShowIf("sequenceType", SequenceType.KillEnnemies), ShowIf("activatesSpawnersDuringSequence")]
    public Spawner[] spawnersToTrigger = null;

    [ShowIf("sequenceType", SequenceType.KillEnnemies), ShowIf("activatesSpawnersDuringSequence")]
    public float delayOnTrigger = 0;


    //TIMER SETTINGS
    [ShowIf("sequenceType", SequenceType.Timer)]
    public float timeSequenceDuration;

    //[SerializeField]
    //public float timeBeforeStart = 0;

    [Header("Event on end")]
    public bool hasEventOnEnd;

    [ShowIf("hasEventOnEnd")]
    public SequenceEndEventType seqEvent;

    [ShowIf("hasEventOnEnd")]
    public float timeBeforeEvent;


    [ShowIf("seqEvent", SequenceEndEventType.SlowMo), ShowIf("hasEventOnEnd")]
    public float slowMoPower = 0;
    [ShowIf("seqEvent", SequenceEndEventType.SlowMo), ShowIf("hasEventOnEnd")]
    public float slowMoDuration = 0;

    [ShowIf("seqEvent", SequenceEndEventType.Activation), ShowIf("hasEventOnEnd")]
    public bool isActivation = false;
    
    [ShowIf("seqEvent", SequenceEndEventType.Activation), ShowIf("hasEventOnEnd")]
    public TriggerSender.Activable affected = 0;
    

    [ShowIf("seqEvent", SequenceEndEventType.Animation), ShowIf("hasEventOnEnd")]
    public string[] tagsAnimated;


    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Sound), ShowIf("hasEventOnEnd")]
    public string soundPlayed = "";

    [Tooltip("ENTRE 0 ET 1 SINON LE SON VA VOUS SOULEVER A L'ITALIENNE")]
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Sound), ShowIf("hasEventOnEnd")]
    public float volume = 1;

    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Distance en up de la ou la corde est attachée")]
    public float distanceToAnchor = 5;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Vitesse à l'init de balancement")]
    public float initImpulseBalancing = 30;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip ("Damping en pourcentage")]
    public float dampingOnBalancing = 0.3f;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Offset de direction sur le balancing")]
    public float angleBalancing = 0;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Frequence de balancement")]
    public float balancingFrequency = 1;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), PropertyRange(0.001f, 360f), Tooltip("Vitesse minimal de rotation")]
    public float minSpeedRot = 0;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), PropertyRange(0.001f,50f), Tooltip("Temps de transition vers la nouvelle direction")]
    public float timeGoToNewRot = 0;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Vitesse Lerp de retour sur la caméra")]
    public float returnLerpSpeedFromBalance = 5;

    [Header("Camera")]
    public bool enableCamFeedback = true;
    public bool enableCamTransition = false;
    [ShowIf("enableCamTransition")]
    public float transitionTime = 2;

    [ShowIf("enableCamFeedback")]
    public bool stepFadeAtStart = false;
    [ShowIf("stepFadeAtStart"), ShowIf("enableCamFeedback")]
    public float distFadeStart = 2;
    [ShowIf("enableCamFeedback")]
    public bool stepFadeAtEnd = false;
    [ShowIf("stepFadeAtEnd"), ShowIf("enableCamFeedback")]
    public float distFadeEnd = 1;

    [ShowIf("enableCamFeedback")]
    public bool breathingEnabled = false;
    [ShowIf("enableCamFeedback")]
    public float timeFadeBreathing = 1;


    //SEQUENCE BRANCHES
    [Header("Sequence branches")]
    [SerializeField]
    public bool skipsToBranchOnEnd = false;

    [SerializeField, ShowIf("skipsToBranchOnEnd")]
    public bool affectedByBooleanSequenceBranch = false;

    [SerializeField, ShowIf("skipsToBranchOnEnd"), HideIf("affectedByBooleanSequenceBranch")]
    public int branchLinkedId = 0;

    [SerializeField, ShowIf("skipsToBranchOnEnd"), ShowIf("affectedByBooleanSequenceBranch")]
    public List<BooleanLink> booleanLinks = null;


    //OBJECT AFFECT
    [Header("AffectObject")]
    public GameObject affectedObject = null;
    public enum gameObjectActionType { Activate, MoveTo }
    [ShowIf("affectedObject", null)]
    public gameObjectActionType actionType = gameObjectActionType.Activate;
    [ShowIf("affectedObject", null)]
    public float delayOnAction = 0;
    [ShowIf("affectedObject", null), ShowIf("actionType", gameObjectActionType.Activate)]
    public bool _active = true;
    [ShowIf("affectedObject", null), ShowIf("actionType", gameObjectActionType.MoveTo)]
    public Vector3 positionMoveTo = Vector3.zero;

    public enum SequenceType
    {
        KillEnnemies = 0,
        Timer = 1,
        ManualTrigger = 2
    }

    public enum SequenceEndEventType
    {
        SlowMo = 0,
        Activation = 1,
        Animation = 2,
        Sound = 3,
        Balancing = 4,
        Other = 9
    }

    [System.Serializable]
    public struct BooleanLink
    {
        public DataBooleanSequence booleanSequence;
        public bool bSeqRequiredStatus;
        public int indexOfBranchLinked;
    }
}
