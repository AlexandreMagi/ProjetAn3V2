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

    public enum commentType { ReloadPlus, Shotgun, Orb, ZeroG }
    [SerializeField] commentType commentToPlay = commentType.ReloadPlus;
    [SerializeField] float delay = 0;

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

            switch (commentToPlay)
            {
                case commentType.ReloadPlus:
                    Main.Instance.PlayCommentWithDelay(1, "PresA_Borne_Reload_Plus", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(0, "PresB_Reload_Plus", "Comment", Main.Instance.CommentBVolume, delay + 7.2f);
                    SubtitleManager.Instance.SetSubtitle("Reloading correctly is the only subtlety allowed in our sport !", 1, 10f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Exactly, a good timing is key to a longer survival !", 0, 4f, delay + 7.2f);
                    break;
                case commentType.Shotgun:
                    Main.Instance.PlayCommentWithDelay(1, "PresA_Borne_Shotgun", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(0, "PresB_Shotgun", "Comment", Main.Instance.CommentBVolume, delay + 6.5f);
                    SubtitleManager.Instance.SetSubtitle("Few competitors are smart enough to get through this obstacle", 1, 10f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Why is that ? Just just have to hold the trigger !", 0, 4f, delay + 6.5f);
                    break;
                case commentType.Orb:
                    Main.Instance.PlayCommentWithDelay(1, "PresA_Borne_Gravitorb", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(0, "PresB_Borne_Gravitorb", "Comment", Main.Instance.CommentBVolume, delay + 4);
                    SubtitleManager.Instance.SetSubtitle("Finally, the ultimate carnage tool !", 1, 10f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Indeed, the ability to pack hordes of enemies is extremely powerful!", 0, 5.3f, delay + 4);
                    break;
                case commentType.ZeroG:
                    break;
                default:
                    break;
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
