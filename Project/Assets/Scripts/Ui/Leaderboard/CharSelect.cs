using UnityEngine;
using UnityEngine.UI;

public class CharSelect : MonoBehaviour
{

    [SerializeField] LeaderboardButtonChar[] buttonChar = new LeaderboardButtonChar[0];
    public Text charText = null;

    DataLeaderboardUI dataLeaderboard = null;

    int currentIndex = 0;

    void Start()
    {
        foreach (var button in buttonChar) { button.manager = this; }
        dataLeaderboard = UILeaderboard.Instance.dataLeaderboard;
    }

    public void changeChar (int change)
    {
        currentIndex += change;
        if (currentIndex < 0) currentIndex += dataLeaderboard.alphabet.Length;
        if (currentIndex >= dataLeaderboard.alphabet.Length) currentIndex -= dataLeaderboard.alphabet.Length;
        charText.text = dataLeaderboard.alphabet[currentIndex].ToString();
    }

    public void PlayerClicked() { foreach (var button in buttonChar) { button.PlayerClicked(); } }

}
