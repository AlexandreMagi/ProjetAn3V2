using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class EndGameMinigunManager : MonoBehaviour
{

    [SerializeField] TextMeshPro display = null;
    [SerializeField] float timeBeforeCountdownStart = 3;
    [SerializeField] float timeBeforeEndSequence = 15;
    [SerializeField] float timeBeforeSequenceSkip = 3;
    float timeRemaining = 0;
    bool exploded = false;

    [SerializeField] float explosionRadius = 30;
    [SerializeField] float explosionForce = 0;
    [SerializeField] float explosionDamage = 500;
    [SerializeField] float explosionStun = 0;
    [SerializeField] float explosionStunDuration = 0;
    [SerializeField] float explosionLiftValue = 0;

    [SerializeField] string beforeText = "WAITING...";

    [SerializeField] string firstText = "SURVIVE";
    [SerializeField] string lastText = "CONGRATULATION";

    [SerializeField] float clignotementFrequency = 2;
    [SerializeField, PropertyRange(0f,1f)] float clignotementPurcentageDisplay = 0.5f;

    [SerializeField] float fogDelay = 0.1f;
    [SerializeField] float fogValueAimed = 500;
    [SerializeField] float fogTimeTransition = .1f;
    [SerializeField] bool overrideFogColor = false;
    [ShowIf("overrideFogColor"), SerializeField] Color fogColorAimed = Color.white;
    [SerializeField] float fogColorTimeTransition = 1;

    [SerializeField] ParticleSystem exploFinal = null;

    [SerializeField] ManualSpawnerCutter spawnerCutter = null;

    void Start()
    {
        if (display != null) display.text = beforeText;
        timeBeforeEndSequence++;
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining < 0)
            {
                SequenceHandler.Instance.NextSequence();
            }
            else if (timeRemaining < timeBeforeSequenceSkip)
            {
                if (!exploded)
                {
                    exploded = true;
                    Main.Instance.ExplosionFromPlayer(explosionRadius, explosionForce, explosionDamage, explosionStun, explosionStunDuration, explosionLiftValue);
                    TriggerUtil.TriggerFog(fogDelay, fogValueAimed, fogTimeTransition);
                    if (exploFinal != null)
                        exploFinal.Play();
                        
                    TriggerUtil.TriggerFog(fogDelay, fogValueAimed, fogTimeTransition, overrideFogColor ,fogColorAimed, fogColorTimeTransition);
                    if (spawnerCutter != null) spawnerCutter.cutSpawners();
                    Main.Instance.SetupWaitScreenOn();
                }
                if (display != null) display.text = lastText;
            }
            else if (timeRemaining < timeBeforeSequenceSkip + timeBeforeEndSequence)
            {
                if (display != null)
                {
                    display.text = Mathf.FloorToInt(timeRemaining - timeBeforeSequenceSkip).ToString();
                    if (Mathf.FloorToInt(timeRemaining - timeBeforeSequenceSkip) == 0 && Mathf.Repeat(Time.time * clignotementFrequency, 1) > clignotementPurcentageDisplay) display.text = "";
                }
            }
            else if(display != null)
            {
                if (Mathf.Repeat(Time.time * clignotementFrequency, 1) < clignotementPurcentageDisplay) display.text = firstText;
                else display.text = "";
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (timeRemaining == 0)
            timeRemaining = timeBeforeCountdownStart + timeBeforeEndSequence + timeBeforeSequenceSkip;
    }
}
