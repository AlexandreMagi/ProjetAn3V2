using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSoundHandler : MonoBehaviour
{
    private static WindSoundHandler _instance;
    public static WindSoundHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    AudioSource wind = null;
    [SerializeField] float timeToMaxPitch = 3;
    float purcentage = 0;
    [SerializeField] float maxPitchGoTo = 1;
    [SerializeField] float startPitch = 0.5f;


    void StartSound()
    {
        wind = CustomSoundManager.Instance.PlaySound("SE_Wind", "Ambiant", 0.8f);
        if (wind != null) wind.pitch = startPitch;
    }

    // Update is called once per frame
    void Update()
    {
        if (wind != null)
        {
            if (purcentage < 1)
            {
                purcentage += Time.deltaTime / timeToMaxPitch;
                wind.pitch = Mathf.Lerp(startPitch, maxPitchGoTo, purcentage);
                if (purcentage > 1)
                {
                    purcentage = 1;
                    wind.pitch = maxPitchGoTo;
                    Cut();
                }
            }
        }
    }

    public void Cut()
    {
        if (wind != null) wind.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        StartSound();
    }
}
