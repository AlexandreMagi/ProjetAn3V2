﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TriggerUtil
{

    //SPAWNERS
    public static void TriggerSpawners(float timeBeforeStart, Spawner[] spawners)
    {
        Main.Instance.StartCoroutine(TriggerSpawnersCoroutine(timeBeforeStart, spawners));
    }

    static IEnumerator TriggerSpawnersCoroutine(float timeBeforeStart, Spawner[] spawners)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

        foreach (Spawner spawner in spawners)
        {
            spawner.StartSpawn();
        }

        yield break;
    }

    //SLOW MOTION
    public static void TriggerSlowMo(float timeBeforeStart, float duration, float force)
    {
        Main.Instance.StartCoroutine(TriggerSlowMoCoroutine(timeBeforeStart, duration, force));
    }

    static IEnumerator TriggerSlowMoCoroutine(float timeBeforeStart, float duration, float force)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

        TimeScaleManager.Instance.AddSlowMo(force, duration);

        yield break;
    }

    //ANIMATIONS
    public static void TriggerAnimators(float timeBeforeStart, Animator[] animators)
    {
        Main.Instance.StartCoroutine(TriggerAnimatorsCoroutine(timeBeforeStart, animators));
    }

    static IEnumerator TriggerAnimatorsCoroutine(float timeBeforeStart, Animator[] animators)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

        foreach (Animator anim in animators)
        {
            anim.SetTrigger("MakeAction");
        }

        yield break;
    }

    //WEAPON ACTIVATIONS
    public static void TriggerActivation(float timeBeforeStart, TriggerSender.Activable affected, bool isActivation)
    {
        Main.Instance.StartCoroutine(TriggerActivationCoroutine(timeBeforeStart, affected, isActivation));
    }

    static IEnumerator TriggerActivationCoroutine(float timeBeforeStart, TriggerSender.Activable activable, bool state)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

        Main.Instance.SetControlState(activable, state);

        yield break;
    }

    //SOUND EFFECT
    public static void TriggerSound(float timeBeforeStart, string soundPlayed, float volume)
    {
        Main.Instance.StartCoroutine(TriggerSoundCoroutine(timeBeforeStart, soundPlayed, volume));
    }

    static IEnumerator TriggerSoundCoroutine(float timeBeforeStart, string soundName, float volume)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, soundName, false, volume);

        yield break;
    }

    //SHAKE TRIGGER
    public static void TriggerShake(float timeBeforeStart, float shakeForce, Vector3 pos)
    {
        Main.Instance.StartCoroutine(TriggerShakeCoroutine(timeBeforeStart, shakeForce, pos));
    }
    //SHAKE TRIGGER
    public static void TriggerShake(float timeBeforeStart, float shakeForce)
    {
        Main.Instance.StartCoroutine(TriggerShakeCoroutine(timeBeforeStart, shakeForce, Vector3.one * 666)); // 666 value safe
    }

    static IEnumerator TriggerShakeCoroutine(float timeBeforeStart, float shakeForce, Vector3 pos)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

        if (pos != Vector3.one * 666) CameraHandler.Instance.AddShake(shakeForce, pos);
        else CameraHandler.Instance.AddShake(shakeForce);
        yield break;
    }


    //ANIMATION TRIGGER TAG
    public static void TriggerAnimationsFromTags(float timeBeforeStart, string[] tags)
    {
        Main.Instance.StartCoroutine(TriggerAnimationsFromTagsCoroutine(timeBeforeStart, tags));
    }

    static IEnumerator TriggerAnimationsFromTagsCoroutine(float timeBeforeStart, string[] tags)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

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
    public static void TriggerAnimations(float timeBeforeStart, Animator[] anims)
    {
        Main.Instance.StartCoroutine(TriggerAnimationsCoroutine(timeBeforeStart, anims));
    }

    static IEnumerator TriggerAnimationsCoroutine(float timeBeforeStart, Animator[] anims)
    {
        yield return new WaitForSecondsRealtime(timeBeforeStart);

        foreach (Animator anima in anims)
        {
            anima.SetTrigger("MakeAction");
        }

        yield break;
    }
}

