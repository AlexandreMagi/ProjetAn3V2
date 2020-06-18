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
    public LeaderboardCanvasAccesseur cvsVars = null;

    // Variables de single scores
    [SerializeField] GameObject singleScorePrefab = null;
    LeaderboardSingleScoreAccesseur[] singleScoreAcces = new LeaderboardSingleScoreAccesseur[0];
    int nbSingleScoreDisplayed = 0;

    // Variables de single Metrics
    [SerializeField] GameObject singleMetricPrefab = null;
    List<LeaderboardSingleMetricAccesseur> singleMetricsAcces = new List<LeaderboardSingleMetricAccesseur>();
    List<DisplayedMetric> displayedMetrics = new List<DisplayedMetric>();

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
    public bool InLeaderboard { get { return inLeaderboard; } }
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

    float timeRemainingBeforeSceneChange = 0;

    [SerializeField] float timeBeforeAnimateCharSelect = 0.5f;
    float timeRemainingBeforeAnimateCharSelect = 0;

    [SerializeField] float timeBeforeDesactivateLastScreenAnimator = 0.5f;
    float timerBeforeDesactivateLastScreenAnimator = 0;

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
            cvsVars.charSelectors[i].SetupChar(LeaderboardManager.lastName[i]);
            //cvsVars.charSelectors[i].charText.text = LeaderboardManager.lastName[i].ToString();
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

        if (timeRemainingBeforeSceneChange > 0)
        {
            timeRemainingBeforeSceneChange -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeSceneChange < 0)
            {
                timeRemainingBeforeSceneChange = 0;

                switch (currentScreen)
                {
                    case leaderboardScreens.startLeaderboard:
                        InitChoiceNameAndTitle();
                        timeRemainingBeforeAnimateCharSelect = timeBeforeAnimateCharSelect;
                        break;
                    case leaderboardScreens.nameAndTitleChoice:
                        InitFinalLeaderboard();
                        break;
                    case leaderboardScreens.finalLeaderboard:
                        break;
                }
            }
        }

        if (timeRemainingBeforeAnimateCharSelect > 0)
        {
            timeRemainingBeforeAnimateCharSelect -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeAnimateCharSelect < 0)
            {
                cvsVars.idleAnimatorInCharSelect.ChangeWeightSettings(1, 2);
                cvsVars.charSelectAnimator.enabled = false;
            }
        }

        if (timerBeforeDesactivateLastScreenAnimator > 0)
        {
            timerBeforeDesactivateLastScreenAnimator -= Time.unscaledDeltaTime;
            if (timerBeforeDesactivateLastScreenAnimator < 0)
            {
                cvsVars.root_FinalLeaderboard.GetComponent<Animator>().enabled = false;
            }
        }

        if (inLeaderboard) CheckIfGoBackToMenu();

    }

    public void addScore (int score) { playerData.score += score; }

    public void PlayerClicked()
    {
        if (inLeaderboard && UILeaderboard.Instance.CurrentScreen == UILeaderboard.leaderboardScreens.nameAndTitleChoice) 
        { 
            foreach (var charSelect in charSelectors) { charSelect.PlayerClicked(); } 
            cvsVars.titleHandler.PlayerClicked(); 
        } 
        if (inLeaderboard)
        {
            ResetTimerAfk();
            cvsVars.nextButton.PlayerClicked();
            UILeaderboard.Instance.cvsVars.fadeHandler.playerClicked();
        }
    }

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
                    timeRemainingBeforeSceneChange = timeBetweenScreens;

                    foreach (var title in dataLeaderboard.titleAvailable)
                    {
                        Main.Instance.TitlesUnlocked.Add(title);
                    }

                    cvsVars.fadeHandler.ChangeOfScreen(4);
                    //Invoke("InitChoiceNameAndTitle", timeBetweenScreens);
                    //InitChoiceNameAndTitle();
                    ResetTimerAfk();
                    break;
                case leaderboardScreens.nameAndTitleChoice:
                    canChangeScene = false;
                    objectToMove = cvsVars.root_NameAndTitleChoice;
                    timeRemainingBeforeSceneChange = timeBetweenScreens;
                    cvsVars.fadeHandler.ChangeOfScreen(10);
                    //Invoke("InitFinalLeaderboard", timeBetweenScreens);
                    //InitFinalLeaderboard();
                    ResetTimerAfk();
                    break;
                case leaderboardScreens.finalLeaderboard:
                    canChangeScene = false;
                    Main.Instance.EndGame();
                    ResetTimerAfk();
                    break;
            }
        }
    }

    float timerCheckInput = 1;
    [SerializeField] float checkInputEvery = .5f;
    Vector3 saveLastCursorPos = Vector3.zero;
    float timerGoBack = 1;
    [SerializeField] float timeBeforeGoBackToMenu = 5;
    [SerializeField] float distanceCheckIfInput = .03f;

    public void ResetTimerAfk() { timerGoBack = timeBeforeGoBackToMenu; }

    void CheckIfGoBackToMenu()
    {
        Vector3 posCursor = Main.Instance.GetCursorPos();

        timerCheckInput -= Time.unscaledDeltaTime;
        if (timerCheckInput < 0)
        {
            timerCheckInput += checkInputEvery;
            float currDist = Mathf.Sqrt(
                Mathf.Pow(Mathf.Abs(saveLastCursorPos.x - posCursor.x) / Screen.width, 2) +
                Mathf.Pow(Mathf.Abs(saveLastCursorPos.y - posCursor.y) / Screen.height, 2));

            if (currDist > distanceCheckIfInput)
                timerGoBack = timeBeforeGoBackToMenu;

            else
            {
                if (timerGoBack < checkInputEvery)
                {
                    if (currentScreen != leaderboardScreens.finalLeaderboard) LeaderboardManager.Instance.SubmitScoreToLeaderboard(playerData.name, playerData.score, playerData.title);
                    Main.Instance.EndGame();
                }
                else
                    timerGoBack -= checkInputEvery;
            }
            saveLastCursorPos = posCursor;
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
            LeaderboardSingleScoreAccesseur scoreAcces = instantiatedSingleScore.GetComponent<LeaderboardSingleScoreAccesseur>();
            scoreAcces.Init(i * dataLeaderboard.soloScoreIdleDelay, dataLeaderboard.soloScoreIdleAmplitude, dataLeaderboard.soloScoreIdleSpeed, i * dataLeaderboard.soloScoreDelayPopLocal + dataLeaderboard.soloScoreDelayPopGlobal, dataLeaderboard.soloScorePopSpeed);
            singleScoreAcces[i] = scoreAcces;
        }

        // --- Instance des single metric
        for (int i = 0; i < displayedMetrics.Count; i++)
        {
            GameObject instantiatedSingleScore = Instantiate(singleMetricPrefab, cvsVars.root_SingleMetrics);
            LeaderboardSingleMetricAccesseur metricAcces = instantiatedSingleScore.GetComponent<LeaderboardSingleMetricAccesseur>();
            metricAcces.SetupTexts(displayedMetrics[i].type, displayedMetrics[i].currValue, displayedMetrics[i].maxValue);
            metricAcces.Init(i * dataLeaderboard.soloMetricIdleDelay, dataLeaderboard.soloMetricIdleAmplitude, dataLeaderboard.soloMetricIdleSpeed, i * dataLeaderboard.soloMetricDelayPopLocal + dataLeaderboard.soloMetricDelayPopGlobal, dataLeaderboard.soloMetricPopSpeed);
            //metricAcces.SetTextColor(displayedMetrics[i].success ? dataLeaderboard.metricSucceedColor : dataLeaderboard.metricFailedColor);
            singleMetricsAcces.Add(metricAcces);
        }

        int indexPlayer = 0;
        leaderboardDatas = SortPlayerInList(LeaderboardManager.Instance.GetLeaderboard(), playerData, out indexPlayer);

        LeaderboardManager.lastName = playerData.name;
        LeaderboardManager.lastTitle = playerData.title;

        DisplayScores(leaderboardDatas, indexPlayer);

        timerBeforeDesactivateLastScreenAnimator = timeBeforeDesactivateLastScreenAnimator;

        canChangeScene = true;

        LeaderboardManager.Instance.SubmitScoreToLeaderboard(playerData.name, playerData.score, playerData.title);
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
            if (i == 0) singleScoreAcces[i].backgroundOutline.effectColor = dataLeaderboard.firstScoreOutlineColor;
            if (i == 1) singleScoreAcces[i].backgroundOutline.effectColor = dataLeaderboard.secondScoreOutlineColor;
            if (i == 2) singleScoreAcces[i].backgroundOutline.effectColor = dataLeaderboard.thirdScoreOutlineColor;

            if (i < 3)
            {
                singleScoreAcces[i].background.transform.localScale = new Vector3(1, dataLeaderboard.firstsScaleInY, 1);
                //singleScoreAcces[i].background.rectTransform.offsetMax = singleScoreAcces[i].background.rectTransform.offsetMin;
            }

            if (i == dataSend.Length-1) singleScoreAcces[i].rankText.text = "X";
            else singleScoreAcces[i].rankText.text = (i + 1).ToString();
        }
    }

    public void AddMetricToDisplay(string type, string currValue, string maxValue, bool success)
    {
        displayedMetrics.Add(new DisplayedMetric(type, currValue, maxValue, success));
    }

}

public class DisplayedMetric
{
    public string type;
    public string currValue;
    public string maxValue;
    public bool success;

    public DisplayedMetric(string _type, string _currValue, string _maxValue, bool _success)
    {
        type = _type;
        currValue = _currValue;
        maxValue = _maxValue;
        success = _success; 
    }

}
