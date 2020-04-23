using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ShootTriggerManager : MonoBehaviour
{
    int nbChilds = 0;

    int nbEventsSent = 0;

    GameObject main;

    [SerializeField]
    Animator[] animators = null;

    [SerializeField]
    AnimBlocker[] blockers = null;

    [Header("Sequences")]
    [SerializeField]
    bool startsNextSequenceOnTrigger = false;

    [SerializeField, ShowIf("startsNextSequenceOnTrigger")]
    float timeBeforeNextSequence = 0;

    // SHAKE ///////////////////////////////
    [SerializeField, Header("Shake")]
    bool startsShakeAfterAllTriggers = false;

    [SerializeField, ShowIf("startsShakeAfterAllTriggers")]
    float timeBeforeShake = 0;

    [SerializeField, ShowIf("startsShakeAfterAllTriggers")]
    float shakeForce = 0f;
    
    [SerializeField, ShowIf("startsShakeAfterAllTriggers")]
    float shakeDuration = 0f;

    // SOUND ////////////////////////////////
    [SerializeField, Header("Sound")]
    bool isPlayingSound = false;

    [SerializeField, ShowIf("isPlayingSound")]
    float timeBeforeSound = 0;

    [SerializeField, ShowIf("isPlayingSound")]
    string soundPlayed = "";
    [SerializeField, ShowIf("isPlayingSound")]
    float soundVolume = 1;

    // BOOLEAN SEQUENCE ////////////////////
    [SerializeField, Header("Boolean Sequence")]
    bool triggersBooleanSequence = false;

    [SerializeField, ShowIf("triggersBooleanSequence")]
    string booleanName = "";
    [SerializeField, ShowIf("triggersBooleanSequence")]
    bool booleanStateSet = false;
    [SerializeField, ShowIf("triggersBooleanSequence")]
    float delayOnBooleanActivation = 0;


    // END GAME ////////////////////////////
    [SerializeField]
    bool gameEnder = false;

    GameObject[] childs;

    // Start is called before the first frame update
    void Start()
    {
        nbChilds = transform.childCount;

        main = Main.Instance.gameObject;
    }

    /// <summary>
    /// Called by the child ShootTriggers when they are shot (killed). Once they're all dead, triggers the required actions.
    /// </summary>
    public void OnEventSent()
    {
        nbEventsSent++;

        if (nbEventsSent == nbChilds)
        {
            bool canContinue = true;

            foreach (AnimBlocker block in blockers)
            {
                canContinue = !block.isBlocked;

                if (!canContinue)
                    break;
            }

            if (canContinue)
            {
                Trigger();
            }
            else
            {
                StartCoroutine(CheckBlockers());
            }

        }


    }

    /// <summary>
    /// Verifies if theres any AnimBlocker in the hitbox. If there are, it waits until they are all gone.
    /// </summary>
    /// <returns></returns>
    IEnumerator CheckBlockers()
    {
        bool canContinue;
        do
        {
            canContinue = false;

            yield return new WaitForSeconds(.5f);

            foreach (AnimBlocker block in blockers)
            {
                canContinue = !block.isBlocked;

                if (!canContinue)
                    break;
            }
        }
        while (!canContinue);

        Trigger();

        yield break;
    }

    /// <summary>
    /// Shortcut for NextSequence in SequenceHandler
    /// </summary>
    void StartNextsequence()
    {
        SequenceHandler.Instance.NextSequence();
    }


    /// <summary>
    /// Starts all the desired triggers after a specific time.
    /// </summary>
    void Trigger()
    {
        TriggerUtil.TriggerAnimations(0, animators);

        if (soundPlayed != "")
        {
            //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, soundPlayed, false, soundVolume);
            CustomSoundManager.Instance.PlaySound(soundPlayed, "Effect", soundVolume);
        }

        if (startsNextSequenceOnTrigger)
        {
            Invoke("StartNextsequence", timeBeforeNextSequence);
        }

        if (startsShakeAfterAllTriggers)
        {
            TriggerUtil.TriggerShake(timeBeforeShake, shakeForce, shakeDuration, true);
        }

        if (isPlayingSound)
        {
            TriggerUtil.TriggerSound(timeBeforeSound, soundPlayed, soundVolume);
        }

        if (triggersBooleanSequence)
        {
            TriggerUtil.TriggerBooleanSequence(delayOnBooleanActivation, booleanName, booleanStateSet);
        }

        if (gameEnder)
        {
            //LeaderboardManager.Instance.SubmitScoreToLeaderboard("GAM", PublicManager.Instance.GetNbViewers(), "default title in shoot trigger maanger");
            //MetricsGestionnary.Instance.SaveMetrics();

            Main.Instance.InitLeaderboard();

            //SceneHandler.Instance.ChangeScene("MenuScene", .3f, true);
            //CustomSoundManager.Instance.PlaySound("RestartSound", "UI", 1);
        }
    }
}