using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardAndCredits : MonoBehaviour
{

    [SerializeField] Transform rootScore = null;
    [SerializeField] GameObject prefabSingleScore = null;
    [SerializeField] int nbScoreToDisplay = 10;
    List<LeaderboardData> currLeaderboard = null;

    [SerializeField] DataLeaderboardUI dataLeaderboard = null;

    LeaderboardSingleScoreAccesseur[] allScore = null;

    [SerializeField] LeaderboardSingleCredits[] allCredits = null;
    
    public void InitTab()
    {
        currLeaderboard = LeaderboardManager.Instance.GetLeaderboard();
        allScore = new LeaderboardSingleScoreAccesseur[nbScoreToDisplay];
        for (int i = 0; i < nbScoreToDisplay; i++)
        {
            GameObject newObj = Instantiate(prefabSingleScore, rootScore);
            allScore[i] = newObj.GetComponent<LeaderboardSingleScoreAccesseur>();
            if (i < currLeaderboard.Count)
            {
                allScore[i].nameText.text = currLeaderboard[i].name;
                allScore[i].scoreText.text = currLeaderboard[i].score.ToString("N0");
                allScore[i].titleText.text = currLeaderboard[i].title;
                allScore[i].rankText.text = (i + 1).ToString();
            }
            else
            {
                allScore[i].nameText.text = "---";
                allScore[i].scoreText.text = 0.ToString("N0");
                allScore[i].titleText.text = "Nobody";
                allScore[i].rankText.text = (i + 1).ToString();
            }
        }
    }

    public void InitGraph()
    {
        for (int i = 0; i < allScore.Length; i++)
        {
            allScore[i].Init(i * dataLeaderboard.soloScoreIdleDelay, dataLeaderboard.soloScoreIdleAmplitude, dataLeaderboard.soloScoreIdleSpeed, i * dataLeaderboard.soloScoreDelayPopLocal + dataLeaderboard.soloScoreDelayPopGlobal, dataLeaderboard.soloScorePopSpeed);
        }
        for (int i = 0; i < allCredits.Length; i++)
        {
            allCredits[i].Init(i * dataLeaderboard.soloMetricIdleDelay, dataLeaderboard.soloMetricIdleAmplitude, dataLeaderboard.soloMetricIdleSpeed, i * dataLeaderboard.soloMetricDelayPopLocal + dataLeaderboard.soloMetricDelayPopGlobal, dataLeaderboard.soloMetricPopSpeed);
        }
    }

}
