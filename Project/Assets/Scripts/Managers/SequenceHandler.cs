using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class SequenceHandler : MonoBehaviour
{
    [SerializeField, ListDrawerSettings(NumberOfItemsPerPage = 50)]
    List<DataSequence> sequences = null;

    DataSequence currentSequence = null;

    Camera cameraObj = null;
    CinemachineVirtualCamera currentVirtualCamera = null;

    CinemachineBlenderSettings blenderSettings;

    CinemachineBrain cameraBrain = null;

    float elapsedTime = 0;
    float delayOnBlendSequence = 0;
    int sequenceIndex = 0;

    uint enemiesKilled = 0;
    uint bufferedKills = 0;

    bool readSequences = true;

    [HideInInspector]
    public bool isWaitingTimer = false;

    Vector3 pastCamPos = Vector3.one;
    Vector3 newCamPos = Vector3.one;

    //CinemachineVirtualCamera StockPreviousCam = null;

    void Start()
    {
        Instance = this;

        cameraObj = Camera.main;

        cameraBrain = GameObject.FindObjectOfType<CinemachineBrain>();

        currentSequence = sequences[0];

        blenderSettings = ScriptableObject.CreateInstance("CinemachineBlenderSettings") as CinemachineBlenderSettings;

    }

    [Button("Add sequence")]
    private void AddSequence()
    {
        DataSequence dSeq = ScriptableObject.CreateInstance<DataSequence>() as DataSequence;
        dSeq.name = "DS" + (sequences.Count+1);
        sequences.Add(dSeq);
        originalName = "DS" + sequences.Count;
    }

    [Button("Add sequence copied")]
    private void AddSequenceCopied()
    {
        if(sequences.Count > 0)
        {
            DataSequence dSeq = Instantiate(sequences[sequences.Count - 1]);
            dSeq.name = "AutoSequence" + (sequences.Count + 1);

            string seqName = sequences[sequences.Count - 1].camTargetName;
            string[] namesParts = seqName.Split('m');
            int numberPost = int.Parse(namesParts[1]) + 1;

            dSeq.camTargetName = namesParts[0] + "m" + numberPost;

            sequences.Add(dSeq);
            originalName = "AutoSequence" + sequences.Count;
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
        foreach(DataSequence sequence in sequences)
        {
            if(sequence.name == originalName)
            {
                sequence.name = newName;
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



        //VERIFICATION DES SEQUENCES
        if (readSequences && !isWaitingTimer)
        {


            if (delayOnBlendSequence > 0)
            {
                delayOnBlendSequence -= Time.deltaTime;
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
                    if (enemiesKilled >= currentSequence.enemiesToKillInSequence)
                    {
                        TriggerUtil.TriggerSequence(currentSequence.timeBeforeNextSequenceOnKills);
                        isWaitingTimer = true;
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
        if(sequenceNumber < sequences.Count)
        {
            sequenceIndex = sequenceNumber - 1;
            currentVirtualCamera = GameObject.Find(sequences[sequenceNumber - 1].camTargetName).GetComponent<CinemachineVirtualCamera>();
            NextSequence();
        }
        else
        {
            Debug.Log($"No sequence with id {sequenceNumber}");
        }

        
    }

    
    /// <summary>
    /// Passes to the next sequence. Does nothing is no sequence is left
    /// </summary>
    public void NextSequence(bool isForced = false)
    {
        if (currentSequence.cutsSlowMoOnEnd) TimeScaleManager.Instance.Stop();

        
        if (CameraHandler.Instance != null)
        {
            if (isForced) CameraHandler.Instance.ResyncCamera(true);
            CameraHandler.Instance.FeedbackTransition(currentSequence.enableCamFeedback, currentSequence.enableCamTransition, currentSequence.transitionTime);
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

                    TriggerUtil.TriggerSound(currentSequence.timeBeforeEvent, currentSequence.soundPlayed, currentSequence.volume);

                    break;

                case DataSequence.SequenceEndEventType.Animation:

                    TriggerUtil.TriggerAnimationsFromTags(currentSequence.timeBeforeEvent, currentSequence.tagsAnimated);

                    break;

                default:
                    break;
            }
        }

        if (sequenceIndex < sequences.Count - 1)
        {
            sequenceIndex++;
            isWaitingTimer = false;

            currentSequence = sequences[sequenceIndex];

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

            delayOnBlendSequence = currentSequence.animationTime + (currentSequence.sequenceType == DataSequence.SequenceType.Timer ? currentSequence.timeSequenceDuration : 0);
            enemiesKilled = 0;

            if (currentSequence.affectedObject != null)
            {
                if (currentSequence.actionType == DataSequence.gameObjectActionType.Activate) currentSequence.affectedObject.SetActive(currentSequence._active);
                else if (currentSequence.actionType == DataSequence.gameObjectActionType.MoveTo) currentSequence.affectedObject.transform.position = currentSequence.positionMoveTo;
            }


            //DECLENCHEMENT DU FEEDBACK DE CAM
            if (CameraHandler.Instance != null)
            {
                float frequencyValue = Vector3.Distance(pastCamPos, newCamPos) / (delayOnBlendSequence != 0 ? delayOnBlendSequence : 0.1f);
                if (currentSequence.isShortStep)
                {
                    CameraHandler.Instance.ShortStep(currentSequence.shortStepCurve, currentSequence.shortStepAmplitude, currentSequence.animationTime);
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
                if (currentSequence.lookAtObject != null) CameraHandler.Instance.CameraLookAt(currentSequence.lookAtObject, currentSequence.transitionToTime, currentSequence.transitionBackTime, currentSequence.lookAtTime);
                
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
        else
        {
            Debug.Log("No sequence left");

            readSequences = false;
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
}