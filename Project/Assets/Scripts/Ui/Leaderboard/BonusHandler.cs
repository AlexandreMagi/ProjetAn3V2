using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusHandler : MonoBehaviour
{

    const int maxBonusDisplayed = 5;
    [SerializeField] Transform layoutGroup = null;
    public Transform rootGraph = null;
    [SerializeField] GameObject prefabBonusHandler = null;

    List<BonusGraphInstanceHandler> bonusInstances = new List<BonusGraphInstanceHandler>();


    [Header("Progress bars")]
    [SerializeField] float timePop = 0.5f;
    [SerializeField] float timeFill = 1f;
    [SerializeField] float timeAnim = 1f;
    [SerializeField] float timeDepop = 0.5f;
    [SerializeField] float timeStayDepop = 0.5f;

    [SerializeField] CanvasGroup cvsGroupProgress = null;
    [SerializeField] Image maskProgress = null;
    [SerializeField] Outline outlineBar = null;
    [SerializeField] Text scoreText = null;
    [SerializeField] Text typeText = null;
    [SerializeField] Text numberText = null;
    [SerializeField] Text numberMaxText = null;

    enum stateProgress { pop,fill,anim,depop,staydepop,paused };
    stateProgress progressState = stateProgress.paused;

    float purcentageInState = 1;
    float cvsGroupAlpha = 1;
    float outlineAlphaPow = 3;
    [HideInInspector] public float dt = 0;
    float customTime = 0;
    bool firstLoop = true;

    float currScoreDisplayed = 0;
    [SerializeField] float scoreLerpSpeed = 8;

    [HideInInspector] public bool allowToNext = false;

    [SerializeField] float localSpeedMultiplier = 1;

    [Header("ProgressAnim")]
    [SerializeField] Image colorToChangeProgress = null;
    [SerializeField] Color colorProgressNormal = Color.white;

    [SerializeField] Color colorProgressSucces = Color.white;
    [SerializeField] Transform transformToScaleInSuccesAnim = null;
    [SerializeField] DataSimpleAnim progressSuccesAnim = null;
    bool playAnimProgressSucces = false;
    [SerializeField] Color colorProgressFailure = Color.white;
    [SerializeField] Transform transformToScaleInFailureAnim = null;
    [SerializeField] DataSimpleAnim progressFailureAnim = null;
    bool playAnimFailureSucces = false;
    float progressAnimPurcentage = 1;

    [Header("Score Anim")]
    [SerializeField] float scoreAnimMaxSize = .3f;
    [SerializeField] float scoreAnimRangeConsideredSame = 1;
    [SerializeField] float scoreIdleSpeed = 7;
    [SerializeField] float scoreIdleAmplitude = 7;


    [SerializeField] float refForMaxSizeAndShake = 500000;
    [SerializeField] float maxAddedSizeTitle = 0.2f;
    [SerializeField] float scoreIdleShakeSpeed = 7;
    [SerializeField] float scoreIdleShakeAmplitude = 7;

    float currAddedScaleToTitle = 0;
    Vector3 stockedScoreInitialLocalPos = Vector3.zero;


    [Header("Dynamic Leaderboard")]
    [SerializeField] Transform rootParentLeaderboard = null;
    [SerializeField] Transform rootParentLeaderboardGraphs = null;
    [SerializeField] GameObject prefabSoloScoreFirstScreen = null;
    List<LeaderboardSingleScoreFirstScreenAccesseur> allScoreAcesseurs = new List<LeaderboardSingleScoreFirstScreenAccesseur>();
    int nbScoreKept = 0;

    List<LeaderboardData> leaderboardDatas = new List<LeaderboardData>();
    List<LeaderboardData> newLeaderboardDatas = new List<LeaderboardData>();
    LeaderboardData playerData = null;
    Transform playerSoloScore = null;
    LeaderboardSingleScoreFirstScreenAccesseur playerAccesseur = null;

    [SerializeField] float timeDelayBeforePop = 0.2f;
    [SerializeField] float timeDelayBetweenPops = 0.2f;
    [SerializeField] float timeBeforeUpdateLeaderboard = 2f;


    [Header("Explosion If Succes")]
    [SerializeField] float idlePurcentage = 0;
    [SerializeField] float minValueIdle = 0.2f;
    [SerializeField] float idlePurcentageTimeCancel = 2;
    [SerializeField] Image explosionImage = null;
    [SerializeField] float timeExplosion = 1.75f;
    [SerializeField] float scaleMaxExplosion = 15f;
    [SerializeField] float scaleMinExplosion = 1f;
    float currentPurcentageExplosion = 1;
    [SerializeField] Color explosionColorStart = Color.white;
    [SerializeField] Color explosionColorEnd = Color.white;

    [Header("ExplosionDecalSprites")]
    [SerializeField] Transform explosionSource = null;
    [SerializeField] Transform[] affectedByExplosion = null;
    [SerializeField] float explosionForceMultiplierByScore = 5;
    float explosionCurrentMultiplier = 1;
    Vector3[] basePos = null;
    [SerializeField] DataSimpleAnim explosionAnim = null;
    bool doAnimExplosion = false;
    float animExplosionPurcentage = 1;


    Vector3[] ildedPos = null;
    [SerializeField] Transform idleRefMiddle = null;
    [SerializeField] float idleSpeed = 1;
    [SerializeField] float idleMagnitude = 20;

    bool goingAway = false;


    [Header("Particles Projection")]
    [SerializeField] Transform refProjection = null;
    [SerializeField] UIParticuleSystemDispersion[] allParticlsToActivate = null;
    [SerializeField] UIParticuleSystemDispersion looseParticle = null;

    private void Start()
    {
        cvsGroupAlpha = 0;
        maskProgress.fillAmount = 0;

        currScoreDisplayed = UILeaderboard.Instance.Score;
        scoreText.text = Mathf.RoundToInt(currScoreDisplayed).ToString("N0");
        stockedScoreInitialLocalPos = scoreText.transform.localPosition;
        //InitLeaderboard();
        explosionImage.enabled = false;
    }

    public void InitLeaderboard()
    {
        leaderboardDatas = LeaderboardManager.Instance.GetLeaderboard();
        nbScoreKept = LeaderboardManager.Instance.maxKeptScores + 1;
        for (int i = 0; i < nbScoreKept; i++)
        {
            bool isPlayer = false;
            GameObject currGo = Instantiate(prefabSoloScoreFirstScreen, rootParentLeaderboard);
            LeaderboardSingleScoreFirstScreenAccesseur currAccess = currGo.GetComponent<LeaderboardSingleScoreFirstScreenAccesseur>();

            LeaderboardData currIndexData = null;
            if (i == nbScoreKept - 1)
            {
                playerData = new LeaderboardData("YOU", Mathf.RoundToInt(currScoreDisplayed), "No Title");
                currIndexData = playerData = new LeaderboardData("YOU", Mathf.RoundToInt(currScoreDisplayed), "No Title");
                playerSoloScore = currGo.transform;
                playerAccesseur = currAccess;
                isPlayer = true;
            }
            else if (i < leaderboardDatas.Count && leaderboardDatas[i] != null)
                currIndexData = leaderboardDatas[i];
            else
                currIndexData = new LeaderboardData("---", 0, "No Title");
            newLeaderboardDatas.Add(currIndexData);
            currAccess.InitSoloScore(rootParentLeaderboardGraphs, this, currIndexData, i+1, nbScoreKept, isPlayer, timeDelayBeforePop + timeDelayBetweenPops*i);
            allScoreAcesseurs.Add(currAccess);
        }

        // Anim explosion
        basePos = new Vector3[affectedByExplosion.Length];
        ildedPos = new Vector3[affectedByExplosion.Length];
        for (int i = 0; i < affectedByExplosion.Length; i++)
        {
            basePos[i] = affectedByExplosion[i].position;
        }
    }

    void UpdatePlayerInDynamicLeaderboard()
    {
        int currPlayerIndex = nbScoreKept - 1;
        for (int i = newLeaderboardDatas.Count-2; i > -1; i--)
        {
            if (newLeaderboardDatas[i].score < currScoreDisplayed)
            {
                allScoreAcesseurs[i].rank = i + 2;
                currPlayerIndex--;
            }
            else
            {
                allScoreAcesseurs[i].rank = i + 1;
            }
        }
        if (currPlayerIndex < 0) currPlayerIndex = 0;
        playerSoloScore.SetSiblingIndex(currPlayerIndex);
        playerData.score = Mathf.RoundToInt(currScoreDisplayed);
        playerAccesseur.rank = currPlayerIndex + 1;
    }

    void Update()
    {

        dt = Time.unscaledDeltaTime * UILeaderboard.Instance.deltaTimeMultiplier * localSpeedMultiplier;
        customTime += dt;
        currScoreDisplayed = Mathf.Lerp(currScoreDisplayed, UILeaderboard.Instance.Score, dt * scoreLerpSpeed);

        // --- Anim Score
        if (Mathf.Abs(currScoreDisplayed - UILeaderboard.Instance.Score) > scoreAnimRangeConsideredSame)
            currAddedScaleToTitle = Mathf.Lerp(currAddedScaleToTitle, scoreAnimMaxSize, dt);
        else
            currAddedScaleToTitle = Mathf.Lerp(currAddedScaleToTitle, 0, dt);


        // --- IDLE

        idlePurcentage = Mathf.MoveTowards(idlePurcentage, minValueIdle, (1- minValueIdle)* dt / idlePurcentageTimeCancel);

        Vector3 scaleChange = Vector3.one * Mathf.Sin(customTime * scoreIdleSpeed) * scoreIdleAmplitude + Vector3.one * currAddedScaleToTitle + Vector3.one * (maxAddedSizeTitle * currScoreDisplayed / refForMaxSizeAndShake);
        scoreText.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one + scaleChange, idlePurcentage);

        Vector3 posChange = (new Vector3(Mathf.PerlinNoise(10, customTime * scoreIdleShakeSpeed) * 2 - 1, Mathf.PerlinNoise(1, customTime * scoreIdleShakeSpeed) * 2 - 1) * (scoreIdleShakeAmplitude * currScoreDisplayed / refForMaxSizeAndShake));
        scoreText.transform.localPosition = Vector3.Lerp(stockedScoreInitialLocalPos, stockedScoreInitialLocalPos + posChange, idlePurcentage);

        if (!goingAway && basePos != null)
        {
            for (int i = 0; i < basePos.Length; i++)
            {
                Vector3 dir = Vector3.Normalize(basePos[i] - idleRefMiddle.position);
                ildedPos[i] = basePos[i] + dir * Mathf.Sin(customTime * idleSpeed) * idleMagnitude;
                affectedByExplosion[i].position = ildedPos[i];
            }
        }
        HandleExplosionAnim();

        scoreText.text = Mathf.RoundToInt(currScoreDisplayed).ToString("N0");

        DoProgressBarAnim();

        if (timeBeforeUpdateLeaderboard >= 0)
        {
            timeBeforeUpdateLeaderboard -= dt;
        }
        if (timeBeforeUpdateLeaderboard < 0) UpdatePlayerInDynamicLeaderboard();

        #region Progress Bar
        switch (progressState)
        {
            // --- Apparition de la progress bar
            case stateProgress.pop:
                if (upPurcentage(timePop))
                {
                    maskProgress.fillAmount = 0;
                    cvsGroupAlpha = 1;
                    progressState = stateProgress.fill;
                }
                else
                {
                    cvsGroupAlpha = purcentageInState;
                }
                break;

            // --- Remplissement de la progress bar
            case stateProgress.fill:
                if (upPurcentage(timeFill))
                {
                    numberText.text = Main.Instance.AllEndGameBonus[0].currValue.ToString() + Main.Instance.AllEndGameBonus[0].addedCharacter;
                    maskProgress.fillAmount = Main.Instance.AllEndGameBonus[0].currValue / Main.Instance.AllEndGameBonus[0].maxValue;

                    if (Main.Instance.AllEndGameBonus[0].currValue >= Main.Instance.AllEndGameBonus[0].maxValue)
                    {
                        playAnimProgressSucces = true;
                        currentPurcentageExplosion = 0;
                        foreach (var particleSystem in allParticlsToActivate)
                        {
                            particleSystem.Play(refProjection.position);
                        }
                        idlePurcentage = 1;
                        animExplosionPurcentage = 0;
                        doAnimExplosion = true;
                    }
                    else
                    {
                        playAnimFailureSucces = true;
                        looseParticle.Play(refProjection.position);
                    }
                    progressAnimPurcentage = 0;
                    progressState = stateProgress.anim;
                }
                else
                {
                    numberText.text = Mathf.Ceil(Main.Instance.AllEndGameBonus[0].currValue * purcentageInState).ToString() + Main.Instance.AllEndGameBonus[0].addedCharacter;
                    maskProgress.fillAmount = purcentageInState * (Main.Instance.AllEndGameBonus[0].currValue / Main.Instance.AllEndGameBonus[0].maxValue);
                }
                break;

            // --- Anim de la progress bar (réussite ou echec)
            case stateProgress.anim:
                if (upPurcentage(timeAnim))
                {
                    if (Main.Instance.AllEndGameBonus[0].currValue >= Main.Instance.AllEndGameBonus[0].maxValue)
                    {
                        SpawnNewBonusInstance("+ " + Main.Instance.AllEndGameBonus[0].addedScore.ToString("N0"), Main.Instance.AllEndGameBonus[0].title, Main.Instance.AllEndGameBonus[0].description);
                        UILeaderboard.Instance.addScore(Main.Instance.AllEndGameBonus[0].addedScore);
                        explosionCurrentMultiplier = 1 + UILeaderboard.Instance.Score * (explosionForceMultiplierByScore -1)/ refForMaxSizeAndShake;
                    }
                    progressState = stateProgress.depop;
                }
                else
                {

                }
                break;

            // --- Disparition de la progress bar
            case stateProgress.depop:
                if (upPurcentage(timeDepop))
                {
                    cvsGroupAlpha = 0;
                    maskProgress.fillAmount = 0;
                    progressState = stateProgress.staydepop;
                }
                else
                {
                    cvsGroupAlpha = 1-purcentageInState;
                }
                break;

            // --- Temps invisible de la progress bar
            case stateProgress.staydepop:
                if (upPurcentage(timeStayDepop))
                {
                    progressState = stateProgress.paused;
                }
                else
                {
                    cvsGroupAlpha = 0;
                }
                break;

            // --- Pause de l'état actuel
            case stateProgress.paused:
                if (allowToNext)
                {
                    if (!firstLoop && Main.Instance.AllEndGameBonus.Count > 0)
                    {
                        Main.Instance.AllEndGameBonus.RemoveAt(0);
                    }
                    if (firstLoop)
                    {
                        currScoreDisplayed = UILeaderboard.Instance.Score;
                        scoreText.text = Mathf.RoundToInt(currScoreDisplayed).ToString("N0");
                    }
                    if (Main.Instance.AllEndGameBonus.Count > 0)
                    {
                        firstLoop = false;
                        progressState = stateProgress.pop;
                        purcentageInState = 0;
                        cvsGroupAlpha = 0;
                        maskProgress.fillAmount = 0;
                        transformToScaleInSuccesAnim.localScale = Vector3.one;
                        transformToScaleInFailureAnim.localScale = Vector3.one;
                        typeText.text = Main.Instance.AllEndGameBonus[0].type;
                        numberText.text = "0" + Main.Instance.AllEndGameBonus[0].addedCharacter;
                        numberMaxText.text = "/" + Main.Instance.AllEndGameBonus[0].maxValue + Main.Instance.AllEndGameBonus[0].addedCharacter;
                        colorToChangeProgress.color = colorProgressNormal;
                    }
                }
                break;
        }
        cvsGroupProgress.alpha = cvsGroupAlpha;
        outlineBar.effectColor = new Color(outlineBar.effectColor.r, outlineBar.effectColor.g, outlineBar.effectColor.b, Mathf.Pow (cvsGroupAlpha, outlineAlphaPow));
        #endregion

        if (currentPurcentageExplosion < 1)
        {
            currentPurcentageExplosion += dt / timeExplosion;
            explosionImage.enabled = true;
            explosionImage.transform.localScale = Vector3.Lerp(Vector3.one * scaleMinExplosion, Vector3.one * scaleMaxExplosion, currentPurcentageExplosion);
            explosionImage.color = Color.Lerp(explosionColorStart, explosionColorEnd, currentPurcentageExplosion);
            if (currentPurcentageExplosion > 1)
            {
                currentPurcentageExplosion = 1;
                explosionImage.enabled = false;
            }
        }


        //if (Input.GetKeyDown(KeyCode.H)) SpawnNewBonusInstance("+ 50 000", "test titre", "test description de titre long");
    }

    void HandleExplosionAnim()
    {
        if (doAnimExplosion)
        {
            doAnimExplosion = !explosionAnim.AddPurcentage(animExplosionPurcentage, dt, out animExplosionPurcentage);
            for (int i = 0; i < affectedByExplosion.Length; i++)
            {
                Vector3 dir = Vector3.Normalize(ildedPos[i] - explosionSource.position);
                affectedByExplosion[i].transform.position = ildedPos[i];
                affectedByExplosion[i].transform.Translate(dir * explosionAnim.ValueAt(animExplosionPurcentage) * explosionCurrentMultiplier, Space.World);

            }
        }
    }

    bool upPurcentage (float time)
    {
        if (purcentageInState < 1)
        {
            purcentageInState += dt / time;
            if (purcentageInState > 1)
            {
                purcentageInState = 0;
                return true;
            }
            return false;
        }
        return false;
    }

    void DoProgressBarAnim()
    {
        if (playAnimProgressSucces)
        {
            playAnimProgressSucces = !progressSuccesAnim.AddPurcentage(progressAnimPurcentage, dt, out progressAnimPurcentage);
            transformToScaleInSuccesAnim.localScale = Vector3.one + Vector3.one * progressSuccesAnim.ValueAt(progressAnimPurcentage);
            colorToChangeProgress.color = colorProgressSucces;
        }
        if (playAnimFailureSucces)
        {
            playAnimFailureSucces = !progressFailureAnim.AddPurcentage(progressAnimPurcentage, dt, out progressAnimPurcentage);
            transformToScaleInFailureAnim.localScale = Vector3.one + Vector3.one * progressFailureAnim.ValueAt(progressAnimPurcentage);
            colorToChangeProgress.color = colorProgressFailure;
        }
    }

    public void SpawnNewBonusInstance(string _scoreText, string _titleText, string _descriptionText)
    {
        if (UILeaderboard.Instance.CurrentScreen == UILeaderboard.leaderboardScreens.startLeaderboard)
        {
            if (bonusInstances.Count < maxBonusDisplayed)
            {
                GameObject curr = Instantiate(prefabBonusHandler, layoutGroup);
                curr.transform.SetAsFirstSibling();
                BonusGraphInstanceHandler scriptBonusInstance = curr.GetComponent<BonusGraphInstanceHandler>();
                scriptBonusInstance.manager = this;
                scriptBonusInstance.InitGraph(_scoreText, _titleText, _descriptionText);
                bonusInstances.Add(scriptBonusInstance);
            }
            else
            {
                Transform curr = layoutGroup.GetChild(layoutGroup.childCount - 1);
                curr.SetAsFirstSibling();
                BonusGraphInstanceHandler scriptBonusInstance = curr.GetComponent<BonusGraphInstanceHandler>();
                scriptBonusInstance.InitGraph(_scoreText, _titleText, _descriptionText);
            }
        }
    }

}

public class EndGameBonus
{
    public float currValue;
    public float maxValue;
    public string type;
    public int addedScore;
    public string title;
    public string description;
    public Sprite sprite;
    public string addedCharacter;

    public EndGameBonus(float _currValue, float _maxValue, string _type, int _addedScore, string _title, string _description, Sprite _sprite, string _addedCharacter)
    {
        currValue = _currValue;
        maxValue = _maxValue;
        type = _type;
        addedScore = _addedScore;
        title = _title;
        description = _description;
        sprite = _sprite;
        addedCharacter = _addedCharacter;
    }

}