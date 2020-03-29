using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboard : MonoBehaviour
{

    // Accesseur
    public static UILeaderboard Instance { get; private set; }
    void Awake() { Instance = this; }

    // Canvas qui va se placer devant tout
    [SerializeField] GameObject cvs = null;
    LeaderboardCanvasAccesseur cvsVars = null;

    // Variables de single scores
    [SerializeField] GameObject singleScorePrefab = null;
    LeaderboardSingleScoreAccesseur[] singleScoreAcces = new LeaderboardSingleScoreAccesseur[0];
    int nbSingleScoreDisplayed = 0;

    // Settings
    public DataLeaderboardUI dataLeaderboard = null;

    // Stock des gérant de character
    CharSelect[] charSelectors = new CharSelect[0];

    // Var
    LeaderboardData[] leaderboardDatas = new LeaderboardData[0];
    bool inLeaderboard = false; // Indique que l'on est en état leaderboard
    LeaderboardData playerData = null;
    enum leaderboardScreens { startLeaderboard, nameAndTitleChoice, finalLeaderboard }
    leaderboardScreens currentScreen = leaderboardScreens.startLeaderboard;


    public void InitLeaderboard(int score)
    {
        // --- Desactivation des canvas existants
        Canvas[] existingsCanvas = FindObjectsOfType<Canvas>();
        foreach (var canvas in existingsCanvas) { canvas.gameObject.SetActive(false); }

        // --- Spawn du nouveau canvas
        cvs = Instantiate(cvs);
        // Pop
        cvsVars = cvs.GetComponent<LeaderboardCanvasAccesseur>();
        cvsVars.root_FirstLeaderboard.gameObject.SetActive(true);
        cvsVars.root_NameAndTitleChoice.gameObject.SetActive(false);
        cvsVars.root_FinalLeaderboard.gameObject.SetActive(false);
        charSelectors = cvsVars.charSelectors;
        cvsVars.crossHairHandler.SetAsInstance();

        // Indique que l'on est en état leaderboard
        inLeaderboard = true;

        // Setup score
        playerData = new LeaderboardData();
        playerData.score = score;

        // Init au dernier nom marqué
        for (int i = 0; i < cvsVars.charSelectors.Length; i++)
        {
            cvsVars.charSelectors[i].charText.text = LeaderboardManager.lastName[i].ToString();
        }
    }

    public void PlayerClicked() { if (inLeaderboard) { foreach (var charSelect in charSelectors) { charSelect.PlayerClicked(); } cvsVars.titleHandler.PlayerClicked(); } }

    public void NextScreen()
    {
        switch (currentScreen)
        {
            case leaderboardScreens.startLeaderboard:
                InitChoiceNameAndTitle();
                break;
            case leaderboardScreens.nameAndTitleChoice:
                InitFinalLeaderboard();
                break;
            case leaderboardScreens.finalLeaderboard:
                Main.Instance.EndGame (playerData);
                break;
        }
    }

    void InitChoiceNameAndTitle()
    {
        // --- Pop
        cvsVars.root_FirstLeaderboard.gameObject.SetActive(false);
        cvsVars.root_NameAndTitleChoice.gameObject.SetActive(true);
        cvsVars.root_FinalLeaderboard.gameObject.SetActive(false);
        currentScreen = leaderboardScreens.nameAndTitleChoice;
        cvsVars.scoreAtCharSelect.text = "SCORE : " + playerData.score.ToString("N0");
    }

    void InitFinalLeaderboard()
    {
        // --- Récupération des valeurs
        string name = "";
        for (int i = 0; i < cvsVars.charSelectors.Length; i++)
        {
            name += cvsVars.charSelectors[i].charText.text;
        }
        string title = cvsVars.titleChoiceText.text;

        // --- Pop
        cvsVars.root_FirstLeaderboard.gameObject.SetActive(false);
        cvsVars.root_NameAndTitleChoice.gameObject.SetActive(false);
        cvsVars.root_FinalLeaderboard.gameObject.SetActive(true);
        currentScreen = leaderboardScreens.finalLeaderboard;

        // --- Setup nom et titre
        playerData.name = name;
        playerData.title = title;

        // --- Instance des single scores
        nbSingleScoreDisplayed = LeaderboardManager.Instance.maxKeptScores + 1;
        singleScoreAcces = new LeaderboardSingleScoreAccesseur[nbSingleScoreDisplayed];
        for (int i = 0; i < nbSingleScoreDisplayed; i++)
        {
            GameObject instantiatedSingleScore = Instantiate(singleScorePrefab, cvsVars.root_SingleScores);
            singleScoreAcces[i] = instantiatedSingleScore.GetComponent<LeaderboardSingleScoreAccesseur>();
        }

        int indexPlayer = 0;
        leaderboardDatas = SortPlayerInList(LeaderboardManager.Instance.GetLeaderboard(), playerData, out indexPlayer);

        LeaderboardManager.lastName = playerData.name;
        LeaderboardManager.lastTitle = playerData.title;

        DisplayScores(leaderboardDatas, indexPlayer);

    }

    // Trie le tableau de rang dans l'ordre croissant (le premier rang sera donc au début)
    public LeaderboardData[] SortPlayerInList (List<LeaderboardData> dataSend, LeaderboardData playerData, out int indexPlayer)
    {
        LeaderboardData[] sortedArray = new LeaderboardData[nbSingleScoreDisplayed];
        sortedArray[sortedArray.Length - 1] = playerData;
        indexPlayer = sortedArray.Length - 1;
        for (int i = 0; i < sortedArray.Length - 1; i++)
        {
            LeaderboardData dataAtIndex = null;
            if (i >= dataSend.Count || dataSend[i] == null) dataAtIndex = new LeaderboardData("---", 0, "nobody");
            else dataAtIndex = dataSend[i];
            sortedArray[i] = dataAtIndex;
        }

        for (int i = sortedArray.Length - 1; i > -1; i--)
        {
            if (i > 0)
            {
                if (sortedArray[i].score > sortedArray[i - 1].score)
                {
                    LeaderboardData savedData = sortedArray[i - 1];
                    sortedArray[i - 1] = sortedArray[i];
                    sortedArray[i] = savedData;

                    if (i == indexPlayer) indexPlayer--;
                }
            }
        }
        Debug.Log("Player rank = " + (indexPlayer+1));
        return sortedArray;
    }

    void DisplayScores(LeaderboardData[] dataSend, int playerIndex)
    {
        for (int i = 0; i < dataSend.Length; i++)
        {
            singleScoreAcces[i].nameText.text = dataSend[i].name;
            singleScoreAcces[i].titleText.text = " - " + dataSend[i].title;
            singleScoreAcces[i].scoreText.text = dataSend[i].score.ToString();
            singleScoreAcces[i].scoreText.text = dataSend[i].score.ToString("N0");

            if (i == playerIndex) singleScoreAcces[i].background.color = dataLeaderboard.playerColorInLeaderboard;

            if (i == dataSend.Length-1) singleScoreAcces[i].rankText.text = "X";
            else singleScoreAcces[i].rankText.text = (i + 1).ToString();

        }
    }

}
