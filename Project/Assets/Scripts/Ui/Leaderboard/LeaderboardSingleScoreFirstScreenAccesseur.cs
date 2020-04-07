using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardSingleScoreFirstScreenAccesseur : MonoBehaviour
{
    public Image background = null;
    public Outline backgroundOutline = null;
    public Text rankText = null;
    public Text nameText = null;
    public Text scoreText = null;
    public Transform rootGraph = null;
    [HideInInspector] public int rank = 0;
    public LeaderboardData data = null;
    public float speedLerp = 8;
    int nbPlayer = 0;
    float timeBeforeAnimPop = 0;

    BonusHandler manager = null;

    [SerializeField] DataSimpleAnim popAnim = null;
    float popPurcentage = 0;
    bool doPop = false;

    public void InitSoloScore(Transform rootGraphParent, BonusHandler _manager, LeaderboardData _data, int _rank, int _nbPlayer, bool isPlayer, float timeBeforeAnim)
    {
        rootGraph.SetParent(rootGraphParent);
        manager = _manager;
        data = _data;
        nameText.text = data.name;
        scoreText.text = data.score.ToString("N0");
        rank = _rank;
        nbPlayer = _nbPlayer;
        rankText.text = rank.ToString();
        if (isPlayer) background.color = UILeaderboard.Instance.dataLeaderboard.playerColorInLeaderboard;
        timeBeforeAnimPop = timeBeforeAnim;
        rootGraph.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (rootGraph != null && manager != null)
            rootGraph.transform.position = Vector3.Lerp(rootGraph.transform.position, transform.position, manager.dt * speedLerp);
        rankText.text = rank.ToString();
        scoreText.text = data.score.ToString("N0");

        if (rank == 1) backgroundOutline.effectColor = UILeaderboard.Instance.dataLeaderboard.firstScoreOutlineColor;
        else if (rank == nbPlayer)
        {
            backgroundOutline.effectColor = UILeaderboard.Instance.dataLeaderboard.lastScoreOutlineColor;
            rankText.text = "X";
        }
        else backgroundOutline.effectColor = UILeaderboard.Instance.dataLeaderboard.normalScoreOutlineColor;

        if (timeBeforeAnimPop > 0)
        {
            rootGraph.transform.position = transform.position;
            rootGraph.transform.localScale = Vector3.zero;
            timeBeforeAnimPop -= manager.dt;
            if (timeBeforeAnimPop < 0)
            {
                timeBeforeAnimPop = 0;
                doPop = true;
            }
        }

        if (doPop)
        {
            doPop = !popAnim.AddPurcentage(popPurcentage, manager.dt, out popPurcentage);
            rootGraph.transform.localScale = Vector3.one * popAnim.ValueAt(popPurcentage);
            if (!doPop)
                rootGraph.transform.localScale = Vector3.one;
        }

    }

}
