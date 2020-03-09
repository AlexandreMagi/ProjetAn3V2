using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameChoice : MonoBehaviour
{
    public static EndGameChoice Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] GameObject rootGameEnd = null;
    [SerializeField] Text publicMalusText = null;
    [SerializeField] Text publicChanceSurvival = null;
    [SerializeField] Text countdown = null;

    string lastText = "";

    [SerializeField] AnimationCurve bumpAnimCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float bumpAnimTime = 0.2f;
    [SerializeField] float bumpAnimAmplitude = 0.2f;
    float bumpAnimPurcentage = 1;

    [SerializeField] Color colorBase = Color.white;
    [SerializeField] Color colorDanger = Color.red;
    [SerializeField] float dangerTransitionLerpSpeed = 5;
    [SerializeField] int remainingSecondDanger = 5;
    [SerializeField] float dangerScale = 1.5f;
    float baseScaleCoutdown = 1;

    [SerializeField] Animator anmtrDisplay = null;

    public void SetupChoice(int publicMalus, int purcentageChance)
    {
        rootGameEnd.SetActive(true);
        publicMalusText.text = "- " + publicMalus;
        publicChanceSurvival.text = purcentageChance + " %";
        countdown.text = Mathf.RoundToInt(Main.Instance.TimeRemainingBeforeGameOver).ToString();
        anmtrDisplay.SetTrigger("Pop");
    }

    public void ActivateChoice()
    {
        //anmtrDisplay.StopPlayback();
        anmtrDisplay.enabled = false;
    }

    public void EndChoice()
    {
        rootGameEnd.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        countdown.text = Mathf.CeilToInt(Main.Instance.TimeRemainingBeforeGameOver).ToString();
        if (countdown.text != lastText)
            bumpAnimPurcentage = 0;
        lastText = countdown.text;

        if (Mathf.CeilToInt(Main.Instance.TimeRemainingBeforeGameOver) <= remainingSecondDanger)
        {
            countdown.color = Color.Lerp(countdown.color, colorDanger, Time.unscaledDeltaTime * dangerTransitionLerpSpeed);
            baseScaleCoutdown = Mathf.Lerp(baseScaleCoutdown, dangerScale, Time.unscaledDeltaTime * dangerTransitionLerpSpeed);
        }
        else
        {
            countdown.color = colorBase;
            baseScaleCoutdown = 1;
        }

        if (bumpAnimPurcentage < 1)
        {
            countdown.transform.localScale = Vector3.one * baseScaleCoutdown + Vector3.one * bumpAnimCurve.Evaluate(bumpAnimPurcentage) * bumpAnimAmplitude;
            bumpAnimPurcentage += Time.unscaledDeltaTime / bumpAnimTime;
            if (bumpAnimPurcentage > 1)
            {
                bumpAnimPurcentage = 1;
                countdown.transform.localScale = Vector3.one * baseScaleCoutdown;
            }
        }
    }
}
