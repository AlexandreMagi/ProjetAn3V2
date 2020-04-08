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
        //titleText.text = LeaderboardManager.lastTitle;
        titleText.text = Main.Instance.TitlesUnlocked[0].ToString();
    }

    public void changeTitle(int change)
    {
        currentIndex += change;
        if (currentIndex < 0) currentIndex += Main.Instance.TitlesUnlocked.Count;
        if (currentIndex >= Main.Instance.TitlesUnlocked.Count) currentIndex -= Main.Instance.TitlesUnlocked.Count;
        titleText.text = Main.Instance.TitlesUnlocked[currentIndex].ToString();
    }

    public void PlayerClicked() { foreach (var button in buttonChange) { button.PlayerClicked(); } }
}
