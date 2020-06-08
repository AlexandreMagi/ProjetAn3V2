using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TriggerSetupLeaderboardName : MonoBehaviour
{
    [SerializeField] TextMeshPro[] allNames = null;
    List<LeaderboardData> currLeaderboard = null;

    [SerializeField] bool doOnlyOnce = true;
    bool canDo = true;

    private void OnTriggerEnter(Collider other)
    {
        if (canDo)
        {
            if (doOnlyOnce) canDo = false;
            currLeaderboard = LeaderboardManager.Instance.GetLeaderboard();
            for (int i = 0; i < allNames.Length; i++)
            {
                if (allNames[i] != null && currLeaderboard[i] != null)
                {
                    allNames[i].text = currLeaderboard[i].name;
                }
            }
        }
    }

}
