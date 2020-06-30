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

    bool currSubtitleIndependentFromTimeScale = false;
    float lastACommentLaunched = 0;
    float lastBCommentLaunched = 0;

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
            timeRemainingBeforeReset -= currSubtitleIndependentFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            if (timeRemainingBeforeReset <= 0)
            {
                subtitleText.text = "";
            }
        }
    }

    public void SetSubtitle(string sentence, int streamerID, float timeStay, float delay, bool topPosition = false, bool independentFromTimeScale = false)
    {
        StartCoroutine(SetSubtitleCoroutine(sentence, streamerID,timeStay, delay, topPosition, independentFromTimeScale));
    }

    IEnumerator SetSubtitleCoroutine(string sentence, int streamerID, float timeStay, float delay, bool topPosition = false, bool independentFromTimeScale = false)
    {
        float thisCommentTimeLaunch = Time.time;
        if (streamerID == 0) lastACommentLaunched = thisCommentTimeLaunch;
        else lastBCommentLaunched = thisCommentTimeLaunch;

        subtitleText.text = "";
        if (independentFromTimeScale)
        {
            yield return new WaitForSecondsRealtime(delay);
        }
        else
        {
            yield return new WaitForSeconds(delay);
        }
        bool canLaunchSound = true;
        if (streamerID == 0 && lastACommentLaunched > thisCommentTimeLaunch || streamerID == 1 && lastBCommentLaunched > thisCommentTimeLaunch) canLaunchSound = false;

        if (canLaunchSound)
        {
            currSubtitleIndependentFromTimeScale = independentFromTimeScale;
            subtitleText.text = sentence;
            subtitleText.color = streamerID == 0 ? robotColor : orcColor;
            timeRemainingBeforeReset = timeStay;
            subtitleText.transform.position = topPosition ? topPos.position : botPos.position;
        }
        yield break;
    }

}
