using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sequences/DataSequence")]
public class DataSequence : ScriptableObject
{
    [Header("Transition vers la séquence")]
    public string vCamTargetName;

    public CinemachineBlendDefinition.Style animationStyle;

    public float fAnimationTime;

    public bool hasEventOnEnd;

    public bool cutsSlowMoOnEnd;

    [Header("Séquence paramètres")]
    [EnumToggleButtons]
    public SequenceType sequenceType;

    //ENEMIES SETTINGS
    [ShowIf("sequenceType", SequenceType.KillEnnemies)]
    public int nEnemiesToKillInSequence;

    [ShowIf("sequenceType", SequenceType.KillEnnemies)]
    public bool bAcceptsBufferKill;

    [ShowIf("sequenceType", SequenceType.KillEnnemies)]
    public float nTimeBeforeNextSequenceOnKills;

    //TIMER SETTINGS
    [ShowIf("sequenceType", SequenceType.Timer)]
    public float fTimeSequenceDuration;

    [SerializeField]
    public float tTimeBeforeStart = 0;

    [ShowIf("hasEventOnEnd")]
    public SequenceEndEventType seqEvent;

    [ShowIf("hasEventOnEnd")]
    public float tTimeBeforeEvent;


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
    public bool bEnableCamFeedback = true;
    public bool bEnableCamTransition = false;
    public float fSpeedTransition = 2;


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
