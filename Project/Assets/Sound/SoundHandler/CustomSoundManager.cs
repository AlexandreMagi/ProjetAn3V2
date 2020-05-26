using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public class CustomSoundManager : MonoBehaviour
{

    #region var

    [SerializeField, PropertyRange(0f,1f)] float mainVolume = .8f;
    float currentPitch = 1;
    float speedPitchChanges = 0.8f;
    float purcentagePitched = 0.5f;

    [SerializeField] AudioMixer mixer = null;
    [SerializeField] AudioClip[] allSounds = new AudioClip[0];
    List<AudioSource> allSources = new List<AudioSource>();

    float globalMultiplierForVolumes = 0.4f;
    public float GlobalMultiplierForVolumes { get { return globalMultiplierForVolumes; } }

    [SerializeField] AudioMixerGroup[] mixerGroups = new AudioMixerGroup[0];
    [SerializeField] Transform defaultParent = null;

    [SerializeField] float timeBetweenCheckIfAudioSourceArePlaying = 3;
    float timeRemainingBeforeAudioSourceCheck = -1;

    #endregion

    public static CustomSoundManager Instance { get; private set; }
    void Awake() { Instance = this; }

    void Start()
    {
        if (defaultParent == null && CameraHandler.Instance != null) defaultParent = CameraHandler.Instance.renderingCam.transform;
    }

    void Update()
    {
        currentPitch = Mathf.MoveTowards(currentPitch, (1 - purcentagePitched) + Time.timeScale * purcentagePitched, Time.unscaledDeltaTime * speedPitchChanges);
        mixer.SetFloat("MainVolume", mainVolume * 100 - 80);
        mixer.SetFloat("MainPitch", currentPitch);
    }

    #region Play Sound Functions

    public AudioSource PlaySound(string soundName, string mixerGroupName, float volume = 1)
    {
        return ActuallyPlaySound(FindClip(soundName), FindAudioMixerGroup(mixerGroupName), false, 1, volume);
    }
    public AudioSource PlaySound(string soundName, string mixerGroupName, Transform parent = null, float volume = 1, bool loop = false, float pitch = 1, float pitchRandom = 0, float pitchConstantModifier = 0, int maxSameSoundPlayedAtTheSameTime = 0)
    {
        return ActuallyPlaySound(FindClip(soundName), FindAudioMixerGroup(mixerGroupName), loop, calculatePitch(pitch, pitchRandom, pitchConstantModifier), volume, parent, maxSameSoundPlayedAtTheSameTime);
    }
    public AudioSource PlaySound(AudioClip soundClip, string mixerGroupName, Transform parent = null, float volume = 1, bool loop = false, float pitch = 1, float pitchRandom = 0, float pitchConstantModifier = 0, int maxSameSoundPlayedAtTheSameTime = 0)
    {
        return ActuallyPlaySound(soundClip, FindAudioMixerGroup(mixerGroupName), loop, calculatePitch(pitch, pitchRandom, pitchConstantModifier), volume, parent, maxSameSoundPlayedAtTheSameTime);
    }
    public AudioSource PlaySound(string soundName, AudioMixerGroup mixerGroup, Transform parent = null, float volume = 1, bool loop = false, float pitch = 1, float pitchRandom = 0, float pitchConstantModifier = 0, int maxSameSoundPlayedAtTheSameTime = 0)
    {
        return ActuallyPlaySound(FindClip(soundName), mixerGroup, loop, calculatePitch(pitch, pitchRandom, pitchConstantModifier), volume, parent, maxSameSoundPlayedAtTheSameTime);
    }
    public AudioSource PlaySound(AudioClip soundClip, AudioMixerGroup mixerGroup, Transform parent = null, float volume = 1, bool loop = false, float pitch = 1, float pitchRandom = 0, float pitchConstantModifier = 0, int maxSameSoundPlayedAtTheSameTime = 0)
    {
        return ActuallyPlaySound(soundClip, mixerGroup, loop, calculatePitch(pitch, pitchRandom, pitchConstantModifier), volume, parent, maxSameSoundPlayedAtTheSameTime);
    }

    #endregion

    #region Utilitary Functions

    bool checkIfSoundAlreadyPlayed (string soundName, int nbTimeMax)
    {
        int nbCurrPlaying = 0;
        for (int i = 0; i < allSources.Count; i++)
        {
            if (allSources[i] != null && allSources[i].isPlaying && allSources[i].clip.name == soundName) nbCurrPlaying++;
        }
        if (nbCurrPlaying >= nbTimeMax)
        {
            Debug.Log("Error : Too many sound played at the same time. Sound named '" + soundName + "' will not be played");
            return true;
        }
        return false;
    }

    float calculatePitch(float pitch, float pitchRandom, float pitchConstantModifier) { return pitch + Random.Range(-pitchRandom, pitchRandom) + pitchConstantModifier; }

    AudioMixerGroup FindAudioMixerGroup(string mixerGroupName)
    {
        for (int i = 0; i < mixerGroups.Length; i++)
        {
            if (mixerGroups[i].name == mixerGroupName) return mixerGroups[i];
        }
        Debug.Log("Error : Audio Mixer named '" + mixerGroupName + "' does not exist");
        return null;
    }

    AudioClip FindClip(string soundName)
    {
        for (int i = 0; i < allSounds.Length; i++)
        {
            if (allSounds[i] != null && allSounds[i].name == soundName)
            {
                return allSounds[i];
            }
        }
        if (soundName != "") Debug.Log("Error : Audio Clip named '" + soundName + "' does not exist or isn't in the sound handler script");
        return null;
    }

    AudioSource FindAudioSource()
    {
        for (int i = 0; i < allSources.Count; i++)
        {
            if (allSources[i] != null && !allSources[i].isPlaying)
            {
                return allSources[i];
            }
        }

        GameObject newObj = new GameObject();
        AudioSource newAudioSource = newObj.AddComponent<AudioSource>();
        allSources.Add(newAudioSource);
        return newAudioSource;
    }

    void PauseAllNonPlayingAudioSource()
    {
        timeRemainingBeforeAudioSourceCheck = timeBetweenCheckIfAudioSourceArePlaying;
        for (int i = 0; i < allSources.Count; i++)
        {
            if (allSources[i] != null && !allSources[i].isPlaying)
            {
                allSources[i].Pause();
            }
        }
    }

    #endregion

    AudioSource ActuallyPlaySound(AudioClip clip, AudioMixerGroup mixerGroup = null, bool loop = false, float pitch = 1, float volume = 1, Transform parent = null, int maxSameSoundPlayedAtTheSameTime = 0)
    {
        if (clip != null)
        {
            if (parent == null)
            {
                if (defaultParent != null) parent = defaultParent;
                else Debug.Log("No parent found for sound '" + clip.name + "'");
            }
            if (maxSameSoundPlayedAtTheSameTime > 0 && checkIfSoundAlreadyPlayed(clip.name, maxSameSoundPlayedAtTheSameTime)) return null;
            AudioSource currentSource = FindAudioSource();

            currentSource.gameObject.name = clip.name + "SoundSource";
            currentSource.clip = clip;
            if (mixerGroup != null) currentSource.outputAudioMixerGroup = mixerGroup;
            currentSource.volume = volume * GlobalMultiplierForVolumes;
            currentSource.spatialBlend = 0;
            currentSource.minDistance = 1;
            currentSource.loop = loop;
            currentSource.pitch = pitch;
            currentSource.transform.parent = parent;
            currentSource.transform.position = parent != null ? parent.position : Vector3.zero;
            currentSource.enabled = true;
            currentSource.Play();
            if (timeRemainingBeforeAudioSourceCheck < 0)
                PauseAllNonPlayingAudioSource();
            return currentSource;
        }
        return null;
    }

}