using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
    [Header("Sound Played")]
    [ShowIf("sound","")] public AudioClip soundClip = null;
    [HideIf("soundClip")] public string sound = "";

    public AudioMixerGroup mixerGroup = null;

    [Header("Parameters")]
    public bool playOnStart = false;
    [PropertyRange(0f, 2f)] public float volume = 1;
    public bool fade = false;
    [ShowIf("fade")] public bool fadeDependentFromTimeScale = true;
    [PropertyRange(0f, 2f),ShowIf("fade")] public float volumeBase = 1;
    [PropertyRange(0f, 50f), ShowIf("fade")] public float timeToGoTo = 1;
    public bool loop = false;

    public bool changePitch = false;
    [ShowIf("changePitch")] public float pitch = 1;
    [ShowIf("changePitch")] public float pitchRandom = 0;
    [ShowIf("changePitch")] public float pitchConstantAdded = 0;

    public int  maxInstanceThatCanBePlayed = 0;
    [PropertyRange(0f, 1f)] public float spatialBlendOverride = 1;
    public Transform overrideSoundParent = null;

    float currentFadePurcentage = 1;

    [HideInInspector] public AudioSource audioSource = null;

    public bool playedByTrigger = false;
    [SerializeField, ShowIf("playedByTrigger")] LayerMask layerDetect = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (playOnStart) Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFadePurcentage < 1 && audioSource != null && fade)
        {
            currentFadePurcentage += (fadeDependentFromTimeScale ? Time.deltaTime : Time.unscaledDeltaTime) / timeToGoTo;
            if (currentFadePurcentage > 1) currentFadePurcentage = 1;
            audioSource.volume = Mathf.Lerp(volumeBase, volume, currentFadePurcentage) * CustomSoundManager.Instance.GlobalMultiplierForVolumes;
        }
    }

    public void Play()
    {
        if (CustomSoundManager.Instance != null)
        {
            float _pitch = pitch;
            float _pitchRandom = pitchRandom;
            float _pitchConstantAdded = pitchConstantAdded;
            Transform _parent = overrideSoundParent != null ? overrideSoundParent : transform;
            if (!changePitch)
            {
                _pitch = 1;
                _pitchRandom = 0;
                _pitchConstantAdded = 0;
            }

            if (soundClip != null) audioSource = CustomSoundManager.Instance.PlaySound(soundClip, mixerGroup, _parent, volume, loop, _pitch, _pitchRandom, _pitchConstantAdded, maxInstanceThatCanBePlayed);
            else audioSource = CustomSoundManager.Instance.PlaySound(sound, mixerGroup, _parent, volume, loop, _pitch, _pitchRandom, _pitchConstantAdded, maxInstanceThatCanBePlayed);
            currentFadePurcentage = 0;
            audioSource.spatialBlend = spatialBlendOverride;
        }
    }

    private void OnTriggerEnter(Collider other) { if (layerDetect == (layerDetect | (1 << other.gameObject.layer))) { Play(); } }

}
