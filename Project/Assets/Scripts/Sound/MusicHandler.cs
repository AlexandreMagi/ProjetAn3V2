using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler Instance { get; private set; }
    void Awake() { Instance = this; }

    [SerializeField] AudioSource musicSource = null;
    public enum Musics { none, dropAndMinigun, drone, explo, lastStage, preLastStage, introPreLastStage };
    [SerializeField] AudioClip dropAndMinigunMusic = null;
    [SerializeField] AudioClip drone = null;
    [SerializeField] AudioClip explo = null;
    [SerializeField] AudioClip lastStage = null;
    [SerializeField] AudioClip preLastStage = null;
    [SerializeField] AudioClip introPreLastStage = null;

    public enum TransitionState { delay, fadingOut, waiting, fadingIn, none };

    [SerializeField] int maxChannel = 3;
    MusicHandlerInstance[] allChannel = null;

    void Start()
    {
        allChannel = new MusicHandlerInstance[maxChannel];
        for (int i = 0; i < allChannel.Length; i++)
        {
            allChannel[i] = new MusicHandlerInstance();
            allChannel[i].musicSource = Instantiate(musicSource, musicSource.transform.parent);
        }
    }

    public void PlayMusic(int channel, Musics musicToPlay, float delay, float fadeOut, float timeWaitBetween, float fadeIn, float volume, bool doItNow = false, bool loop = false)
    {
        //Debug.Log("Request Music");
        if (channel >= 0 && channel < allChannel.Length)
        {
            allChannel[channel].currMusicRequest = new MusicRequest(musicToPlay, delay, fadeOut, timeWaitBetween, fadeIn, volume, doItNow, loop);
            allChannel[channel].volumeTimeTransition = 0;
        }
    }

    void Update()
    {
        for (int i = 0; i < allChannel.Length; i++)
        {
            if (allChannel[i].currMusicRequest != null && allChannel[i] != null)
            {
                //Debug.Log("Curr State is = " + currState + " / " + Mathf.RoundToInt(completionState * 100) + "%");
                switch (allChannel[i].currState)
                {
                    // ---
                    case TransitionState.delay:
                        if (allChannel[i].currMusicRequest.delay != 0)
                            allChannel[i].completionState += Time.deltaTime / allChannel[i].currMusicRequest.delay;
                        if (allChannel[i].completionState > 1 || allChannel[i].currMusicRequest.delay == 0)
                        {
                            allChannel[i].currState = allChannel[i].currMusicRequest.doItNow ? TransitionState.fadingOut : TransitionState.waiting;
                            allChannel[i].completionState = 0;
                        }
                        break;


                    // ---
                    case TransitionState.fadingOut:
                        if (allChannel[i].currMusicRequest.fadeOut != 0)
                            allChannel[i].completionState += Time.deltaTime / allChannel[i].currMusicRequest.fadeOut;
                        if (allChannel[i].completionState > 1 || allChannel[i].currMusicRequest.fadeOut == 0)
                        {
                            allChannel[i].currState = TransitionState.waiting;
                            allChannel[i].completionState = 0;
                            allChannel[i].currMusicVolume = 0;
                        }
                        else allChannel[i].currMusicVolume = Mathf.Lerp(allChannel[i].savedVolume, 0, allChannel[i].completionState);
                        break;


                    // ---
                    case TransitionState.waiting:
                        if (allChannel[i].currMusicRequest.timeWaitBetween != 0)
                            allChannel[i].completionState += Time.deltaTime / allChannel[i].currMusicRequest.timeWaitBetween;
                        allChannel[i].currMusicVolume = 0;
                        if (allChannel[i].completionState > 1 || allChannel[i].currMusicRequest.timeWaitBetween == 0)
                        {
                            ChangeMusic(i,allChannel[i].currMusicRequest.musicToPlay, allChannel[i].currMusicRequest.loop);
                            allChannel[i].currState = TransitionState.fadingIn;
                            allChannel[i].completionState = 0;
                        }
                        break;


                    // ---
                    case TransitionState.fadingIn:
                        if (allChannel[i].currMusicRequest.fadeIn != 0)
                            allChannel[i].completionState += Time.deltaTime / allChannel[i].currMusicRequest.fadeIn;
                        if (allChannel[i].completionState > 1 || allChannel[i].currMusicRequest.fadeIn == 0)
                        {
                            allChannel[i].currState = TransitionState.none;
                            allChannel[i].completionState = 0;
                            allChannel[i].currMusicVolume = 1 * allChannel[i].currMusicRequest.volume;
                            allChannel[i].currMusicRequest = null;
                        }
                        else allChannel[i].currMusicVolume = allChannel[i].completionState * allChannel[i].currMusicRequest.volume;
                        break;


                    // ---
                    case TransitionState.none:
                        if (allChannel[i].currMusicRequest.doItNow || !allChannel[i].musicSource.isPlaying)
                        {
                            //currState = currMusicRequest.doItNow ? TransitionState.fadingOut : TransitionState.waiting;
                            allChannel[i].currState = TransitionState.delay;
                            allChannel[i].completionState = 0;
                            allChannel[i].savedVolume = allChannel[i].currMusicVolume;
                        }
                        break;
                }
            }
            else
            {
                allChannel[i].currState = TransitionState.none;
                allChannel[i].completionState = 0;

                if (allChannel[i].volumeTimeTransition != 0) allChannel[i].currMusicVolume = Mathf.MoveTowards(allChannel[i].currMusicVolume, allChannel[i].aimedVolume, Mathf.Abs(allChannel[i].aimedVolume - allChannel[i].savedVolume) * Time.deltaTime / allChannel[i].volumeTimeTransition);

            }
            if (allChannel[i].musicSource != null)
            {
                allChannel[i].musicSource.volume = allChannel[i].currMusicVolume;
            }
        }

    }

    void ChangeMusic(int channel, Musics musicToPlay, bool loop)
    {
        if (channel >= 0 && channel < allChannel.Length)
        {
            if (allChannel[channel].musicSource != null)
            {
                //Debug.Log("Change Music");
                switch (musicToPlay)
                {
                    case Musics.none:
                        allChannel[channel].musicSource.clip = null;
                        break;
                    case Musics.dropAndMinigun:
                        allChannel[channel].musicSource.clip = dropAndMinigunMusic;
                        break;
                    case Musics.drone:
                        allChannel[channel].musicSource.clip = drone;
                        break;
                    case Musics.explo:
                        allChannel[channel].musicSource.clip = explo;
                        break;
                    case Musics.lastStage:
                        allChannel[channel].musicSource.clip = lastStage;
                        break;
                    case Musics.preLastStage:
                        allChannel[channel].musicSource.clip = preLastStage;
                        break;
                    case Musics.introPreLastStage:
                        allChannel[channel].musicSource.clip = introPreLastStage;
                        break;
                }
                allChannel[channel].musicSource.Stop();
                allChannel[channel].musicSource.volume = allChannel[channel].currMusicVolume;
                allChannel[channel].musicSource.loop = loop;
                allChannel[channel].musicSource.Play();
            }
        }

    }

    public void ChangeMusicVolume (int channel, float volumeAimed, float timeTransition)
    {
        if (channel >= 0 && channel < allChannel.Length)
        {
            allChannel[channel].aimedVolume = volumeAimed;
            allChannel[channel].savedVolume = allChannel[channel].currMusicVolume;
            allChannel[channel].volumeTimeTransition = timeTransition;
        }
    }

}

public class MusicHandlerInstance
{
    public AudioSource musicSource = null;
    public MusicRequest currMusicRequest = null;
    public MusicHandler.TransitionState currState = MusicHandler.TransitionState.none;
    public float currMusicVolume = 1;
    public float completionState = 0;
    public float savedVolume = 0;
    public float aimedVolume = 0;
    public float volumeTimeTransition = 1;
}

public class MusicRequest
{
    public MusicHandler.Musics musicToPlay = MusicHandler.Musics.none;
    public bool doItNow = false;
    public float delay = 0;
    public float fadeOut = 0;
    public float timeWaitBetween = 0;
    public float fadeIn = 0;
    public float volume = 0;
    public bool loop = false;

    public MusicRequest(MusicHandler.Musics _musicToPlay, float _delay, float _fadeOut, float _timeWaitBetween, float _fadeIn, float _volume, bool _doItNow, bool _loop)
    {
        musicToPlay = _musicToPlay;
        delay = _delay;
        fadeOut = _fadeOut;
        timeWaitBetween = _timeWaitBetween;
        fadeIn = _fadeIn;
        volume = _volume;
        doItNow = _doItNow;
        loop = _loop;
    }

}
