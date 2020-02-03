/////////////////////////////////////////////////////////////////
/// CODE FAIT PAR MAX //////////////
////////////////////////////////////////////////////////////////


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootgunTriggerShoot : MonoBehaviour
{

    bool canStartCoroutine = true;
    [SerializeField]
    string soundPlayed = "";
    [SerializeField]
    float soundVolume = 1;
    [SerializeField]
    float delay = 0;

    [SerializeField]
    float timerBeforeNextSequence = 0.5f;

    bool bSoundPlayed = false;

    public void OnAnimDetection()
    {
        Destroy(GetComponent<Animator>());

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;

        rb.AddForce(new Vector3(200, 0, 0));

        if (!bSoundPlayed)
        {
            bSoundPlayed = true;
            Invoke("PlaySound", delay);
        }

        if (canStartCoroutine)
            StartCoroutine(TimerBeforeNextSequence()); canStartCoroutine = false;

    }

    public void OnAnimDetectionBase()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("MakeAction");
    }


    void PlaySound()
    {
        if (soundPlayed != "")
        {
            CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, soundPlayed, false, soundVolume);
        }
    }

    IEnumerator TimerBeforeNextSequence()
    {
        yield return new WaitForSeconds(timerBeforeNextSequence);

        SequenceHandler.Instance.NextSequence();

        yield break;
    }
}
