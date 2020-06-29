using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{

    [SerializeField] Text subtitleText = null;
    float timeRemainingBeforeReset = 0;

    public static SubtitleManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        subtitleText.text = "";
    }

    void Update()
    {
        if (timeRemainingBeforeReset > 0)
        {
            timeRemainingBeforeReset -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeReset <= 0)
            {
                subtitleText.text = "";
            }
        }
    }

    public void SetSubtitle (string sentence, float timeStay)
    {
        subtitleText.text = sentence;
        timeRemainingBeforeReset = timeStay;
    }

    public void SetSubtitle (string sentence, float timeStay, float delay)
    {
        subtitleText.text = sentence;
        StartCoroutine(SetSubtitleCoroutine(sentence, timeStay, delay));
    }

    IEnumerator SetSubtitleCoroutine(string sentence, float timeStay, float delay)
    {
        yield return new WaitForSeconds(delay);
        subtitleText.text = sentence;
        timeRemainingBeforeReset = timeStay;
        yield break;
    }

}
