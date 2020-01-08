using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
using static ShowWhenAttribute;

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
    public SequenceType sequenceType;

    //ENEMIES SETTINGS
    [ShowWhen("sequenceType", Condition.Equals, 0)]
    public int nEnemiesToKillInSequence;

    [ShowWhen("sequenceType", Condition.Equals, 0)]
    public bool bAcceptsBufferKill;

    [ShowWhen("sequenceType", Condition.Equals, 0)]
    public float nTimeBeforeNextSequenceOnKills;

    //TIMER SETTINGS
    [ShowWhen("sequenceType", Condition.Equals, 1)]
    public float fTimeSequenceDuration;

    [SerializeField]
    public float tTimeBeforeStart = 0;

    [ShowWhen("hasEventOnEnd")]
    public SequenceEndEventType seqEvent;

    [ShowWhen("hasEventOnEnd")]
    public float tTimeBeforeEvent;


    [ShowWhen("seqEvent", Condition.Equals, (int)SequenceEndEventType.SlowMo)]
    public float slowMoPower = 0;
    [ShowWhen("seqEvent", Condition.Equals, (int)SequenceEndEventType.SlowMo)]
    public float slowMoDuration = 0;

    [ShowWhen("seqEvent", Condition.Equals, (int)SequenceEndEventType.Activation)]
    public bool isActivation = false;
    
    [ShowWhen("seqEvent", Condition.Equals, (int)SequenceEndEventType.Activation)]
    public C_TriggerSender.Activable affected = 0;
    

    [ShowWhen("seqEvent", Condition.Equals, (int)SequenceEndEventType.Animation)]
    public string[] tagsAnimated;


    [SerializeField, ShowWhen("seqEvent", Condition.Equals, (int)SequenceEndEventType.Sound)]
    public string soundPlayed = "";

    [Tooltip("ENTRE 0 ET 1 SINON LE SON VA VOUS SOULEVER A L'ITALIENNE")]
    [SerializeField, ShowWhen("seqEvent", Condition.Equals, (int)SequenceEndEventType.Sound)]
    public float volume = 1;

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
