using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerUtil
{

    //SPAWNERS
    public static void TriggerSpawners(float tTimeBeforeStart, Spawner[] spawners)
    {
        Main.Instance.StartCoroutine(TriggerSpawnersCoroutine(tTimeBeforeStart, spawners));
    }

    static IEnumerator TriggerSpawnersCoroutine(float tTimeBeforeStart, Spawner[] spawners)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        foreach (Spawner spawner in spawners)
        {
            spawner.StartSpawn();
        }

        yield break;
    }

    //SLOW MOTION
    public static void TriggerSlowMo(float tTimeBeforeStart, float duration, float force)
    {
        Main.Instance.StartCoroutine(TriggerSlowMoCoroutine(tTimeBeforeStart, duration, force));
    }

    static IEnumerator TriggerSlowMoCoroutine(float tTimeBeforeStart, float duration, float force)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        //C_TimeScale.Instance.AddSlowMo(force, duration);

        yield break;
    }

    //ANIMATIONS
    public static void TriggerAnimators(float tTimeBeforeStart, Animator[] animators)
    {
        Main.Instance.StartCoroutine(TriggerAnimatorsCoroutine(tTimeBeforeStart, animators));
    }

    static IEnumerator TriggerAnimatorsCoroutine(float tTimeBeforeStart, Animator[] animators)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        foreach (Animator anim in animators)
        {
            anim.SetTrigger("MakeAction");
        }

        yield break;
    }

    //WEAPON ACTIVATIONS
    public static void TriggerActivation(float tTimeBeforeStart, TriggerSender.Activable affected, bool isActivation)
    {
        Main.Instance.StartCoroutine(TriggerActivationCoroutine(tTimeBeforeStart, affected, isActivation));
    }

    static IEnumerator TriggerActivationCoroutine(float tTimeBeforeStart, TriggerSender.Activable activable, bool state)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        Main.Instance.SetControlState(activable, state);

        yield break;
    }

    //SOUND EFFECT
    public static void TriggerSound(float tTimeBeforeStart, string soundPlayed, float volume)
    {
        Main.Instance.StartCoroutine(TriggerSoundCoroutine(tTimeBeforeStart, soundPlayed, volume));
    }

    static IEnumerator TriggerSoundCoroutine(float tTimeBeforeStart, string soundName, float volume)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, soundName, false, volume);

        yield break;
    }

    //SHAKE TRIGGER
    public static void TriggerShake(float tTimeBeforeStart, float shakeForce)
    {
        Main.Instance.StartCoroutine(TriggerShakeCoroutine(tTimeBeforeStart, shakeForce));
    }

    static IEnumerator TriggerShakeCoroutine(float tTimeBeforeStart, float shakeForce)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        //Main.Instance.GetComponent<C_Camera>().AddShake(shakeForce);

        yield break;
    }


    //ANIMATION TRIGGER TAG
    public static void TriggerAnimationsFromTags(float tTimeBeforeStart, string[] tags)
    {
        Main.Instance.StartCoroutine(TriggerAnimationsFromTagsCoroutine(tTimeBeforeStart, tags));
    }

    static IEnumerator TriggerAnimationsFromTagsCoroutine(float tTimeBeforeStart, string[] tags)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        List<Animator> animTag = new List<Animator>();
        Animator anim;
        GameObject[] goTab;

        foreach (string tag in tags)
        {
            goTab = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject go in goTab)
            {
                anim = go.GetComponent<Animator>();
                if (anim != null)
                {
                    animTag.Add(anim);
                }

            }
        }

        foreach (Animator anima in animTag)
        {
            anima.SetTrigger("MakeAction");
        }

        yield break;
    }

    //ANIMATION TRIGGER W/O TAGS
    public static void TriggerAnimations(float tTimeBeforeStart, Animator[] anims)
    {
        Main.Instance.StartCoroutine(TriggerAnimationsCoroutine(tTimeBeforeStart, anims));
    }

    static IEnumerator TriggerAnimationsCoroutine(float tTimeBeforeStart, Animator[] anims)
    {
        yield return new WaitForSecondsRealtime(tTimeBeforeStart);

        foreach (Animator anima in anims)
        {
            anima.SetTrigger("MakeAction");
        }

        yield break;
    }
}

