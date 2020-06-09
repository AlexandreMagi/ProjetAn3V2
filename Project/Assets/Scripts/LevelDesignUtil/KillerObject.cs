using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerObject : MonoBehaviour
{
    float timeBeforeEndOfMulti = 0;

    [SerializeField]
    bool killEnemyEqualsDamage = false;

    public bool countsAsPlayerKill = true;

    [SerializeField]
    float shakeForceAtVictim = 0;
    [SerializeField]
    float shakeDurationAtVictim = 0;


    [SerializeField]
    string soundToPlayAtStart = "Se_KillerObjectSound";
    [SerializeField]
    float soundToPlayAtStartVolume = 2;
    [SerializeField]
    string soundToPlayAtKill = "Se_KillerObjectKillSound";
    [SerializeField]
    float soundToPlayAtKillVolume = 2;
    [SerializeField]
    float soundMinDistanceListening = 8;
    [SerializeField]
    string soundToPlayAtGoDisable = "";
    [SerializeField]
    float soundToPlayAtDisableVolume = 2;
    [SerializeField]
    float soundToPlayAtDisableMinDistanceListening = 8;

    [SerializeField] float minDistanceToPlayKillSound = 10;
    [SerializeField] float minTimeBetweenKillSound = .5f;
    float timeRemainingBeforeCanPlayKillSound = 0;

    AudioSource ambiantAudioSource = null;


    float timeFadeVolumeAtStart = 5;
    float savedVolume = 0;

    [SerializeField] float shakeCapLimit = .1f;

    public void Start()
    {
        if (CustomSoundManager.Instance != null && soundToPlayAtStart != "") ambiantAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayAtStart, "Ambiant", transform, soundToPlayAtStartVolume, true);
        if (ambiantAudioSource != null)
        {
            ambiantAudioSource.spatialBlend = 1;
            ambiantAudioSource.minDistance = soundMinDistanceListening;
            savedVolume = ambiantAudioSource.volume;
            ambiantAudioSource.volume = 0;
            //ambiantAudioSource.transform.position = transform.position;
        }
    }

    public void Update()
    {
        if (timeRemainingBeforeCanPlayKillSound >= 0) timeRemainingBeforeCanPlayKillSound -= Time.deltaTime;

        if (timeFadeVolumeAtStart != 0 && ambiantAudioSource!=null)
        {
            timeFadeVolumeAtStart = Mathf.MoveTowards(timeFadeVolumeAtStart, 0, Time.deltaTime);
            ambiantAudioSource.volume = Mathf.Lerp(0, savedVolume, 1 - timeFadeVolumeAtStart / 5);
        }

        if (timeBeforeEndOfMulti > 0)
        {
            timeBeforeEndOfMulti -= Time.unscaledDeltaTime;
            if(timeBeforeEndOfMulti <= 0)
            {
                timeBeforeEndOfMulti = 0;
                //PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.EnvironmentKill, Vector3.zero);
            }
        }
    }

    private void OnDisable()
    {
        AudioSource disableAudioSource = null;
        if (CustomSoundManager.Instance != null && soundToPlayAtGoDisable != "") disableAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayAtGoDisable, "Ambiant", CameraHandler.Instance.renderingCam.transform, soundToPlayAtDisableVolume,false,1,.2f,0,1);
        if (disableAudioSource != null)
        {
            disableAudioSource.spatialBlend = 1;
            disableAudioSource.minDistance = soundToPlayAtDisableMinDistanceListening;
            disableAudioSource.transform.position = transform.position;
        }
    }

    void OnTriggerStay(Collider other)
    {
        IEntity otherEnemy = other.GetComponent<IEntity>();
        if (other.GetComponent<IEntity>() != null && other.GetComponent<Player>() == null && other.GetComponent<Prop>() == null)
        {
            
            if (killEnemyEqualsDamage)
            {
                if (other.GetComponent<Swarmer>() != null)
                    Player.Instance.TakeDamage(other.GetComponent<Swarmer>().GetDamage());
            }

            if (countsAsPlayerKill)
            {
                otherEnemy.TakeDamage(999999);
                timeBeforeEndOfMulti = 0.3f;
                if (CameraHandler.Instance != null && shakeForceAtVictim > 0 && shakeDurationAtVictim > 0 && CameraHandler.Instance.CheckIfCanAddShake(shakeCapLimit)) CameraHandler.Instance.AddShake(shakeForceAtVictim, shakeDurationAtVictim);             
            }
            else
            {
                otherEnemy.ForceKill();
            }

        }

        if (otherEnemy != null && other.GetComponent<Prop>() != null)
        {
            otherEnemy.TakeDamage(9999);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IEntity otherEnemy = other.GetComponent<IEntity>();
        if (other.GetComponent<IEntity>() != null && other.GetComponent<Player>() == null && other.GetComponent<Prop>() == null)
        {
            if (soundToPlayAtKill != "" && (CameraHandler.Instance == null || CameraHandler.Instance.GetDistanceWithCam(other.gameObject.transform.position) < minDistanceToPlayKillSound) && timeRemainingBeforeCanPlayKillSound < 0)
            {
                timeRemainingBeforeCanPlayKillSound = minTimeBetweenKillSound;
                AudioSource killAudioSource = CustomSoundManager.Instance.PlaySound(soundToPlayAtKill, "Ambiant", null, soundToPlayAtKillVolume,false,1,0.2f);
                if (killAudioSource!=null)
                {
                    killAudioSource.spatialBlend = 1;
                    killAudioSource.minDistance = soundMinDistanceListening;
                    killAudioSource.transform.position = other.gameObject.transform.position;
                }
            }
        }

    }

}
