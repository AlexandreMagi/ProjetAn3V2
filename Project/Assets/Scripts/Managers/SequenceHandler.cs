using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class SequenceHandler : MonoBehaviour
{
    [HideInInspector, ListDrawerSettings(NumberOfItemsPerPage = 50)]
    List<DataSequence> sequences = null;

    [SerializeField]
    List<SequenceBranch> sequenceBranches = null;

    DataSequence currentSequence = null;
    SequenceBranch currentBranch = null;

    Camera cameraObj = null;
    CinemachineVirtualCamera currentVirtualCamera = null;

    CinemachineBlenderSettings blenderSettings;

    CinemachineBrain cameraBrain = null;

    [SerializeField]
    Collider cameraCollider;

    float elapsedTime = 0;
    float delayOnBlendSequence = 0;
    int sequenceIndex = 0;
    int branchIndex = 0;

    uint enemiesKilled = 0;
    uint bufferedKills = 0;

    bool readSequences = true;

    [HideInInspector]
    public bool isWaitingTimer = false;

    Vector3 pastCamPos = Vector3.one;
    Vector3 newCamPos = Vector3.one;

    bool isSequenceTrigger = false;

    //CinemachineVirtualCamera StockPreviousCam = null;

    void Start()
    {
        Instance = this;

        cameraObj = Camera.main;

        cameraBrain = GameObject.FindObjectOfType<CinemachineBrain>();

        currentBranch = sequenceBranches[0];

        currentSequence = currentBranch.GetDataSequenceAt(0);

        if (currentSequence.changeWaitScreen)
        {
            if (!currentSequence.activateWaitScreen) UiCrossHair.Instance.StopWaitFunction();
            else UiCrossHair.Instance.WaitFunction();
        }

        blenderSettings = ScriptableObject.CreateInstance("CinemachineBlenderSettings") as CinemachineBlenderSettings;

    }

    //[Button("Transfer")]
    void Transfer()
    {
        foreach(DataSequence seq in sequences)
        {
            sequenceBranches[0].AddSequence(seq);
        }
    }

    [Button("Add branch")]
    private void AddBranch()
    {
        SequenceBranch branch = ScriptableObject.CreateInstance<SequenceBranch>() as SequenceBranch;

        branch.name = "New branch" + (sequenceBranches.Count + 1);

        sequenceBranches.Add(branch);

        originalBranchName = branch.name;
    }

    [Button("Add sequence")]
    private void AddSequence()
    {
        DataSequence dSeq = ScriptableObject.CreateInstance<DataSequence>() as DataSequence;

        SequenceBranch lastBranch = sequenceBranches[sequenceBranches.Count - 1];

        dSeq.name = "DS" + (lastBranch.GetNumberOfSequences()+1);

        lastBranch.AddSequence(dSeq);

        originalName = "DS" + lastBranch.GetNumberOfSequences();
    }

    [Button("Add sequence copied")]
    private void AddSequenceCopied()
    {
        SequenceBranch lastBranch = sequenceBranches[sequenceBranches.Count - 1];
        if(branchIndexCopy >= 0 && branchIndexCopy < sequenceBranches.Count)
        {
            lastBranch = sequenceBranches[branchIndexCopy];
        }

        if (lastBranch.GetNumberOfSequences() > 0)
        {
            DataSequence dSeq = Instantiate(lastBranch.GetDataSequenceAt(lastBranch.GetNumberOfSequences() - 1));

            dSeq.name = "AutoSequence" + (lastBranch.GetNumberOfSequences() + 1);

            string seqCamName = dSeq.camTargetName;
            string[] namesParts = seqCamName.Split('m');
            int numberPost = int.Parse(namesParts[1]) + 1;

            dSeq.camTargetName = namesParts[0] + "m" + numberPost;

            lastBranch.AddSequence(dSeq);

            originalName = "AutoSequence" + lastBranch.GetNumberOfSequences();
        }
        else
        {
            Debug.Log("Bien essayé Max.");
        }

        
    }

    [HorizontalGroup("Rename", LabelWidth = 45)]
    [LabelText("Original")]
    public string originalName;

    [HorizontalGroup("Rename", LabelWidth = 30)]
    [LabelText("New")]
    public string newName;

    [HorizontalGroup("Rename")]
    [Button("Rename")]
    private void renameVar()
    {
        foreach(SequenceBranch branch in sequenceBranches)
        {
            List<DataSequence> sequencesInBranch = branch.GetAllSequences();

            foreach (DataSequence sequence in sequencesInBranch)
            {
                if (sequence != null && sequence.name == originalName)
                {
                    sequence.name = newName;
                    break;
                }
            }
        }

        
    }

    [HorizontalGroup("RenameBranch", LabelWidth = 45)]
    [LabelText("Original")]
    public string originalBranchName;

    [HorizontalGroup("RenameBranch", LabelWidth = 30)]
    [LabelText("New")]
    public string newBranchName;

    [HorizontalGroup("BranchCopy")]
    [LabelText("Branch to copy sequence")]
    public int branchIndexCopy = -1;

    [HorizontalGroup("RenameBranch")]
    [Button("Rename branch")]
    private void renameBranch()
    {
        foreach (SequenceBranch branch in sequenceBranches)
        {
             if (branch.name == originalBranchName)
             {
                 branch.name = newBranchName;
                 break;
             }
        }

    }


    public static SequenceHandler Instance { get; private set; }

    public float GetPurcentageBetweenNextCam() 
    {
        float MaxDist = Vector3.Distance(pastCamPos, newCamPos);
        float CurrDist = Vector3.Distance(pastCamPos, GameObject.Find("Main Camera").transform.position);
        if (MaxDist > 0)
            return CurrDist / MaxDist;
        else
            return 1;
    }

    public int GetCurrentSequenceIndex()
    {
        return sequenceIndex;
    }

    /// <summary>
    /// During the Update, the handler will check if the completion confitions are met to get to the next sequence.
    /// </summary>
    void Update()
    {

        if (currentVirtualCamera == null)
        {
            if (CameraHandler.Instance != null)
                currentVirtualCamera = CameraHandler.Instance.cinemachineCam.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;
            else
                currentVirtualCamera = GameObject.Find("Main Camera").GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera;


            blenderSettings.m_CustomBlends = new CinemachineBlenderSettings.CustomBlend[1];
        }

        if(!cameraCollider.enabled && delayOnBlendSequence <= 0) cameraCollider.enabled = true;


        //VERIFICATION DES SEQUENCES
        if (readSequences && !isWaitingTimer)
        {
            if (delayOnBlendSequence > 0)
            {
                delayOnBlendSequence -= Time.deltaTime;

                if(delayOnBlendSequence <= 0)
                {
                    delayOnBlendSequence = 0;
                   
                }
            }
            else
            {
                /*//DECLENCHEMENT DU FEEDBACK DE CAM
                if (CameraHandler.Instance != null)
                    CameraHandler.Instance.UpdateCamSteps(0, 100);*/

                /*
                C_Charger[] _Chargeurs = GameObject.FindObjectsOfType<C_Charger>();
                for (int i = 0; i < _Chargeurs.Length; i++)
                {
                    _Chargeurs[i].PlayerArrivedToPosition();
                }
                C_Shooter[] _Shooter = GameObject.FindObjectsOfType<C_Shooter>();
                for (int i = 0; i < _Shooter.Length; i++)
                {
                    _Shooter[i].PlayerArrivedToPosition();
                }
                */

                //CHECK AVANCEE TIMER SEQUENCE
                if (currentSequence.sequenceType == DataSequence.SequenceType.Timer)
                {
                    elapsedTime += Time.deltaTime;
                    if (elapsedTime >= currentSequence.timeSequenceDuration)
                    {
                        NextSequence();

                        elapsedTime = 0;
                    }

                }

                //CHECK AVANCEE KILL SEQUENCE
                if (currentSequence.sequenceType == DataSequence.SequenceType.KillEnnemies)
                {
                    Main.Instance.setAIWalls(true);

                    if (enemiesKilled >= currentSequence.enemiesToKillInSequence)
                    {
                        TimeScaleManager.Instance.AddSlowMo(0.9f, .5f);
                        TriggerUtil.TriggerSequence(currentSequence.timeBeforeNextSequenceOnKills);
                        isWaitingTimer = true;
                    }

                    if(currentSequence.activatesSpawnersDuringSequence && (!isSequenceTrigger) && enemiesKilled >= currentSequence.numberOfKillsRequiredToPop)
                    {
                        TriggerUtil.TriggerSpawners(currentSequence.delayOnTrigger, currentSequence.spawnersToTrigger, true);
                        isSequenceTrigger = true;
                    }
                   
                }

                //CHECK BOOLEAN SEQUENCE
                if (currentSequence.isAffectedByBooleanSequence)
                {
                    if(BooleanSequenceManager.Instance.GetStateOfBoolSequence(currentSequence.booleanSequenceName) == currentSequence.booleanSequenceStateRequired)
                    {
                        NextSequence();
                    }
                }
            }


        }

    }

    public void SkipToSequence(int sequenceNumber)
    {
        if(sequenceNumber < GetLargeSequenceNumber())
        {
            //cameraCollider.enabled = false;
            int indexLeft = sequenceNumber;
            int subBranchID = 0;
            foreach(SequenceBranch branch in sequenceBranches)
            {
                int numSeq = branch.GetNumberOfSequences();

                if (indexLeft >= numSeq)
                {
                    indexLeft -= numSeq;
                    subBranchID++;
                    continue;
                }
                else
                {
                    if(indexLeft == 0)
                    {
                        subBranchID--;
                        indexLeft = sequenceBranches[subBranchID].GetNumberOfSequences() - 1;
                    }

                    sequenceIndex = indexLeft;
                    branchIndex = subBranchID;
                    break;
                }
            }

            //Debug.Log($"Sequence index : {sequenceIndex} -- Branch index : {branchIndex}");

            currentBranch = sequenceBranches[branchIndex];
            currentSequence = sequenceBranches[branchIndex].GetDataSequenceAt(sequenceIndex - 1);
            //currentVirtualCamera = GameObject.Find(currentSequence.camTargetName).GetComponent<CinemachineVirtualCamera>();

            CameraHandler.Instance.ForceCinemachineCam();

            cameraCollider.enabled = false;

            //CHANGEMENT DE CAM -- Pour pas casser les timers
            currentVirtualCamera.Priority = 10;
            pastCamPos = currentVirtualCamera.transform.position;
            currentVirtualCamera = GameObject.Find(currentSequence.camTargetName).GetComponent<CinemachineVirtualCamera>();
            currentVirtualCamera.Priority = 11;
            newCamPos = currentVirtualCamera.transform.position;


            NextSequence();
        }

        else
        {
            Debug.Log($"No sequence with id {sequenceNumber}");
        }

        
    }

    
    /// <summary>
    /// Passes to the next sequence. Does nothing if no sequence is left
    /// </summary>
    public void NextSequence(bool isForced = false)
    {
        //Debug.Log($"Sequence index : {sequenceIndex} -- Branch index : {branchIndex}");

        Main.Instance.setAIWalls(false);

        isSequenceTrigger = false;

        if (currentSequence.cutsSlowMoOnEnd) TimeScaleManager.Instance.Stop();

        
        if (CameraHandler.Instance != null)
        {
            CameraHandler.Instance.StopBalancing();
            if (isForced) CameraHandler.Instance.ResyncCamera(true);
            if (currentSequence.cutLookAtOnEndOfSequence) CameraHandler.Instance.ReleaselookAt();
        }
        
        if (currentSequence.hasEventOnEnd)
        {
            switch (currentSequence.seqEvent)
            {
                case DataSequence.SequenceEndEventType.SlowMo:

                    TriggerUtil.TriggerSlowMo(currentSequence.timeBeforeEvent, currentSequence.slowMoDuration, currentSequence.slowMoPower);

                    break;

                case DataSequence.SequenceEndEventType.Activation:

                    TriggerUtil.TriggerActivation(currentSequence.timeBeforeEvent, currentSequence.affected, currentSequence.isActivation);

                    break;

                case DataSequence.SequenceEndEventType.Sound:

                    TriggerUtil.TriggerSound(currentSequence.timeBeforeEvent, currentSequence.soundPlayed, currentSequence.soundMixer,currentSequence.volume);

                    break;

                case DataSequence.SequenceEndEventType.Animation:

                    TriggerUtil.TriggerAnimationsFromTags(currentSequence.timeBeforeEvent, currentSequence.tagsAnimated);

                    break;

                case DataSequence.SequenceEndEventType.Balancing:

                    CameraHandler.Instance.SetupBalancing(currentSequence.distanceToAnchor, currentSequence.initImpulseBalancing, currentSequence.dampingOnBalancing, currentSequence.angleBalancing, 
                        currentSequence.minSpeedRot, currentSequence.returnLerpSpeedFromBalance,currentSequence.minSpeedRot, currentSequence.balancingFrequency, currentSequence.timeGoToNewRot,
                        currentSequence.additionalRotAfterFirstBalancing,currentSequence.additionalRotLerpGoTo);

                    break;

                case DataSequence.SequenceEndEventType.ArmorProgressiveGain:

                    Player.Instance.GainArmorOverTime(currentSequence.timeBeforeEvent, currentSequence.armorToGainOverTime,currentSequence.armorGainRate);

                    break;


                case DataSequence.SequenceEndEventType.SpawnEnemies:

                    TriggerUtil.TriggerSpawners(currentSequence.timeBeforeEvent, currentSequence.spawnersToActivate, true);

                    break;

                default:
                    break;
            }
        }

        if (currentSequence.resetsBufferAtEndOfSequence)
        {
            bufferedKills = 0;
        }

        bool branchSkipValidated = false;

        if (currentSequence.skipsToBranchOnEnd)
        {
            if (currentSequence.affectedByBooleanSequenceBranch)
            {
                foreach (DataSequence.BooleanLink link in currentSequence.booleanLinks)
                {
                    if (BooleanSequenceManager.Instance.GetStateOfBoolSequence(link.booleanSequence.boolName) == link.bSeqRequiredStatus)
                    {
                        currentBranch = sequenceBranches[link.indexOfBranchLinked];
                        branchIndex = link.indexOfBranchLinked;
                        branchSkipValidated = true;

                        Debug.Log("Link detected and validated");
                        break;
                    }
                }
            }
            else
            {
                currentBranch = sequenceBranches[currentSequence.branchLinkedId];
                branchIndex = currentSequence.branchLinkedId;
            }

            if(branchSkipValidated || !currentSequence.affectedByBooleanSequenceBranch)
                sequenceIndex = 0;

            //Debug.Log($"Sequence index : {sequenceIndex} -- Branch index : {branchIndex}");
        }

        if(!currentSequence.skipsToBranchOnEnd || (currentSequence.affectedByBooleanSequenceBranch && !branchSkipValidated))
        {
            if(sequenceIndex >= currentBranch.GetNumberOfSequences() - 1)
            {
                Debug.Log("No more sequences");
            }
            else
            {
                sequenceIndex++;
            }

        }

        //Debug.Log($"Sequence index : {sequenceIndex} -- Branch index : {branchIndex}");

        if (currentSequence.waitScreenAtEndOfSequence)
            WaitScreenFunction();

        isWaitingTimer = false;

        currentSequence = currentBranch.GetDataSequenceAt(sequenceIndex);

        //Debug.Log(currentVirtualCamera);
            
        //CREATION DU NOUVEAU BLEND
        CinemachineBlendDefinition blendDef = new CinemachineBlendDefinition
        {
            m_Style = currentSequence.animationStyle,
            m_Time = currentSequence.animationTime
        };

        //SETUP BLEND
        CinemachineBlenderSettings.CustomBlend blend = new CinemachineBlenderSettings.CustomBlend
        {
            m_From = currentVirtualCamera.Name,
            m_To = currentSequence.camTargetName,
            m_Blend = blendDef
        };

        if(blenderSettings.m_CustomBlends == null)
        {
            blenderSettings.m_CustomBlends = new CinemachineBlenderSettings.CustomBlend[1];
        }

        blenderSettings.m_CustomBlends[0] = blend;

        cameraBrain.m_CustomBlends = blenderSettings;

        //CHANGEMENT DE CAM
        currentVirtualCamera.Priority = 10;
        pastCamPos = currentVirtualCamera.transform.position;
        currentVirtualCamera = GameObject.Find(currentSequence.camTargetName).GetComponent<CinemachineVirtualCamera>();
        currentVirtualCamera.Priority = 11;
        newCamPos = currentVirtualCamera.transform.position;

        /*
        //APPEL DE FONCTIONS DANS LES CHARGEURS
        C_Charger[] _Chargeurs = GameObject.FindObjectsOfType<C_Charger>();
        for (int i = 0; i < _Chargeurs.Length; i++)
        {
            _Chargeurs[i].PlayerChangePosition();
        }
        C_Shooter[] _Shooter = GameObject.FindObjectsOfType<C_Shooter>();
        for (int i = 0; i < _Shooter.Length; i++)
        {
            _Shooter[i].PlayerChangePosition();
        }
        */

        if (currentSequence.ChangeMinigunState)
        {
            if (Weapon.Instance != null) Weapon.Instance.SetMinigun(currentSequence.EnableMinigun);
        }

        delayOnBlendSequence = currentSequence.animationTime + (currentSequence.sequenceType == DataSequence.SequenceType.Timer ? currentSequence.timeSequenceDuration : 0);
        enemiesKilled = 0;

        if (currentSequence.affectedObject != null)
        {
            if (currentSequence.actionType == DataSequence.gameObjectActionType.Activate) currentSequence.affectedObject.SetActive(currentSequence._active);
            else if (currentSequence.actionType == DataSequence.gameObjectActionType.MoveTo) currentSequence.affectedObject.transform.position = currentSequence.positionMoveTo;
        }
        if (!currentSequence.waitScreenAtEndOfSequence)
            WaitScreenFunction();
        Weapon.Instance.rotateLocked = currentSequence.lockWeaponLight;

        if (TutorialCheckpoint.Instance != null) TutorialCheckpoint.Instance.EndTutorialCheckpoint();

        if (currentSequence.checkpointToUse != null && TutorialCheckpoint.Instance != null) TutorialCheckpoint.Instance.InitTutorialCheckpoint(currentSequence.checkpointToUse);

        //DECLENCHEMENT DU FEEDBACK DE CAM
        if (CameraHandler.Instance != null)
        {
            CameraHandler.Instance.FeedbackTransition(currentSequence.enableCamFeedback, currentSequence.enableCamTransition, currentSequence.transitionTime);
            if (currentSequence.changeNoiseSettings)
            {
                CameraHandler.Instance.ChangeNoiseSettings(currentSequence.noisePurcentageAimed, currentSequence.timeTransitionNoise, currentSequence.noiseAmplitudePos, currentSequence.noiseAmplitudeRot, currentSequence.noiseFrequency);
            }
            float frequencyValue = Vector3.Distance(pastCamPos, newCamPos) / 2 / (delayOnBlendSequence != 0 ? delayOnBlendSequence : 0.1f);
            CameraHandler.Instance.UpdatePos(pastCamPos,newCamPos, currentSequence.stepFadeAtStart, currentSequence.stepFadeAtEnd, currentSequence.distFadeStart, currentSequence.distFadeEnd);
            CameraHandler.Instance.StepSound(!currentSequence.removeStepSound);
            CameraHandler.Instance.UpdateBreathing(currentSequence.breathingEnabled, currentSequence.timeFadeBreathing);
            if (currentSequence.isShortStep)
            {
                CameraHandler.Instance.ShortStep(currentSequence.shortStepCurve, currentSequence.shortStepAmplitude, currentSequence.animationTime, currentSequence.stepSoundPurcentageOnShortStep);
            }
            else
            {
                if (currentSequence.modifySteps)
                {
                    CameraHandler.Instance.UpdateCamSteps(frequencyValue * currentSequence.modifierFrequenceCamStep, currentSequence.animationTime);
                    if (currentSequence.modifyStepsCurve)
                        CameraHandler.Instance.SetCurrentAnimCurveModified(currentSequence.modifiedStepCurve);
                    else
                        CameraHandler.Instance.SetCurrentAnimCurve(blendDef);
                }
                else
                {
                    CameraHandler.Instance.UpdateCamSteps(frequencyValue, currentSequence.animationTime);
                    CameraHandler.Instance.SetCurrentAnimCurve(blendDef);
                }
            }
            if (currentSequence.lookAtObject != null) CameraHandler.Instance.CameraLookAt(currentSequence.lookAtObject, currentSequence.weightLookAt, currentSequence.weightRemoveRotLookAt, currentSequence.transitionToTime, currentSequence.transitionBackTime, currentSequence.lookAtTime);
                
        }
        if (currentSequence.animToPlay != null)
        {
            CameraHandler.Instance.TriggerAnim(currentSequence.animToPlay, currentSequence.animationTime);
        }

        if (currentSequence.sequenceType == DataSequence.SequenceType.KillEnnemies)
        {
            if (currentSequence.acceptsBufferKill)
            {
                enemiesKilled = bufferedKills;

            }
            bufferedKills = 0;
        }
    }


    /// <summary>
    /// Called when an Enemy dies by the player
    /// </summary>
    public void OnEnemyKill()
    {
        if (currentSequence.sequenceType == DataSequence.SequenceType.KillEnnemies)
        {
            enemiesKilled++;

            //Debug.Log(enemiesKilled);
        }
        else
        {
            bufferedKills++;
        }
    }

    public bool IsCurrentSequenceOnAction()
    {
        return (readSequences && currentSequence.sequenceType == DataSequence.SequenceType.KillEnnemies);
    }

    public int GetLargeSequenceNumber()
    {
        int number = 0;

        foreach(SequenceBranch branch in sequenceBranches)
        {
            number += branch.GetNumberOfSequences();
        }

        return number;
    }

    void WaitScreenFunction()
    {
        if (currentSequence.changeWaitScreen)
            StartCoroutine(WaitScreenCoroutine(currentSequence.activateWaitScreen));
    }

    IEnumerator WaitScreenCoroutine(bool activate)
    {
        if (currentSequence.waitScreenDelay > 0)
            yield return new WaitForSeconds(currentSequence.waitScreenDelay);

        if (activate)
        {
            UiCrossHair.Instance.WaitFunction();
            Main.Instance.SetupWaitScreenOn();
        }
        else
        {
            UiCrossHair.Instance.StopWaitFunction();
            Main.Instance.SetupWaitScreenOff();
        }

        yield break;
    }

}