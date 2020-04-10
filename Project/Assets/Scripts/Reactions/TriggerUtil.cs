using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Video;

public static class TriggerUtil
{
    //SEQUENCE
    public static void TriggerSequence(float timeBeforeStart)
    {
        Main.Instance.StartCoroutine(TriggerBooleanSequenceCoroutine(timeBeforeStart));
    }

    static IEnumerator TriggerBooleanSequenceCoroutine(float timeBeforeStart)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        SequenceHandler.Instance.NextSequence();

        yield break;
    }

    //SPAWNERS
    public static void TriggerSpawners(float timeBeforeStart, Spawner[] spawners)
    {
        Main.Instance.StartCoroutine(TriggerSpawnersCoroutine(timeBeforeStart, spawners));
    }

    static IEnumerator TriggerSpawnersCoroutine(float timeBeforeStart, Spawner[] spawners)
    {
        yield return new WaitForSeconds(timeBeforeStart);

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
        yield return new WaitForSeconds(timeBeforeStart);

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
        yield return new WaitForSeconds(timeBeforeStart);

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
        yield return new WaitForSeconds(timeBeforeStart);

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
        yield return new WaitForSeconds(timeBeforeStart);

        //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, soundName, false, volume);
        CustomSoundManager.Instance.PlaySound(soundName, "Effect", volume);

        yield break;
    }

    //SHAKE TRIGGER
    public static void TriggerShake(float timeBeforeStart, float shakeForce, float shakeDuration, Vector3 pos)
    {
        Main.Instance.StartCoroutine(TriggerShakeCoroutine(timeBeforeStart, shakeForce, shakeDuration, pos));
    }
    //SHAKE TRIGGER
    public static void TriggerShake(float timeBeforeStart, float shakeForce, float shakeDuration)
    {
        Main.Instance.StartCoroutine(TriggerShakeCoroutine(timeBeforeStart, shakeForce, shakeDuration, Vector3.one * 666)); // 666 value safe
    }

    static IEnumerator TriggerShakeCoroutine(float timeBeforeStart, float shakeForce,float shakeDuration, Vector3 pos)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        if (pos != Vector3.one * 666) CameraHandler.Instance.AddShake(shakeForce, pos, shakeDuration);
        else CameraHandler.Instance.AddShake(shakeForce, shakeDuration);

        yield break;
    }


    //ANIMATION TRIGGER TAG
    public static void TriggerAnimationsFromTags(float timeBeforeStart, string[] tags)
    {
        Main.Instance.StartCoroutine(TriggerAnimationsFromTagsCoroutine(timeBeforeStart, tags));
    }

    static IEnumerator TriggerAnimationsFromTagsCoroutine(float timeBeforeStart, string[] tags)
    {
        yield return new WaitForSeconds(timeBeforeStart);

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
        yield return new WaitForSeconds(timeBeforeStart);

        foreach (Animator anima in anims)
        {
            anima.SetTrigger("MakeAction");
        }

        yield break;
    }

    //BOOLEAN SEQUENCES
    public static void TriggerBooleanSequence(float timeBeforeStart, string boolName, bool state)
    {
        Main.Instance.StartCoroutine(TriggerBooleanSequenceCoroutine(timeBeforeStart, boolName, state));
    }

    static IEnumerator TriggerBooleanSequenceCoroutine(float timeBeforeStart, string boolName, bool state)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        BooleanSequenceManager.Instance.SetStateOfBoolSequence(boolName, state);

        yield break;
    }

    //FOLLOW TARGET
    public static void TriggerFollowTarget(float timeBeforeStart, Transform target, float timeGoTo, float timeGoBack, float followDuration)
    {
        Main.Instance.StartCoroutine(TriggerFollowTargetCoroutine(timeBeforeStart, target, timeGoTo, timeGoBack, followDuration));
    }

    static IEnumerator TriggerFollowTargetCoroutine(float timeBeforeStart, Transform target, float timeGoTo, float timeGoBack, float followDuration)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        CameraHandler.Instance.CameraLookAt(target, 1, 1, timeGoTo, timeGoBack, followDuration);

        yield break;
    }

    //VFX
    public static void TriggerVFX(float timeBeforeStart, ParticleSystem[] VFXTab, bool state)
    {
        Main.Instance.StartCoroutine(TriggerVFXCoroutine(timeBeforeStart, VFXTab, state));
    }

    static IEnumerator TriggerVFXCoroutine(float timeBeforeStart, ParticleSystem[] VFXTab, bool state)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach(ParticleSystem vfx in VFXTab)
        {
            if (vfx != null)
            {
                if (state)
                    vfx.Play();
                else
                    vfx.Stop();
            }
        }

        yield break;
    }

    //Plane Shake
    public static void TriggerPlaneShake(bool activate, float timeBeforeStart)
    {
        Main.Instance.StartCoroutine(TriggerPlaneShakeCoroutine(activate, timeBeforeStart));
    }

    static IEnumerator TriggerPlaneShakeCoroutine(bool activate, float timeBeforeStart)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        PlaneShakeManager.Instance.activated = activate;

        yield break;
    }

    // Desactivation/Activation de l'outline
    public static void TriggerOutline(bool activate, float timeBeforeStart)
    {
        Main.Instance.StartCoroutine(TriggerOutlineCoroutine(activate, timeBeforeStart));
    }

    static IEnumerator TriggerOutlineCoroutine(bool activate, float timeBeforeStart)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        if (PostprocessManager.Instance != null)
            PostprocessManager.Instance.OutlineSetActive(activate);
        yield break;
    }

    // Desactivation/Activation de la reveal light
    public static void TriggerRevealLight(bool activate, float timeBeforeStart)
    {
        Main.Instance.StartCoroutine(TriggerRevealLightCoroutine(activate, timeBeforeStart));
    }

    static IEnumerator TriggerRevealLightCoroutine(bool activate, float timeBeforeStart)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        Weapon.Instance.EnableDisableRevealLight(activate);
        yield break;
    }

    // White Screen Effect
    public static void TriggerWhiteScreenEffect(float timeBeforeStart)
    {
        Main.Instance.StartCoroutine(TriggerWhiteScreenEffectCoroutine(timeBeforeStart));
    }

    static IEnumerator TriggerWhiteScreenEffectCoroutine(float timeBeforeStart)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        if (WhiteScreenEffect.Instance != null)
            WhiteScreenEffect.Instance.StartWhiteScreenEffect();
        yield break;
    }

    //Video players
    public static void TriggerVideo(float timeBeforeStart, List<VideoPlayer> players, bool playMode, bool loopMode)
    {
        Main.Instance.StartCoroutine(TriggervideoCoroutine(timeBeforeStart, players, playMode, loopMode));
    }

    static IEnumerator TriggervideoCoroutine(float timeBeforeStart, List<VideoPlayer> players, bool playMode, bool loopMode)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach(VideoPlayer videoPl in players)
        {
            if (playMode)
                videoPl.Play();
            else
                videoPl.Stop();

            videoPl.isLooping = loopMode;
        }
        yield break;
    }
}

