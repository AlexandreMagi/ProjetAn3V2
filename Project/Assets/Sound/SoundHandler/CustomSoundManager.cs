using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CustomSoundManager : MonoBehaviour
{

    [SerializeField] AudioClip[] tSounds = new AudioClip[0];
    [SerializeField] string[] tSoundsAmbient = new string[0];
    [SerializeField] GameObject hAudioSourcePrefab = null;
    [SerializeField] bool bStartWithNoSound = false;
    [SerializeField] float fFirstEnvironmentSoundTimeBeforePlay = 1;
    [SerializeField] int nNbAudioSource = 50;
    [SerializeField] Vector2 vRandomTimeBetweenAmbianceSound = new Vector2(2, 5);
    [SerializeField] float fVolumeAmbiance = 0.5f;
    [SerializeField] Vector2 vRandomAndFixedPitchAmbiance = new Vector2(0.5f, 0.4f);
    [SerializeField] string[] tSoundsNonPitchedByTimeScale = new string[0];
    GameObject[] hAudioSources;

    float nTimeFade = 5;
    float fCurrentTimer = 0;
    int nSoundDir = 0;

    [SerializeField,PropertyRange(0.1f, 1f)] float fMaxVolumeModifier = 0.5f;
    float nVolumeModifierLocal = 1;
    float MuteMultiplier = 1;
    bool muted = false;
    bool WasMuted = false;

    float fTimerBeforeNextSound = 5;
    float fCurrentTimerSound = 0;

    private float TimeScaleMultiplier = 1;

    private static CustomSoundManager _instance;
    public static CustomSoundManager Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        hAudioSources = new GameObject[nNbAudioSource];
        for (int i = 0; i < hAudioSources.Length; i++)
        {
            hAudioSources[i] = Instantiate(hAudioSourcePrefab);
            hAudioSources[i].transform.SetParent(this.transform);
        }

        fTimerBeforeNextSound = fFirstEnvironmentSoundTimeBeforePlay;


        if (bStartWithNoSound)
            nVolumeModifierLocal = 0.001f;
        else
        {
            nVolumeModifierLocal = fMaxVolumeModifier;
        }

        TimeScaleMultiplier = 0.5f + Time.timeScale/2;
    }


    public void Mute() { muted = true;  }
    public void _Mute() { MuteMultiplier = 0.0001f; WasMuted = true; }
    public void UnMute() { muted = false; }
    public void _UnMute() { MuteMultiplier = 1; WasMuted = false; }

    // Update is called once per frame
    void Update()
    {
        

        FadeSound(1, 2);
        // ######################################## FADE DE SON  ######################################## //
        #region Fade de Son
        // ----- Enlève l'ancien modifieur local
        if (nVolumeModifierLocal != 0)
        {
            for (int i = 0; i < hAudioSources.Length; i++)
            {
                if (hAudioSources[i] != null)
                {
                    hAudioSources[i].GetComponent<AudioSource>().volume /= nVolumeModifierLocal;
                    hAudioSources[i].GetComponent<AudioSource>().volume /= MuteMultiplier;
                }
                bool bPitchThisSound = true;
                if (hAudioSources[i].GetComponent<AudioSource>().clip != null)
                {
                    for (int j = 0; j < tSoundsNonPitchedByTimeScale.Length; j++)
                    {
                        if (hAudioSources[i].GetComponent<AudioSource>().clip.name == tSoundsNonPitchedByTimeScale[j])
                        {
                            bPitchThisSound = false;
                        }
                    }
                    if (bPitchThisSound)
                        hAudioSources[i].GetComponent<AudioSource>().pitch /= TimeScaleMultiplier;
                }
            }
        }

        // ----- Change la valeur du modifieur local
        fCurrentTimer += Time.deltaTime * nSoundDir;
        if (fCurrentTimer > nTimeFade)
        {
            fCurrentTimer = nTimeFade;
            nSoundDir = 0;
            nVolumeModifierLocal = fMaxVolumeModifier;
        }
        else if (fCurrentTimer < 0.001f)
        {
            fCurrentTimer = 0.001f;
            nSoundDir = 0;
            nVolumeModifierLocal = 0.001f;
        }
        else
            nVolumeModifierLocal = fCurrentTimer * fMaxVolumeModifier / nTimeFade;

        if (WasMuted != muted)
        {
            if (muted) _Mute();
            else _UnMute();
        }

        TimeScaleMultiplier = 0.5f + Time.timeScale / 2;

        // ----- Ré-attribue le modifieur local
        if (nVolumeModifierLocal != 0)
        {
            for (int i = 0; i < hAudioSources.Length; i++)
            {
                if (hAudioSources[i] != null)
                {
                    hAudioSources[i].GetComponent<AudioSource>().volume *= nVolumeModifierLocal;
                    hAudioSources[i].GetComponent<AudioSource>().volume *= MuteMultiplier;
                }

                bool bPitchThisSound = true;
                if (hAudioSources[i].GetComponent<AudioSource>().clip != null)
                {
                    for (int j = 0; j < tSoundsNonPitchedByTimeScale.Length; j++)
                    {
                        if (hAudioSources[i].GetComponent<AudioSource>().clip.name == tSoundsNonPitchedByTimeScale[j])
                            bPitchThisSound = false;
                    }
                    if (bPitchThisSound)
                        hAudioSources[i].GetComponent<AudioSource>().pitch *= TimeScaleMultiplier;
                }
                    
            }
        }

        #endregion

        // --- Joue un son d'environnement tout les "fTimerBeforeNextSound"
        fCurrentTimerSound += Time.deltaTime;
        if (fCurrentTimerSound > fTimerBeforeNextSound) environmentSound();


    }

    public void RemoveSound (string sName)
    {
        for (int i = 0; i < hAudioSources.Length; i++)
        {
            if (hAudioSources[i].GetComponent<AudioSource>().isPlaying && hAudioSources[i].GetComponent<AudioSource>().clip.name == sName)
                hAudioSources[i].GetComponent<AudioSource>().Stop();
        }
    }

    void environmentSound()
    {
        if (tSoundsAmbient.Length != 0)
        {
            fCurrentTimerSound -= fTimerBeforeNextSound;
            fTimerBeforeNextSound = Random.Range(vRandomTimeBetweenAmbianceSound.x, vRandomTimeBetweenAmbianceSound.y);
            PlaySound(CameraHandler.Instance.renderingCam.gameObject, tSoundsAmbient[Random.Range(0, tSoundsAmbient.Length)], false, fVolumeAmbiance, vRandomAndFixedPitchAmbiance.x, vRandomAndFixedPitchAmbiance.y);
        }
    }

    public void StopAllSound()
    {
        for (int i = 0; i < hAudioSources.Length; i++)
        {
            if (hAudioSources[i].GetComponent<AudioSource>().isPlaying)
                hAudioSources[i].GetComponent<AudioSource>().Stop();
        }
    }

    public AudioSource PlaySound(GameObject hSource, string sSoundName, bool bLoop, float fVolume, float fPitchRandom = 0, float fPitchConstantModifier = 0, bool bCanPlayIfAlreadyExisting = true)
    {
        int IndexSound = -1;
        for (int i = 0; i < tSounds.Length; i++)
        {
            if (tSounds[i] != null && tSounds[i].name == sSoundName)
            {
                IndexSound = i;
                break;
            }
        }
        if (IndexSound == -1)
            Debug.LogWarning("ERROR : Sound named (" + sSoundName + ") doesn't exist, please check the call function");
        if (IndexSound >= 0)
        {
            if (!bCanPlayIfAlreadyExisting)
            {
                for (int i = 0; i < hAudioSources.Length; i++)
                {
                    if (hAudioSources[i].GetComponent<AudioSource>().isPlaying && hAudioSources[i].GetComponent<AudioSource>().clip == tSounds[IndexSound])
                    {
                        return hAudioSources[i].GetComponent<AudioSource>();
                    }
                }
            }

            for (int i = 0; i < hAudioSources.Length; i++)
            {
                if (!hAudioSources[i].GetComponent<AudioSource>().isPlaying)
                {
                    hAudioSources[i].transform.SetParent(hSource.transform);
                    hAudioSources[i].transform.position = hSource.transform.position;
                    hAudioSources[i].GetComponent<AudioSource>().clip = tSounds[IndexSound];
                    hAudioSources[i].GetComponent<AudioSource>().loop = bLoop;
                    hAudioSources[i].GetComponent<AudioSource>().pitch = (1 + Random.Range(-fPitchRandom, fPitchRandom) + fPitchConstantModifier);

                    bool bPitchThisSound = true;
                    for (int j = 0; j < tSoundsNonPitchedByTimeScale.Length; j++)
                    {
                        if (hAudioSources[i].GetComponent<AudioSource>().clip.name == tSoundsNonPitchedByTimeScale[j])
                            bPitchThisSound = false;
                    }
                    if (bPitchThisSound)
                        hAudioSources[i].GetComponent<AudioSource>().pitch *= TimeScaleMultiplier;

                    hAudioSources[i].GetComponent<AudioSource>().volume = fVolume * nVolumeModifierLocal * MuteMultiplier;
                    hAudioSources[i].GetComponent<AudioSource>().Play();
                    return hAudioSources[i].GetComponent<AudioSource>();
                }
            }
        }
        return null;
    }

    public void FadeSound(int nDir, float fTime)
    {
        if (nDir != 0)
        {
            if (nDir > 1) nDir = 1;
            if (nDir < -1) nDir = -1;
        }
        if (fTime < 0) fTime = fTime * -1;
        fCurrentTimer = fCurrentTimer * fTime / nTimeFade;
        nSoundDir = nDir;
        nTimeFade = fTime;

    }
}
