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
    public static void TriggerSpawners(float timeBeforeStart, Spawner[] spawners, bool state)
    {
        Main.Instance.StartCoroutine(TriggerSpawnersCoroutine(timeBeforeStart, spawners, state));
    }

    static IEnumerator TriggerSpawnersCoroutine(float timeBeforeStart, Spawner[] spawners, bool state)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach (Spawner spawner in spawners)
        {
            if (state)
                spawner.StartSpawn();
            else
                spawner.StopSpawn();
        }

        yield break;
    }

    //CHANGEMENT DE VOLUME DE PUBLIC
    public static void TriggerPublicSound(float timeBeforeStart, float volumeAimed, float volumeTimeTransition)
    {
        Main.Instance.StartCoroutine(TriggerPublicSoundCoroutine(timeBeforeStart, volumeAimed, volumeTimeTransition));
    }

    static IEnumerator TriggerPublicSoundCoroutine(float timeBeforeStart, float volumeAimed, float volumeTimeTransition)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        PublicLoopSoundHandler.Instance.ChangePublicVolume(volumeAimed, volumeTimeTransition);

        yield break;
    }

    //CHANGEMENT DE FOG
    public static void TriggerFog(float timeBeforeStart, float fogEndValueAimed, float fogTimeTransition)
    {
        Main.Instance.StartCoroutine(TriggerFogCoroutine(timeBeforeStart, fogEndValueAimed, fogTimeTransition));
    }

    static IEnumerator TriggerFogCoroutine(float timeBeforeStart, float fogEndValueAimed, float fogTimeTransition)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        float pastValue = RenderSettings.fogEndDistance;
        float completion = 0;
        if (fogTimeTransition != 0)
        {
            while(completion < 1)
            {
                //Debug.Log(RenderSettings.fogEndDistance);
                completion += Time.deltaTime / fogTimeTransition;
                completion = Mathf.Clamp01(completion);
                RenderSettings.fogEndDistance = Mathf.Lerp(pastValue, fogEndValueAimed, AnimationCurve.EaseInOut(0, 0, 1, 1).Evaluate(completion));
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            RenderSettings.fogEndDistance = fogEndValueAimed;
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

    //MULTIPLE ANIMATIONS
    public static void TriggerAnimators(float timeBeforeStart, Animator[] animators, AnimationClip[] clips, float[] delays)
    {
        Main.Instance.StartCoroutine(TriggerAnimatorsCoroutine(timeBeforeStart, animators, clips, delays));
    }

    static IEnumerator TriggerAnimatorsCoroutine(float timeBeforeStart, Animator[] animators, AnimationClip[] clips, float[] delays)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        if(clips.Length - 1 != delays.Length)
        {
            Debug.LogError("Mauvais remplissage des conditions d'animation multiples.");
            yield break;
        }
        else
        {
            int currentAnimationIndex = 0;
            do
            {
                foreach(Animator animatorCurrent in animators)
                {
                    AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animatorCurrent.runtimeAnimatorController);
                    animatorCurrent.runtimeAnimatorController = animatorOverrideController;
                    animatorOverrideController[animatorCurrent.runtimeAnimatorController.animationClips[0].name] = clips[currentAnimationIndex];

                    animatorCurrent.SetTrigger("MakeAction");
                }

                if (currentAnimationIndex < delays.Length)
                yield return new WaitForSeconds(delays[currentAnimationIndex]);

                currentAnimationIndex++;
            }
            while (currentAnimationIndex < clips.Length);

            //yield return new WaitForSeconds(5f);

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
    public static void TriggerSound(float timeBeforeStart, string soundPlayed, string soundMixer, float volume, bool loop = false)
    {
        Main.Instance.StartCoroutine(TriggerSoundCoroutine(timeBeforeStart, soundPlayed, soundMixer, volume, loop));
    }

    static IEnumerator TriggerSoundCoroutine(float timeBeforeStart, string soundName, string soundMixer, float volume, bool loop = false)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, soundName, false, volume);
        CustomSoundManager.Instance.PlaySound(soundName, soundMixer,null, volume, loop);

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

    //Kill du joueur
    public static void TriggerInstantKill(float timeBeforeStart, bool preventRevive)
    {
        Main.Instance.StartCoroutine(TriggerInstantKillCoroutine(timeBeforeStart, preventRevive));
    }

    static IEnumerator TriggerInstantKillCoroutine(float timeBeforeStart, bool preventRevive)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        if (preventRevive)Main.Instance.PreventPlayerFromRevive();
        Player.Instance.SetLifeTo(1);
        Player.Instance.GainArmor(-9999);
        Player.Instance.TakeDamage(1);

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


    //Animation swarmer trigger
    public static void TriggerAnimationOnSwarmers(float timeBeforeStart, SwarmerProceduralAnimation.AnimSwarmer animation, List<Swarmer> swarmers)
    {
        Main.Instance.StartCoroutine(TriggerAnimationOnSwarmersCoroutine(timeBeforeStart, animation, swarmers));
    }

    static IEnumerator TriggerAnimationOnSwarmersCoroutine(float timeBeforeStart, SwarmerProceduralAnimation.AnimSwarmer animation, List<Swarmer> swarmers)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach (Swarmer swarm in swarmers)
        {
            swarm.ForcePlayAnimation(animation);
		}
	}

    //Valeur allant de X à Y en Z temps
    public static void TriggerValue(float timeBeforeStart, float valueStart, float valueStop, float valueTransitionDuration, List<Renderer> meshAffecteds, string shaderValueName, bool isSwarmer)
    {
        Main.Instance.StartCoroutine(TriggerValueCoroutine(timeBeforeStart, valueStart, valueStop, valueTransitionDuration, meshAffecteds, shaderValueName, isSwarmer));
    }

    static IEnumerator TriggerValueCoroutine(float timeBeforeStart, float valueStart, float valueStop, float valueTransitionDuration, List<Renderer> meshAffecteds, string shaderValueName, bool isSwarmer)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        float purcentageTransition = 0;
        while (purcentageTransition < 1)
        {
            purcentageTransition += Time.deltaTime / valueTransitionDuration;
            float value = Mathf.Lerp(valueStart, valueStop, purcentageTransition);

            foreach (var me in meshAffecteds)
            {
                if (isSwarmer)
                    me.materials[1].SetFloat(shaderValueName, value);
                else
                    me.material.SetFloat(shaderValueName, value);
            }

            yield return new WaitForEndOfFrame();
        }

        yield break;
    }

    //LIGHTS
    public static void TriggerLights(float timeBeforeStart, Light[] lightsToAffect, bool lightsState, bool lightChangesColor, Color colorOfLight)
    {
        Main.Instance.StartCoroutine(TriggerLightsCoroutine(timeBeforeStart, lightsToAffect, lightsState, lightChangesColor, colorOfLight));
    }

    static IEnumerator TriggerLightsCoroutine(float timeBeforeStart, Light[] lightsToAffect, bool lightsState, bool lightChangesColor, Color colorOfLight)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        foreach (Light l in lightsToAffect)
        {
            l.enabled = lightsState;
            if (lightChangesColor)
            {
                l.color = colorOfLight;
            }
            LightHandler lh = l.GetComponent<LightHandler>();
            if (lh)
            {
                lh.enabled = lightsState;
            }
        }

        yield break;
    }

    //Camera near clip
    public static void TriggerNearClipChange(float timeBeforeStart, Camera cameraToChange, float newNearClip)
    {
        Main.Instance.StartCoroutine(TriggerNearClipChangeCoroutine(timeBeforeStart, cameraToChange, newNearClip));
    }

    static IEnumerator TriggerNearClipChangeCoroutine(float timeBeforeStart, Camera cameraToChange, float newNearClip)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        cameraToChange.nearClipPlane = newNearClip;

        yield break;
    }

    // Parenting
    public static void TriggerParenting(float timeBeforeStart, Transform _child, Transform _parent)
    {
        Main.Instance.StartCoroutine(TriggerParentingCoroutine(timeBeforeStart, _child, _parent));
    }

    static IEnumerator TriggerParentingCoroutine(float timeBeforeStart, Transform _child, Transform _parent)
    {
        yield return new WaitForSeconds(timeBeforeStart);
        _child.SetParent(_parent, true);
        yield break;
    }

    //End of game
    public static void TriggerEndOfGame(float timeBeforeStart)
    {
        Main.Instance.StartCoroutine(TriggerEndOfGameCoroutine(timeBeforeStart));
    }

    static IEnumerator TriggerEndOfGameCoroutine(float timeBeforeStart)
    {
        yield return new WaitForSeconds(timeBeforeStart);

        Main.Instance.InitLeaderboard();

        yield break;
    }
}

