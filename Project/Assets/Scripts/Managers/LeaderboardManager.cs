using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    static LeaderboardManager Instance;

    public void Start()
    {
        Instance = this;
    }
}
