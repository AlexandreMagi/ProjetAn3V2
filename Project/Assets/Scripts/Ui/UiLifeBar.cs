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

    [SerializeField] DataLifeBarUi dataLifebar = null;

    [SerializeField] Transform rootVerticalShield = null;
    [SerializeField] Transform rootMiddleShield = null;

    [SerializeField] GameObject[] lifeCapsules = new GameObject[0];
    [SerializeField] GameObject gameOverRoot = null;

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

    private void Start()
    {
        stockArmor = Player.Instance.GetBaseValues().x;
        stockMaxArmor = stockArmor;
        stockLife = Player.Instance.GetBaseValues().y;
        stockMaxLife = stockLife;
    }

    private void Update()
    {
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

        rootVerticalShield.transform.localScale = Vector3.Lerp (new Vector3(1, lastArmor / stockMaxArmor, 1), new Vector3(1, stockArmor / stockMaxArmor, 1), currentRecoverPurcentage);
        flickerShield.moveRange = Mathf.Lerp(maxFlicker, 0, stockArmor / stockMaxArmor);
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
                    Debug.Log("Armure prend dégats");
                }
                else
                {
                    Debug.Log("Armure casse");
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
    }

    public void UpdateCapsules(float life)
    {
        for (int i = 0; i < lifeCapsules.Length; i++)
        {
            lifeCapsules[i].SetActive(i < life / stockMaxLife * lifeCapsules.Length);
        }
        stockLife = life;
    }
    public void UpdateArmor(float armor)
    {
        //rootVerticalShield.transform.localScale = new Vector3(1, armor / stockMaxArmor, 1);
        stockArmor = armor;
    }
    public void EndGame()
    {
        gameOverRoot.SetActive(true);
    }

}
