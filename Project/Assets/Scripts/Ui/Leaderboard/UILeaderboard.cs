using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboard : MonoBehaviour
{

    // Accesseur
    public static UILeaderboard Instance { get; private set; }
    void Awake() 
    { 
        Instance = this;
    }

    // Canvas qui va se placer devant tout
    [SerializeField] GameObject cvs = null;
    LeaderboardCanvasAccesseur cvsVars = null;

    // Variables de single scores
    [SerializeField] GameObject singleScorePrefab = null;
    LeaderboardSingleScoreAccesseur[] singleScoreAcces = new LeaderboardSingleScoreAccesseur[0];
    int nbSingleScoreDisplayed = 0;

    // Timer
    [SerializeField] float timerBeforeActivateProgressBars = 2;
    float timeRemainingBeforeProgress = 0;

    // Acces bonus handler
    public BonusHandler bonusHandler = null;

    // Settings
    public DataLeaderboardUI dataLeaderboard = null;

    // Stock des gérant de character
    CharSelect[] charSelectors = new CharSelect[0];

    // Var
    LeaderboardData[] leaderboardDatas = new LeaderboardData[0];
    bool inLeaderboard = false; // Indique que l'on est en état leaderboard
    LeaderboardData playerData = null;
    public enum leaderboardScreens { startLeaderboard, nameAndTitleChoice, finalLeaderboard }
    leaderboardScreens currentScreen = leaderboardScreens.startLeaderboard;
    public leaderboardScreens CurrentScreen { get { return currentScreen; } }

    public float deltaTimeMultiplier = 1;

    public int Score { get { return playerData.score; } }

    bool canChangeScene = false;

    public void InitLeaderboard(int score) { StartCoroutine(InitLeaderboardCoroutine(score)); }

    [SerializeField] float timeBeforePopUI = 0.7f;
    [SerializeField] float timeBeforePopFirstScreenLeaderboard = 1.5f;

    Transform objectToMove = null;
    [SerializeField] float speedScreenGoLeftWhenSwitched = 100f;
    [SerializeField] float timeBetweenScreens = 0.5f;

    [SerializeField] float scoreTextLerpSpeedTransition = 8;
    float transitionFontSize = 0;
    float currScoreDisplayed = 0;

    IEnumerator InitLeaderboardCoroutine (int score)
    {
        // --- Desactivation des canvas existants
        Canvas[] existingsCanvas = FindObjectsOfType<Canvas>();
        foreach (var canvas in existingsCanvas) { canvas.gameObject.SetActive(false); }

        // --- Spawn du nouveau canvas
        cvs = Instantiate(cvs);
        // Pop
        cvsVars = cvs.GetComponent<LeaderboardCanvasAccesseur>();
        charSelectors = cvsVars.charSelectors;
        cvsVars.crossHairHandler.SetAsInstance();

        // Indique que l'on est en état leaderboard
        inLeaderboard = true;

        // Setup score
        playerData = new LeaderboardData();
        playerData.score = score;

        yield return new WaitForSecondsRealtime(timeBeforePopUI);

        // Pop des trucs
        cvsVars.root_FirstLeaderboard.gameObject.SetActive(true);
        cvsVars.root_NameAndTitleChoice.gameObject.SetActive(false);
        cvsVars.root_FinalLeaderboard.gameObject.SetActive(false);
        cvsVars.bonusHandler.InitPopAnim();

        // Init au dernier nom marqué
        for (int i = 0; i < cvsVars.charSelectors.Length; i++)
        {
            cvsVars.charSelectors[i].charText.text = LeaderboardManager.lastName[i].ToString();
        }
        timeRemainingBeforeProgress = timerBeforeActivateProgressBars;

        yield return new WaitForSecondsRealtime(timeBeforePopFirstScreenLeaderboard);

        cvsVars.bonusHandler.InitLeaderboard();

        canChangeScene = true;
        yield break;
    }

    private void Update()
    {
        if (timeRemainingBeforeProgress > 0)
        {
            timeRemainingBeforeProgress -= cvsVars.bonusHandler.dt;
            if (timeRemainingBeforeProgress < 0)
            {
                // Permet de commencer le premier screen
                cvsVars.bonusHandler.allowToNext = true;
            }
        }

        if (objectToMove != null)
        {
            objectToMove.Translate(Vector3.left * speedScreenGoLeftWhenSwitched * Time.unscaledDeltaTime * deltaTimeMultiplier);
            if (canChangeScene) objectToMove = null;
        }

        if (playerData != null && cvsVars.scoreAtCharSelect != null && (currentScreen == leaderboardScreens.nameAndTitleChoice || !canChangeScene) && currScoreDisplayed != 0) 
        {
            currScoreDisplayed = Mathf.Lerp(currScoreDisplayed, playerData.score, Time.unscaledDeltaTime * deltaTimeMultiplier * scoreTextLerpSpeedTransition);
            cvsVars.scoreAtCharSelect.text = Mathf.RoundToInt(currScoreDisplayed).ToString("N0");
        }

        if (currentScreen == leaderboardScreens.nameAndTitleChoice)
        {
            cvsVars.scoreAtCharSelect.transform.position = Vector3.Lerp(cvsVars.scoreAtCharSelect.transform.position, cvsVars.scoreCharGoTo.transform.position, Time.unscaledDeltaTime * deltaTimeMultiplier * scoreTextLerpSpeedTransition);
            cvsVars.scoreAtCharSelect.transform.localScale = Vector3.Lerp(cvsVars.scoreAtCharSelect.transform.localScale, cvsVars.scoreCharGoTo.transform.localScale, Time.unscaledDeltaTime * deltaTimeMultiplier * scoreTextLerpSpeedTransition);
            transitionFontSize = Mathf.Lerp(transitionFontSize, cvsVars.scoreCharGoTo.fontSize, Time.unscaledDeltaTime * deltaTimeMultiplier * scoreTextLerpSpeedTransition);
            cvsVars.scoreAtCharSelect.fontSize = Mathf.RoundToInt(transitionFontSize);

            if (Vector3.Distance(cvsVars.scoreAtCharSelect.transform.position, cvsVars.scoreCharGoTo.transform.position) < 0.01f)
                cvsVars.scoreAtCharSelect.transform.SetParent(cvsVars.parentForScoreAfterTransition);
        }
    }

    public void addScore (int score) { playerData.score += score; }

    public void PlayerClicked() { if (inLeaderboard && UILeaderboard.Instance.CurrentScreen == UILeaderboard.leaderboardScreens.nameAndTitleChoice) { foreach (var charSelect in charSelectors) { charSelect.PlayerClicked(); } cvsVars.titleHandler.PlayerClicked(); } }

    public void NextScreen()
    {
        if (canChangeScene)
        {
            switch (currentScreen)
            {
                case leaderboardScreens.startLeaderboard:
                    canChangeScene = false;
                    objectToMove = cvsVars.root_FirstLeaderboard;
                    cvsVars.bonusHandler.goAway();
                    transitionFontSize = cvsVars.scoreAtCharSelect.fontSize;
                    Invoke("InitChoiceNameAndTitle", timeBetweenScreens);
                    //InitChoiceNameAndTitle();
                    break;
                case leaderboardScreens.nameAndTitleChoice:
                    canChangeScene = false;
                    objectToMove = cvsVars.root_NameAndTitleChoice;
                    Invoke("InitFinalLeaderboard", timeBetweenScreens);
                    //InitFinalLeaderboard();
                    break;
                case leaderboardScreens.finalLeaderboard:
                    canChangeScene = false;
                    Main.Instance.EndGame(playerData);
                    break;
            }
        }
    }


    void InitChoiceNameAndTitle()
    {
        // --- Pop
        cvsVars.root_FirstLeaderboard.gameObject.SetActive(false);
        cvsVars.root_NameAndTitleChoice.gameObject.SetActive(true);
        cvsVars.root_FinalLeaderboard.gameObject.SetActive(false);
        currentScreen = leaderboardScreens.nameAndTitleChoice;
        currScoreDisplayed = cvsVars.bonusHandler.currScoreDisplayed;


        cvsVars.scoreAtCharSelect.text = /*"SCORE : " + */playerData.score.ToString("N0");
        canChangeScene = true;
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

        canChangeScene = true;
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
            if (i == dataSend.Length-1) singleScoreAcces[i].backgroundOutline.effectColor = dataLeaderboard.lastScoreOutlineColor;

            if (i == dataSend.Length-1) singleScoreAcces[i].rankText.text = "X";
            else singleScoreAcces[i].rankText.text = (i + 1).ToString();
        }
    }

}
