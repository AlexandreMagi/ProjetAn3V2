using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleManager : MonoBehaviour
{

    [SerializeField] Text subtitleText = null;
    float timeRemainingBeforeReset = 0;

    [SerializeField] Color robotColor = Color.white;
    [SerializeField] Color orcColor = Color.white;

    [SerializeField] Transform topPos = null;
    [SerializeField] Transform botPos = null;

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

    public void SetSubtitle (string sentence, int streamerID, float timeStay, bool topPosition = false)
    {
        subtitleText.text = sentence;
        timeRemainingBeforeReset = timeStay;
        subtitleText.text = sentence;
        subtitleText.color = streamerID == 0 ? robotColor : orcColor;
        subtitleText.transform.position = topPosition ? topPos.position : botPos.position;
    }

    public void SetSubtitle (string sentence, int streamerID, float timeStay, float delay, bool topPosition = false)
    {
        StartCoroutine(SetSubtitleCoroutine(sentence, streamerID,timeStay, delay, topPosition));
    }

    IEnumerator SetSubtitleCoroutine(string sentence, int streamerID, float timeStay, float delay, bool topPosition = false)
    {
        subtitleText.text = "";
        yield return new WaitForSecondsRealtime(delay);
        subtitleText.text = sentence;
        subtitleText.color = streamerID == 0? robotColor : orcColor;
        timeRemainingBeforeReset = timeStay;
        subtitleText.transform.position = topPosition ? topPos.position : botPos.position;
        yield break;
    }

}
