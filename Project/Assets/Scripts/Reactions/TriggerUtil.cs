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
    public static void TriggerAnimators(float timeBeforeStart, Animator[] animators, bool loopTimer, float animationWaitTimer = 0)
    {
        Main.Instance.StartCoroutine(TriggerAnimatorsCoroutine(timeBeforeStart, animators, loopTimer, animationWaitTimer));
    }

    static IEnumerator TriggerAnimatorsCoroutine(float timeBeforeStart, Animator[] animators, bool loopTimer, float animationWaitTimer)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach (Animator anim in animators)
        {
            anim.SetTrigger("MakeAction");
            if (loopTimer && animationWaitTimer != 0)
                yield return new WaitForSeconds(animationWaitTimer);
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
    public static void TriggerShake(float timeBeforeStart, float shakeForce, float shakeDuration, Vector3 pos, bool isStopShake)
    {
        Main.Instance.StartCoroutine(TriggerShakeCoroutine(timeBeforeStart, shakeForce, shakeDuration, pos, isStopShake));
    }
    //SHAKE TRIGGER
    public static void TriggerShake(float timeBeforeStart, float shakeForce, float shakeDuration, bool isStopShake)
    {
        Main.Instance.StartCoroutine(TriggerShakeCoroutine(timeBeforeStart, shakeForce, shakeDuration, Vector3.one * 666, isStopShake)); // 666 value safe
    }

    static IEnumerator TriggerShakeCoroutine(float timeBeforeStart, float shakeForce,float shakeDuration, Vector3 pos, bool isStopShake)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        if (!isStopShake)
        {
            if (pos != Vector3.one * 666) CameraHandler.Instance.AddShake(shakeForce, pos, shakeDuration);
            else CameraHandler.Instance.AddShake(shakeForce, shakeDuration);
        }
        else
        {
            CameraHandler.Instance.RemoveShake();
        }
       

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
    public static void TriggerWhiteScreenEffect(float timeBeforeStart, float _scaleMin = 0, float _scaleMax = 6, float _timeToScaleMax = 3, float _timeStayAtMax = 1, float _timeFadeAlpha = 2, bool _independentFromTimeScale = false)
    {
        Main.Instance.StartCoroutine(TriggerWhiteScreenEffectCoroutine(timeBeforeStart, _scaleMin, _scaleMax, _timeToScaleMax, _timeStayAtMax, _timeFadeAlpha, _independentFromTimeScale));
    }

    static IEnumerator TriggerWhiteScreenEffectCoroutine(float timeBeforeStart, float _scaleMin = 0, float _scaleMax = 6, float _timeToScaleMax = 3, float _timeStayAtMax = 1, float _timeFadeAlpha = 2, bool _independentFromTimeScale = false)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        if (WhiteScreenEffect.Instance != null)
            WhiteScreenEffect.Instance.StartWhiteScreenEffect(_scaleMin, _scaleMax, _timeToScaleMax, _timeStayAtMax, _timeFadeAlpha, _independentFromTimeScale);
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

    //GO activation
    public static void TriggerGameObjectActivation(float timeBeforeStart, List<GameObject> GOList, bool activation)
    {
        Main.Instance.StartCoroutine(TriggerGameObjectActivationCoroutine(timeBeforeStart, GOList, activation));
    }

    static IEnumerator TriggerGameObjectActivationCoroutine(float timeBeforeStart, List<GameObject> GOList, bool activation)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach(GameObject GO in GOList)
        {
            GO.SetActive(activation);
        }

        yield break;
    }

    //Dégats sur joueur
    public static void TriggerDamage(float timeBeforeStart, float damages)
    {
        Main.Instance.StartCoroutine(TriggerDamageCoroutine(timeBeforeStart, damages));
    }

    static IEnumerator TriggerDamageCoroutine(float timeBeforeStart, float damages)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        Player.Instance.TakeDamage(damages);

        yield break;
    }

    //Swarmer trigger
    public static void TriggerSwarmers(float timeBeforeStart, List<Swarmer> swarmers)
    {
        Main.Instance.StartCoroutine(TriggerSwarmersCoroutine(timeBeforeStart, swarmers));
    }

    static IEnumerator TriggerSwarmersCoroutine(float timeBeforeStart, List<Swarmer> swarmers)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach (Swarmer swarm in swarmers)
        {
            swarm.OnManualActivation();
        }

        yield break;
    }

    //Valeur allant de X à Y en Z temps
    public static void TriggerValue(float timeBeforeStart, float valueStart, float valueStop, float valueTransitionDuration, List<MeshRenderer> swarmersAffecteds)
    {
        Main.Instance.StartCoroutine(TriggerValueCoroutine(timeBeforeStart, valueStart, valueStop, valueTransitionDuration, swarmersAffecteds));
    }

    static IEnumerator TriggerValueCoroutine(float timeBeforeStart, float valueStart, float valueStop, float valueTransitionDuration, List<MeshRenderer> swarmersAffecteds)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        float purcentageTransition = 0;
        while (purcentageTransition < 1)
        {
            purcentageTransition += Time.deltaTime / valueTransitionDuration;
            Mathf.Lerp(valueStart, valueStop, purcentageTransition);

            foreach (var swarmer in swarmersAffecteds)
            {

            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }
}

