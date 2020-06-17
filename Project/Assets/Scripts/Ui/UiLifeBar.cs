using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLifeBar : MonoBehaviour
{
    private static UiLifeBar _instance;
    public static UiLifeBar Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }

    [SerializeField] RectTransform RootLifeBarReducing = null;
    [SerializeField] CanvasGroup CvsGroupLifeBar = null;
    [SerializeField] float timeUnusedToReduce = 5;
    [SerializeField] float timeToReduce = 1;
    [SerializeField] float timeToGrow = 0.5f;
    [SerializeField] float aimedScale = 0.5f;
    [SerializeField] float aimedAlpha = 0.5f;

    float timeRemainingReducing = 0;

    [SerializeField] RectTransform rectRootArmor = null;
    //[SerializeField] Transform rootVerticalShield = null;
    [SerializeField] Transform rootMiddleShield = null;
    float baseHeightMask = 0;

    [SerializeField] GameObject[] lifeCapsules = new GameObject[0];
    [SerializeField] GameObject gameOverRoot = null;
    [SerializeField] Image fonduNoirGameOver = null;
    bool mustFondu = false;
    [SerializeField] float speedAlphaFonduGameOver = 0.3f;

    [SerializeField] AnimationCurve animDamageShield = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float animDamageShieldAmplitude = 0.5f;
    [SerializeField] float animDamageShieldTime = 0.5f;
    float animDamageShieldPurcentage = 1;

    [SerializeField] float recoverLerpSpeed = 5;
    [SerializeField] float baseRecoverAlpha = 0.3f;
    [SerializeField] Image recoverCalqueOver = null;
    [SerializeField] ScriptFlicker flickerShield = null;
    [SerializeField] float maxFlicker = 7;
    float currentRecoverPurcentage = 1;
    float lastArmor = 300;

    float stockArmor = 300;
    float stockMaxArmor = 300;
    float stockLife = 300;
    float stockMaxLife = 300;

    bool endGameAnimPlayed = false;
    
    [SerializeField] Text shieldDisplayText = null;
    [SerializeField] Text lifeDisplayText = null;

    [SerializeField] Image matShaderShield = null;
    Material matShaderShieldInstance = null;

    [SerializeField] Image fullScreenShield = null;
    [SerializeField] float timeShieldRemainVisible = 1;
    [SerializeField] float timeShieldFade = 0.2f;
    float timeShieldRemaining = 0;

    private void Start()
    {
        stockArmor = Player.Instance.getArmor();
        stockMaxArmor = Player.Instance.GetBaseValues().x;
        stockLife = Player.Instance.GetBaseValues().y;
        stockMaxLife = stockLife;

        baseHeightMask = rectRootArmor.sizeDelta.y;

        matShaderShieldInstance = matShaderShield.material;
    }

    private void Update()
    {

        UpdateScaleIfUsed();

        if (animDamageShieldPurcentage < 1)
        {
            rootMiddleShield.transform.localScale = Vector3.one + Vector3.one * animDamageShield.Evaluate(animDamageShieldPurcentage) * animDamageShieldAmplitude;
            animDamageShieldPurcentage += Time.unscaledDeltaTime / animDamageShieldTime;
            if (animDamageShieldPurcentage > 1)
            {
                animDamageShieldPurcentage = 1;
                rootMiddleShield.transform.localScale = Vector3.one;
            }
        }

        currentRecoverPurcentage = Mathf.Lerp(currentRecoverPurcentage, 1, Time.unscaledDeltaTime * recoverLerpSpeed);
        recoverCalqueOver.color = new Color(Color.white.r, Color.white.g, Color.white.b, (1-currentRecoverPurcentage) * baseRecoverAlpha);

        rectRootArmor.sizeDelta = new Vector2(rectRootArmor.sizeDelta.x, Mathf.Lerp(lastArmor / stockMaxArmor * baseHeightMask, stockArmor / stockMaxArmor * baseHeightMask, currentRecoverPurcentage));
        //rectRootArmor.sizeDelta = new Vector2 (100,100);

        matShaderShieldInstance.SetFloat("_Mask", 1-Mathf.Lerp(lastArmor / stockMaxArmor, stockArmor / stockMaxArmor, currentRecoverPurcentage));
        shieldDisplayText.text = Mathf.RoundToInt((stockArmor/ stockMaxArmor) * 100).ToString();

        //rootVerticalShield.transform.localScale = Vector3.Lerp (new Vector3(1, lastArmor / stockMaxArmor, 1), new Vector3(1, stockArmor / stockMaxArmor, 1), currentRecoverPurcentage);
        //rootVerticalShield.transform.localScale = Vector3.Lerp (rootVerticalShield.transform.localScale, new Vector3(1, stockArmor / stockMaxArmor, 1), Time.unscaledDeltaTime * recoverLerpSpeed);
        flickerShield.moveRange = Mathf.Lerp(maxFlicker, 0, stockArmor / stockMaxArmor);


        if (mustFondu)
        {
            fonduNoirGameOver.color = new Color(0, 0, 0, Mathf.MoveTowards(fonduNoirGameOver.color.a, 1, Time.unscaledDeltaTime * speedAlphaFonduGameOver));
        }

        if (timeShieldRemaining > 0)
        {
            timeShieldRemaining -= Time.unscaledDeltaTime;

            if (timeShieldRemaining < timeShieldFade)
                fullScreenShield.color = new Color(fullScreenShield.color.r, fullScreenShield.color.g, fullScreenShield.color.b, timeShieldRemaining / timeShieldFade);
            else if (timeShieldRemaining > timeShieldRemainVisible - timeShieldFade)
                fullScreenShield.color = new Color(fullScreenShield.color.r, fullScreenShield.color.g, fullScreenShield.color.b, 1 - ((timeShieldRemaining - (timeShieldRemainVisible - timeShieldFade)) / timeShieldFade));
            else
                fullScreenShield.color = new Color(fullScreenShield.color.r, fullScreenShield.color.g, fullScreenShield.color.b, 1);
            if (timeShieldRemaining < 0)
                fullScreenShield.color = new Color(fullScreenShield.color.r, fullScreenShield.color.g, fullScreenShield.color.b, 0);
        }


    }

    void UpdateScaleIfUsed()
    {
        timeRemainingReducing -= Time.unscaledDeltaTime;
        if (timeRemainingReducing > 0)
        {
            if (timeRemainingReducing < timeToReduce)
            {
                CvsGroupLifeBar.alpha = Mathf.Lerp(1, aimedAlpha, 1 - timeRemainingReducing / timeToReduce);
                RootLifeBarReducing.localScale = Vector3.one * Mathf.Lerp(1, aimedScale, 1 - timeRemainingReducing / timeToReduce);
            }
            else
            {
                CvsGroupLifeBar.alpha = Mathf.MoveTowards(CvsGroupLifeBar.alpha, 1, Time.unscaledDeltaTime / timeToGrow);
                RootLifeBarReducing.localScale = Vector3.one * Mathf.MoveTowards(RootLifeBarReducing.localScale.x, 1, Time.unscaledDeltaTime / timeToGrow);
            }
        }
        else
        {
            CvsGroupLifeBar.alpha = aimedAlpha;
            RootLifeBarReducing.localScale = Vector3.one * aimedScale;
        }
    }

    public void PlayerTookDamage(float armor, float life)
    {
        lastArmor = stockArmor;
        currentRecoverPurcentage = 0;
        if (armor < stockArmor)
        {
            float damageValue = stockArmor - armor;

            if (damageValue > 0)
            {
                if (armor > 0)
                {
                    //Debug.Log("Armure prend dégats");
                }
                else
                {
                    //Debug.Log("Armure casse");
                }
                UpdateArmor(armor);
            }

            stockArmor = armor;

        }
            
        if (life < stockLife)
        {
            UpdateCapsules(life);
            stockLife = life;
        }
        animDamageShieldPurcentage = 0;
        timeRemainingReducing = timeUnusedToReduce + timeToReduce;
    }

    public void UpdateCapsules(float life)
    {
        for (int i = 0; i < lifeCapsules.Length; i++)
        {
            lifeCapsules[i].SetActive(i < life / stockMaxLife * lifeCapsules.Length);
        }
        lifeDisplayText.text = Mathf.RoundToInt(life / stockMaxLife * lifeCapsules.Length).ToString();
        stockLife = life;
        timeRemainingReducing = timeUnusedToReduce + timeToReduce;
    }
    public void UpdateArmor(float armor)
    {
        //rootVerticalShield.transform.localScale = new Vector3(1, armor / stockMaxArmor, 1);
        if (stockArmor <= armor)
        {
            lastArmor = armor;
            timeShieldRemaining = timeShieldRemainVisible;
        }

        stockArmor = armor;
        currentRecoverPurcentage = 0;
        timeRemainingReducing = timeUnusedToReduce + timeToReduce;
    }



    public void EndGame()
    {
        if (!endGameAnimPlayed)
        {
            gameOverRoot.SetActive(true);
            mustFondu = true;
            endGameAnimPlayed = true;
            gameOverRoot.GetComponent<Animator>().SetTrigger("pop");
            fonduNoirGameOver.color = new Color(0, 0, 0, 0);
        }
    }

}
