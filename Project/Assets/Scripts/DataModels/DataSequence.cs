using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sequences/DataSequence")]
public class DataSequence : ScriptableObject
{

    // ############################################################################################## TRANSITIONS

    [Header("Transition vers la séquence")]

    [Tooltip("Nom de la virtual Camera sur laquel la séquence va finir")]
    public string camTargetName;
    [Tooltip("Courbe d'animation pour gerer la vitesse du blend en fonction du temps")]
    public CinemachineBlendDefinition.Style animationStyle;
    [Tooltip("Temps pour arriver à la caméra ciblée")]
    public float animationTime;



    // ############################################################################################## BOOLEAN SEQUENCES

    //BOOLEAN SEQUENCES
    [Header("Boolean sequence")]
    public bool isAffectedByBooleanSequence;

    [ShowIf("isAffectedByBooleanSequence")]
    public string booleanSequenceName;

    [ShowIf("isAffectedByBooleanSequence")]
    public bool booleanSequenceStateRequired;

    public bool cutsSlowMoOnEnd;

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


    // ############################################################################################## BALANCING

    [Title("Parametres balancement")]
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Distance en up de la ou la corde est attachée")]
    public float distanceToAnchor = 5;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Vitesse à l'init de balancement")]
    public float initImpulseBalancing = 30;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Damping en pourcentage")]
    public float dampingOnBalancing = 0.3f;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Offset de direction sur le balancing")]
    public float angleBalancing = 0;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Frequence de balancement")]
    public float balancingFrequency = 1;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), PropertyRange(0.001f, 360f), Tooltip("Vitesse minimal de rotation")]
    public float minSpeedRot = 0.1f;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), PropertyRange(0.001f, 50f), Tooltip("Temps de transition vers la nouvelle direction")]
    public float timeGoToNewRot = 0.1f;
    [SerializeField, ShowIf("seqEvent", SequenceEndEventType.Balancing), ShowIf("hasEventOnEnd"), Tooltip("Vitesse Lerp de retour sur la caméra")]
    public float returnLerpSpeedFromBalance = 5;

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


    [ShowIf("seqEvent", SequenceEndEventType.ArmorProgressiveGain), ShowIf("hasEventOnEnd"), Tooltip("Valeur d'armure que le joueur va gagner")]
    public float armorToGainOverTime;
    [ShowIf("seqEvent", SequenceEndEventType.ArmorProgressiveGain), ShowIf("hasEventOnEnd"), Tooltip("Vitesse de gain d'armure (en secondes)")]
    public float armorGainRate;



    // ##############################################################################################################################


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


    [Header("Chekpoint tutoriel")]
    public DataTutorialCheckpoint checkpointToUse = null;

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
        ArmorProgressiveGain = 5,
        Other = 9
    }

    [System.Serializable]
    public struct BooleanLink
    {
        public DataBooleanSequence booleanSequence;
        public bool bSeqRequiredStatus;
        public int indexOfBranchLinked;
    }




    [Space(50f)]

    // ############################################################################################## CAMERA
    [Title("ALL CAMERA PARAMETERS")]
    [InfoBox("Tout les paramètres qui ont une influence sur la caméra. Cela contient les feedbacks basiques, mais aussi les steps, le lookat, les animations customs et etc.")]

    [Tooltip("Permet de desactiver/activer toute sorte de pas")]
    public bool enableCamFeedback = true;
    [Tooltip("Permet d'avoir une transition entre feedbacks et sans feedbacks")]
    public bool enableCamTransition = false;
    [ShowIf("enableCamTransition"), Tooltip("Temps de transition entre feedbacks et sans feedbacks")]
    public float transitionTime = 2;

    [Header("Transition steps forcées")]

    [ShowIf("enableCamFeedback"), Tooltip("Permet de faire une transition fluide de frequence de pas au début")]
    public bool stepFadeAtStart = false;
    [ShowIf("stepFadeAtStart"), ShowIf("enableCamFeedback"), Tooltip("Distance de transition")]
    public float distFadeStart = 2;
    [ShowIf("enableCamFeedback"), Tooltip("Permet de faire une transition fluide de frequence de pas à la fin")]
    public bool stepFadeAtEnd = false;
    [ShowIf("stepFadeAtEnd"), ShowIf("enableCamFeedback"), Tooltip("Distance de transition")]
    public float distFadeEnd = 1;

    [Header("Respiration")]

    [ShowIf("enableCamFeedback"), Tooltip("Dis si la respiration est activée dans cette séquence")]
    public bool breathingEnabled = false;
    [ShowIf("enableCamFeedback"), Tooltip("Temps de transition vers l'état désiré")]
    public float timeFadeBreathing = 1;



    // ############################################################################################## NOISE

    [Header("Noise")]

    [Tooltip("Indique si les parametres du noises doivent changer")]
    public bool changeNoiseSettings = false;
    [Tooltip("Indique quel pourcentage de noise afficher 1=max, 0=rien"), ShowIf("changeNoiseSettings")]
    public float noisePurcentageAimed = 0;
    [Tooltip("Temps de transition vers le pourcentage visé (1/valeur) donc en vrai c'est plutot une vitesse mais np"), ShowIf("changeNoiseSettings")]
    public float timeTransitionNoise = 1;
    [Tooltip("Amplitude du noise en position"), ShowIf("changeNoiseSettings")]
    public float noiseAmplitudePos = 0.1f;
    [Tooltip("Amplitude du noise en rotation"), ShowIf("changeNoiseSettings")]
    public float noiseAmplitudeRot = 0.5f;
    [Tooltip("Frequence du noise"), ShowIf("changeNoiseSettings")]
    public float noiseFrequency = 3;



    // ############################################################################################## STEPS

    [Header("Steps")]

    // --- Short Step
    [Tooltip("Dis si la transition est un Short Step, permet d'avoir sa propre courbe de pas")]
    public bool isShortStep = false;
    [ShowIf("isShortStep"), Tooltip("Courbe de pas custom")]
    public AnimationCurve shortStepCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [ShowIf("isShortStep"), Tooltip("Multiplicateur d'amplitude pour la courbe")]
    public float shortStepAmplitude = -1;

    // --- Normal Step
    [HideIf("isShortStep"), Tooltip("Permet de changer les parametres des steps par défault")]
    public bool modifySteps = false;
    [ShowIf("modifySteps"), Tooltip("Setup la fréquence des pas en override")]
    public float modifierFrequenceCamStep = 1;
    [ShowIf("modifySteps"), Tooltip("Permet de gerer l'intensité de la fréquence des pas sur le temps de la séquence")]
    public bool modifyStepsCurve = false;
    [ShowIf("modifySteps"), ShowIf("modifyStepsCurve"), Tooltip("Courbe d'intensité de frequence sur le temps de la séquence")]
    public AnimationCurve modifiedStepCurve = null;



    // ############################################################################################## ANIMATIONS CUSTOMS

    [Header("Animated Cam")]

    [Tooltip("Si anim dans cette variable, la caméra va jouer l'anim au lieu de faire une transition via cinémachine")]
    public AnimationClip animToPlay = null;



    // ############################################################################################## LOOK AT

    [Header("Look At")]

    [Tooltip("Objet que la caméra va lookat")]
    public Transform lookAtObject = null;
    [ShowIf("lookAtObject", null), Tooltip("Temps de transition vers le lookt at")]
    public float transitionToTime = 1;
    [ShowIf("lookAtObject", null), Tooltip("Temps de transition vers la rotation normale apres le lookat")]
    public float transitionBackTime = 1;
    [ShowIf("lookAtObject", null), Tooltip("Temps durant lequel la caméra reste en lookat")]
    public float lookAtTime = 1;
    [ShowIf("lookAtObject", null), Tooltip("Dis si le lookat doit être cut à la fin de la séquence")]
    public bool cutLookAtOnEndOfSequence = false;
    [ShowIf("lookAtObject", null), PropertyRange(0f, 1f), Tooltip("Pourcentage d'application du lookat sur la caméra")]
    public float weightLookAt = 1;
    [ShowIf("lookAtObject", null), PropertyRange(0f, 1f), Tooltip("Pourcentage de rotation que le lookat empeche")]
    public float weightRemoveRotLookAt = 1;



    // ############################################################################################## WAIT SCREEN

    [Header("WaitScreen")]

    [Tooltip("Change l'état de wait screen")]
    public bool changeWaitScreen = false;
    [ShowIf("changeWaitScreen"), Tooltip("État visé par le wait screen")]
    public bool activateWaitScreen = false;
}
