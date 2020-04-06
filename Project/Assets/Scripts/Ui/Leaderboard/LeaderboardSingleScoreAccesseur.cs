﻿using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSingleScoreAccesseur : MonoBehaviour
{
    public Image background = null;
    public Outline backgroundOutline = null;
    public Text rankText = null;
    public Text nameText = null;
    public Text titleText = null;
    public Text scoreText = null;

    public Transform rootGraph = null;
    //Anim vars
    float idleDelay = 0;
    float idleAmplitude = 0;
    float idleSpeed = 0;
    Vector3 currBaseScale = Vector3.zero;

    float timeBeforePop = -1;
    float animPopSpeedLerp = 1;

    private void Start()
    {
        rootGraph.localScale = Vector3.zero;
    }

    public void Init(float _idleDelay, float _idleAmplitude, float _idleSpeed, float delayBeforePop, float _popLerpSpeed)
    {
        currBaseScale = new Vector3(1, 0, 1);
        idleDelay = _idleDelay;
        idleAmplitude = _idleAmplitude;
        idleSpeed = _idleSpeed;
        timeBeforePop = delayBeforePop;
        animPopSpeedLerp = _popLerpSpeed;
    }
    private void Update()
    {
        if (timeBeforePop > 0)
        {
            timeBeforePop -= Time.unscaledDeltaTime;
            if (timeBeforePop < 0) timeBeforePop = 0;
        }
        if (timeBeforePop == 0)
        {
            currBaseScale = Vector3.Lerp(currBaseScale, Vector3.one, Time.unscaledDeltaTime * animPopSpeedLerp);
        }
        rootGraph.localScale = currBaseScale + Vector3.one * Mathf.Sin(Time.unscaledTime * idleSpeed + idleDelay) * idleAmplitude * currBaseScale.y;
    }
}
