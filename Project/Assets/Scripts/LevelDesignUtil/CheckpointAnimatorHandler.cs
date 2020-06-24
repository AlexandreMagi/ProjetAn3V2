using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CheckpointAnimatorHandler : MonoBehaviour
{

    [SerializeField] Animator animatorThatReceiveTriggers = null;
    [SerializeField] string[] triggersToCall = null;
    [SerializeField] float[] triggersTimers = null;
    [SerializeField] bool playOnlyOnce = true;
    bool canPlay = true;

    [SerializeField] string[] soundToPlay = null;
    [SerializeField] float[] soundDelay = null;
    [SerializeField] float[] soundVolume = null;

    [SerializeField] VideoPlayer videoPlayerTuto = null;
    [SerializeField] VideoClip videoArduinoOne = null;
    [SerializeField] VideoClip videoArduinoTwo = null;
    [SerializeField] VideoClip videoKeyboard = null;

    private void Start()
    {
        videoPlayerTuto.Pause();
        UpdateClip();
    }

    void UpdateClip()
    {
        if (videoPlayerTuto != null && !videoPlayerTuto.isPlaying)
        {
            if (Main.Instance.IsArduinoMod)
            {
                if (videoArduinoOne != null) videoPlayerTuto.clip = videoArduinoOne;
            }
            else if (videoKeyboard != null)
                videoPlayerTuto.clip = videoKeyboard;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canPlay)
        {
            if (playOnlyOnce) canPlay = false;
            if (videoPlayerTuto != null && !videoPlayerTuto.isPlaying)
            {
                UpdateClip();
                videoPlayerTuto.Play();
            }
            for (int i = 0; i < triggersToCall.Length; i++)
            {
                StartCoroutine(CallTrigger(Animator.StringToHash(triggersToCall[i]), (i < triggersTimers.Length) ? triggersTimers[i] : 0));
            }
            for (int i = 0; i < soundToPlay.Length; i++)
            {
                StartCoroutine(PlaySound(soundToPlay[i], soundDelay[i], soundVolume[i]));
            }
        }
    }

    IEnumerator CallTrigger(int trigger, float timer)
    {
        if (timer > 0) yield return new WaitForSeconds(timer);
        if (animatorThatReceiveTriggers != null)
            animatorThatReceiveTriggers.SetTrigger(trigger);
        yield break;
    }
    
    IEnumerator PlaySound(string soundName, float soundDelay, float soundVolume)
    {
        if (soundDelay > 0) yield return new WaitForSeconds(soundDelay);
        if (CustomSoundManager.Instance != null) 
            CustomSoundManager.Instance.PlaySound(soundName, "Ambiant", soundVolume);
        yield break;
    }

}
