using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharSelect : MonoBehaviour
{

    [SerializeField] LeaderboardButtonChar[] buttonChar = new LeaderboardButtonChar[0];
    public Text charText = null;

    DataLeaderboardUI dataLeaderboard = null;

    int currentIndex = 0;

    void Awake()
    {
        SetupData();
    }

    void Start()
    {
        foreach (var button in buttonChar) { button.manager = this; }
    }

    public void changeChar (int change)
    {
        currentIndex += change;
        if (currentIndex < 0) currentIndex += dataLeaderboard.alphabet.Length;
        if (currentIndex >= dataLeaderboard.alphabet.Length) currentIndex -= dataLeaderboard.alphabet.Length;
        charText.text = dataLeaderboard.alphabet[currentIndex].ToString();
    }

    void SetupData()
    {
        dataLeaderboard = UILeaderboard.Instance.dataLeaderboard;
    }

    public void SetupChar (char _char)
    {
        if (dataLeaderboard == null) SetupData();

        int index = 0;
        for (int i = 0; i < dataLeaderboard.alphabet.Length; i++)
        {
            if (dataLeaderboard.alphabet[i] == _char)
            {
                index = i;
                break;
            }
        }
        currentIndex = index;
        charText.text = dataLeaderboard.alphabet[currentIndex].ToString();
    }

    public void PlayerClicked() { foreach (var button in buttonChar) { button.PlayerClicked(); } }

}
