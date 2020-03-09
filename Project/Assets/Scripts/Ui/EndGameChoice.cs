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

    public void SetupChoice(int publicMalus, int purcentageChance)
    {
        rootGameEnd.SetActive(true);
        publicMalusText.text = "- " + publicMalus;
        publicChanceSurvival.text = purcentageChance + " %";
        countdown.text = Mathf.RoundToInt(Main.Instance.TimeRemainingBeforeGameOver).ToString();
    }

    public void EndChoice()
    {
        rootGameEnd.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        countdown.text = Mathf.RoundToInt(Main.Instance.TimeRemainingBeforeGameOver).ToString();
        if (countdown.text != lastText)
            bumpAnimPurcentage = 0;
        lastText = countdown.text;

        if (bumpAnimPurcentage < 1)
        {
            countdown.transform.localScale = Vector3.one + Vector3.one * bumpAnimCurve.Evaluate(bumpAnimPurcentage) * bumpAnimAmplitude;
            bumpAnimPurcentage += Time.unscaledDeltaTime / bumpAnimTime;
            if (bumpAnimPurcentage > 1)
            {
                bumpAnimPurcentage = 1;
                countdown.transform.localScale = Vector3.one;
            }
        }
    }
}
