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
}
