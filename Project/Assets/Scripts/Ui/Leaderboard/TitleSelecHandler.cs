using UnityEngine;
using UnityEngine.UI;

public class TitleSelecHandler : MonoBehaviour
{

    [SerializeField] TitleSelecButton[] buttonChange = new TitleSelecButton[0];
    public Text titleText = null;

    DataLeaderboardUI dataLeaderboard = null;

    int currentIndex = 0;

    void Start()
    {
        foreach (var button in buttonChange) { button.manager = this; }
        dataLeaderboard = UILeaderboard.Instance.dataLeaderboard;
        //titleText.text = dataLeaderboard.titleAvailable[currentIndex].ToString();
        titleText.text = LeaderboardManager.lastTitle;
    }

    public void changeTitle(int change)
    {
        currentIndex += change;
        if (currentIndex < 0) currentIndex += dataLeaderboard.titleAvailable.Length;
        if (currentIndex >= dataLeaderboard.titleAvailable.Length) currentIndex -= dataLeaderboard.titleAvailable.Length;
        titleText.text = dataLeaderboard.titleAvailable[currentIndex].ToString();
    }

    public void PlayerClicked() { foreach (var button in buttonChange) { button.PlayerClicked(); } }
}
