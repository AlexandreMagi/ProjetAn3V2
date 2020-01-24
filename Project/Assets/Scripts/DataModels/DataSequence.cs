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

    public bool hasEventOnEnd;

    public bool cutsSlowMoOnEnd;

    public float modifierFrequenceCamStep = 1;

    public string animToPlay = "";
    public float animTime = 1;

    //BOOLEAN SEQUENCES
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

    //TIMER SETTINGS
    [ShowIf("sequenceType", SequenceType.Timer)]
    public float timeSequenceDuration;

    [SerializeField]
    public float timeBeforeStart = 0;

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

    [Header("Camera")]
    public bool enableCamFeedback = true;
    public bool enableCamTransition = false;
    public float speedTransition = 2;


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
        Other = 9
    }
}
