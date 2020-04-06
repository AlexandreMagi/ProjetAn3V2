using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataLeaderboardUI", menuName = "ScriptableObjects/DataLeaderboardUI")]
public class DataLeaderboardUI : ScriptableObject
{
    public char[] alphabet;
    public string[] titleAvailable;

    public Color baseColorButtons = Color.white;
    public Color highlightedColorButtons = Color.white;

    public Color playerColorInLeaderboard = Color.white;

    public Color lastScoreOutlineColor = Color.white;
    public Color normalScoreOutlineColor = Color.white;
    public Color firstScoreOutlineColor = Color.white;

    public float scaleWhenMouseOvered = 1.3f;
    public float scaleNormal = 1;
    public float scaleLerp = 8;

    [Header("Metrics")]
    public Color metricSucceedColor = Color.white;
    public Color metricFailedColor = Color.white;

    [Header("Anim Last Screen score")]
    public float soloScoreIdleDelay = -0.3f;
    public float soloScoreIdleAmplitude = 0.01f;
    public float soloScoreIdleSpeed = 1f;
    public float soloScoreDelayPopLocal = 0.05f;
    public float soloScoreDelayPopGlobal = 0.5f;
    public float soloScorePopSpeed = 8f;


    [Header("Anim Last Screen metric")]
    public float soloMetricIdleDelay = -0.5f;
    public float soloMetricIdleAmplitude = 0.03f;
    public float soloMetricIdleSpeed = 1f;
    public float soloMetricDelayPopLocal = 0.5f;
    public float soloMetricDelayPopGlobal = 1.5f;
    public float soloMetricPopSpeed = 8f;
}
