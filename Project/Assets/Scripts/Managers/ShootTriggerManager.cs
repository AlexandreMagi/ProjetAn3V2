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


    [SerializeField]
    bool startsNextSequenceOnTrigger = false;

    [SerializeField, ShowIf("startsNextSequenceOnTrigger")]
    float timeBeforeNextSequence = 0;

    // SHAKE ///////////////////////////////
    [SerializeField, Header("Shake")]
    bool startsShakeAfterAllTriggers = false;

    [SerializeField, ShowIf("startsShakeAfterAllTriggers")]
    float shakeForce = 0f;

    // SOUND ////////////////////////////////
    [SerializeField, Header("Sound")]
    bool isPlayingSound = false;

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


    // Start is called before the first frame update
    void Start()
    {
        nbChilds = transform.childCount;

        main = Main.Instance.gameObject;
    }

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

    void StartNextsequence()
    {
        SequenceHandler.Instance.NextSequence();
    }

    void Trigger()
    {
        TriggerUtil.TriggerAnimations(0, animators);

        if (soundPlayed != "")
        {
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, soundPlayed, false, soundVolume);
        }

        if (startsNextSequenceOnTrigger)
        {
            Invoke("StartNextsequence", timeBeforeNextSequence);
        }

        if (startsShakeAfterAllTriggers)
        {
            TriggerUtil.TriggerShake(0, shakeForce);
        }

        if (isPlayingSound)
        {
            TriggerUtil.TriggerSound(0, soundPlayed, soundVolume);
        }

        if (triggersBooleanSequence)
        {
            TriggerUtil.TriggerBooleanSequence(0, booleanName, booleanStateSet);
        }
    }
}