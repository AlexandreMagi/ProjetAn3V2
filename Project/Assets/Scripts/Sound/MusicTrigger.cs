using UnityEngine;
using Sirenix.OdinInspector;

public class MusicTrigger : MonoBehaviour
{

    [SerializeField] bool doOnlyOnce = true;
    [SerializeField] bool changeMusic = true;

    [SerializeField] int channel = 0;
    [SerializeField, ShowIf("changeMusic")] MusicHandler.Musics musicToPlay = MusicHandler.Musics.none;
    [SerializeField, ShowIf("changeMusic")] float timeBeforeDoAnything = 0;
    [SerializeField, ShowIf("changeMusic")] bool doItNow = false;
    [SerializeField, ShowIf("changeMusic"), ShowIf("doItNow")] float fadeOut = 0;
    [SerializeField, ShowIf("changeMusic")] float timeWaitBetween = 0;
    [SerializeField, ShowIf("changeMusic")] float fadeIn = 0;
    [SerializeField, ShowIf("changeMusic")] float volume = 1;
    [SerializeField, ShowIf("changeMusic")] bool loop = false;

    [SerializeField, HideIf("changeMusic")] float volumeAimed = 1;
    [SerializeField, HideIf("changeMusic")] float volumeTimeTransition = 1;

    void OnTriggerEnter(Collider other)
    {
        if (changeMusic) MusicHandler.Instance.PlayMusic(channel,musicToPlay, timeBeforeDoAnything, doItNow ? fadeOut : 0, timeWaitBetween, fadeIn, volume, doItNow, loop);
        else MusicHandler.Instance.ChangeMusicVolume(channel,volumeAimed, volumeTimeTransition);

        if (doOnlyOnce) gameObject.SetActive(false);
    }

}
