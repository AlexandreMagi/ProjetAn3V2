using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public static MusicHandler Instance { get; private set; }
    void Awake() { Instance = this; }

    [SerializeField] AudioSource musicSource = null;
    public enum Musics { none, dropAndMinigun, drone, explo };
    [SerializeField] AudioClip dropAndMinigunMusic = null;
    [SerializeField] AudioClip drone = null;
    [SerializeField] AudioClip explo = null;


    float currMusicVolume = 1;
    MusicRequest currMusicRequest = null;

    public enum TransitionState { delay, fadingOut, waiting, fadingIn, none };
    TransitionState currState = TransitionState.none;
    float completionState = 0;
    float savedVolume = 0;

    void Start()
    {
         
    }

    public void PlayMusic(Musics musicToPlay, float delay, float fadeOut, float timeWaitBetween, float fadeIn, float volume, bool doItNow = false, bool loop = false)
    { 
        currMusicRequest = new MusicRequest(musicToPlay, delay, fadeOut, timeWaitBetween, fadeIn, volume, doItNow, loop);
    }

    void Update()
    {
        if (currMusicRequest!= null)
        {
            //Debug.Log("Curr State is = " + currState + " / " + Mathf.RoundToInt(completionState * 100) + "%");
            switch (currState)
            {
                // ---
                case TransitionState.delay:
                    if (currMusicRequest.delay != 0)
                        completionState += Time.deltaTime / currMusicRequest.delay;
                    if (completionState > 1 || currMusicRequest.delay == 0)
                    {
                        currState = currMusicRequest.doItNow ? TransitionState.fadingOut : TransitionState.waiting;
                        completionState = 0;
                    }
                    break;


                // ---
                case TransitionState.fadingOut:
                    if (currMusicRequest.fadeOut != 0)
                        completionState += Time.deltaTime / currMusicRequest.fadeOut;
                    if (completionState > 1 || currMusicRequest.fadeOut == 0)
                    {
                        currState = TransitionState.waiting;
                        completionState = 0;
                        currMusicVolume = 0;
                    }
                    else currMusicVolume = Mathf.Lerp(savedVolume, 0, completionState);
                    break;


                // ---
                case TransitionState.waiting:
                    if (currMusicRequest.timeWaitBetween != 0)
                        completionState += Time.deltaTime / currMusicRequest.timeWaitBetween;
                    currMusicVolume = 0;
                    if (completionState > 1 || currMusicRequest.timeWaitBetween == 0)
                    {
                        ChangeMusic(currMusicRequest.musicToPlay, currMusicRequest.loop);
                        currState = TransitionState.fadingIn;
                        completionState = 0;
                    }
                    break;


                // ---
                case TransitionState.fadingIn:
                    if (currMusicRequest.fadeIn != 0)
                        completionState += Time.deltaTime / currMusicRequest.fadeIn;
                    if (completionState > 1 || currMusicRequest.fadeIn == 0)
                    {
                        currState = TransitionState.none;
                        completionState = 0;
                        currMusicVolume = 1 * currMusicRequest.volume;
                        currMusicRequest = null;
                    }
                    else currMusicVolume = completionState * currMusicRequest.volume;
                    break;


                // ---
                case TransitionState.none:
                    if (currMusicRequest.doItNow || !musicSource.isPlaying)
                    {
                        //currState = currMusicRequest.doItNow ? TransitionState.fadingOut : TransitionState.waiting;
                        currState = TransitionState.delay;
                        completionState = 0;
                        savedVolume = currMusicVolume;
                    }
                    break;
            }
        }
        else
        {
            currState = TransitionState.none;
            completionState = 0;
        }
        if (musicSource != null)
        {
            musicSource.volume = currMusicVolume;
        }
    }

    void ChangeMusic(Musics musicToPlay, bool loop)
    {
        if (musicSource != null)
        {
            //Debug.Log("Change Music");
            switch (musicToPlay)
            {
                case Musics.none:
                    musicSource.clip = null;
                    break;
                case Musics.dropAndMinigun:
                    musicSource.clip = dropAndMinigunMusic;
                    break;
                case Musics.drone:
                    musicSource.clip = drone;
                    break;
                case Musics.explo:
                    musicSource.clip = explo;
                    break;
            }
            musicSource.Stop();
            musicSource.volume = currMusicVolume;
            musicSource.loop = loop;
            musicSource.Play();
        }

    }

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
