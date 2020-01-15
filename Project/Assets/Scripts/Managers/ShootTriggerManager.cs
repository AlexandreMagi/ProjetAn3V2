using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField, ShowWhen("startsNextSequenceOnTrigger")]
    float timeBeforeNextSequence = 0;

    [SerializeField]
    bool startsShakeAfterAllTriggers = false;
    [SerializeField, ShowWhen("startsShakeAfterAllTriggers")]
    float shakeForce = 0f;

    [SerializeField]
    string soundPlayed = "";
    [SerializeField]
    float soundVolume = 1;

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
                TriggerAnim();
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

        TriggerAnim();

        yield break;
    }

    void StartNextsequence()
    {
        SequenceHandler.Instance.NextSequence();
    }

    void TriggerAnim()
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
    }
}