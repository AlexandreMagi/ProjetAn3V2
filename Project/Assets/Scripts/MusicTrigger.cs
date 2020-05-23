using UnityEngine;
using Sirenix.OdinInspector;

public class MusicTrigger : MonoBehaviour
{

    [SerializeField] MusicHandler.Musics musicToPlay = MusicHandler.Musics.none;
    [SerializeField] float timeBeforeDoAnything = 0;
    [SerializeField] bool doItNow = false;
    [SerializeField, ShowIf("doItNow")] float fadeOut = 0;
    [SerializeField] float timeWaitBetween = 0;
    [SerializeField] float fadeIn = 0;
    [SerializeField] float volume = 1;
    [SerializeField] bool loop = false;

    void OnTriggerEnter(Collider other)
    {
        MusicHandler.Instance.PlayMusic(musicToPlay, timeBeforeDoAnything, doItNow ? fadeOut : 0, timeWaitBetween, fadeIn, volume, doItNow, loop);
    }

}
