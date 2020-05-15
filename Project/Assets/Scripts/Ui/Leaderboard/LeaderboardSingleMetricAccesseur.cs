using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSingleMetricAccesseur : MonoBehaviour
{
    public Text metricType = null;
    public Text currValueText = null;
    public Text maxValueText = null;
    public Transform rootGraph = null;

    //Anim vars
    float idleDelay = 0;
    float idleAmplitude = 0;
    float idleSpeed = 0;
    Vector3 currBaseScale = Vector3.zero;

    float timeBeforePop = -1;
    float popLerpSpeed = -1;

    public void SetupTexts(string type, string currValue, string maxValue)
    {
        metricType.text = type;
        currValueText.text = currValue;
        maxValueText.text = "/"+maxValue;
    }
    public void Init(float _idleDelay, float _idleAmplitude, float _idleSpeed, float delayBeforePop, float _popLerpSpeed)
    {
        currBaseScale = new Vector3(1, 0, 1);
        idleDelay = _idleDelay;
        idleAmplitude = _idleAmplitude;
        idleSpeed = _idleSpeed;
        timeBeforePop = delayBeforePop;
        popLerpSpeed = _popLerpSpeed;
    }

    public void SetTextColor (Color colorSend)
    {
        metricType.color = colorSend;
        currValueText.color = colorSend;
        maxValueText.color = colorSend;
    }

    private void Start()
    {
        rootGraph.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (timeBeforePop > 0)
        {
            timeBeforePop -= Time.unscaledDeltaTime;
            if (timeBeforePop < 0)
            {
                timeBeforePop = 0;
                CustomSoundManager.Instance.PlaySound("Se_ProgressTick", "Leaderboard", null, .5f, false, 1);
            }
        }
        if (timeBeforePop == 0)
        {
            currBaseScale = Vector3.Lerp(currBaseScale, Vector3.one, Time.unscaledDeltaTime * popLerpSpeed);
        }
        rootGraph.localScale = currBaseScale + Vector3.one * Mathf.Sin(Time.unscaledTime * idleSpeed + idleDelay) * idleAmplitude * currBaseScale.y;
    }

}
