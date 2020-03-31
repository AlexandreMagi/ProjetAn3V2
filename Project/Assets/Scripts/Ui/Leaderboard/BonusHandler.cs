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

    bool firstLoop = true;

    float currScoreDisplayed = 0;
    [SerializeField] float scoreLerpSpeed = 8;

    [HideInInspector] public bool allowToNext = false;

    private void Start()
    {
        cvsGroupAlpha = 0;
        maskProgress.fillAmount = 0;

        currScoreDisplayed = UILeaderboard.Instance.Score;
        scoreText.text = Mathf.RoundToInt(currScoreDisplayed).ToString("N0");
    }

    void Update()
    {
        float dt = Time.unscaledDeltaTime * UILeaderboard.Instance.deltaTimeMultiplier;
        currScoreDisplayed = Mathf.Lerp(currScoreDisplayed, UILeaderboard.Instance.Score, dt * scoreLerpSpeed);
        scoreText.text = Mathf.RoundToInt(currScoreDisplayed).ToString("N0");

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
                        typeText.text = Main.Instance.AllEndGameBonus[0].type;
                        numberText.text = "0" + Main.Instance.AllEndGameBonus[0].addedCharacter;
                        numberMaxText.text = "/" + Main.Instance.AllEndGameBonus[0].maxValue + Main.Instance.AllEndGameBonus[0].addedCharacter;
                    }
                }
                break;
        }
        cvsGroupProgress.alpha = cvsGroupAlpha;
        outlineBar.effectColor = new Color(outlineBar.effectColor.r, outlineBar.effectColor.g, outlineBar.effectColor.b, Mathf.Pow (cvsGroupAlpha, outlineAlphaPow));
        #endregion

        //if (Input.GetKeyDown(KeyCode.H)) SpawnNewBonusInstance("+ 50 000", "test titre", "test description de titre long");
    }

    bool upPurcentage (float time)
    {
        if (purcentageInState < 1)
        {
            float dt = Time.unscaledDeltaTime * UILeaderboard.Instance.deltaTimeMultiplier;
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