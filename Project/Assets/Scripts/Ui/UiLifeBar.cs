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

    //[SerializeField]
    //Slider lifeBar = null;
    //[SerializeField]
    //Slider lifeBarFeedBack = null;
    //[SerializeField]
    //Slider armorBar = null;
    //[SerializeField]
    //Slider armorBarFeedBack = null;
    ////[SerializeField]
    ////Text lifeText = null;
    //[SerializeField]
    //Text armorText = null;

    int nbLife = 5;
    int nbLifeMax = 5;
    float bps = 2f;
    float timerBps = 0;
    float stockArmor = 300;
    float stockLife = 300;

    [SerializeField] DataLifeBarUi dataLifebar = null;
    [SerializeField] Transform lifeBarRoot = null;

    LifeCapsuleInstance[] dataHandlers = new LifeCapsuleInstance[0];
    [SerializeField] GameObject emptyUiBox = null;
    GameObject[] capsules = new GameObject[0];

    [SerializeField] GameObject[] armorBars = null;
    [SerializeField] ArmorCapsuleInstance[] armorValues = null;

    [SerializeField] DataArmorBarUi armorBarData = null;

    [SerializeField] GameObject gameOverRoot = null;

    private void Start()
    {
        stockArmor = Player.Instance.GetBaseValues().x;
        stockLife = Player.Instance.GetBaseValues().y;

        armorValues = new ArmorCapsuleInstance[armorBars.Length];
        for (int i = 0; i < armorValues.Length; i++)
        {
            armorValues[i] = new ArmorCapsuleInstance(armorBarData, armorBars[i].GetComponent<RectTransform>(), armorBars[i].GetComponent<Image>(), armorBars[i].GetComponent<Outline>());
            armorValues[i].stockArmor = stockArmor / armorValues.Length;
            armorValues[i].currentArmor = stockArmor / armorValues.Length;
        }

        dataHandlers = new LifeCapsuleInstance[nbLife];
        capsules = new GameObject[nbLife];
        float distanceBetweenCapsule = dataLifebar.purcentageUsedY * Screen.height / nbLife;
        bps = dataLifebar.startBps;

        for (int i = 0; i < nbLife; i++)
        {
            capsules[i] = Instantiate(emptyUiBox, lifeBarRoot);
            capsules[i].GetComponent<Image>().sprite = dataLifebar.capsuleSprite;

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, new Vector2(0, i * distanceBetweenCapsule) + new Vector2(Screen.width * dataLifebar.decalSprites.x, Screen.height * dataLifebar.decalSprites.y), this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
            capsules[i].transform.position = transform.TransformPoint(pos);

            capsules[i].GetComponent<RectTransform>().sizeDelta = dataLifebar.baseSize;
            capsules[i].GetComponent<Image>().color = dataLifebar.lifeCapsuleColor;
            Outline componentOutline = capsules[i].AddComponent<Outline>();
            componentOutline.effectColor = Color.black;
            componentOutline.effectDistance = new Vector2(1, -1) * dataLifebar.outlineSize;
            dataHandlers[i] = new LifeCapsuleInstance(dataLifebar, i, capsules[i].GetComponent<RectTransform>(), capsules[i].GetComponent<Image>(), capsules[i].GetComponent<Outline>());
        }
    }

    //public void UpdateLifeDisplay(float value, float realValue)
    //{
    //    lifeBar.value = value;
    //    //lifeText.text = Mathf.CeilToInt(realValue).ToString();
    //}

    //public void UpdateArmorDisplay(float value, float realValue)
    //{
    //    armorBar.value = value;
    //    armorText.text = Mathf.CeilToInt(realValue).ToString();
    //}
    public void AddLife()
    {
        dataHandlers[nbLife].activate();
        nbLife++;
        if (nbLife > nbLifeMax) nbLife--;
        stockLife += Player.Instance.GetBaseValues().y / nbLifeMax;
    }
    public void AddArmor(float addedArmor)
    {
        for (int i = 0; i < armorValues.Length; i++)
        {
            armorValues[i].activate();
            if (armorValues[i].currentArmor + addedArmor < armorValues[i].stockArmor)
            {
                armorValues[i].currentArmor += addedArmor;
                stockArmor += addedArmor;
                addedArmor = 0;
                break;
            }
            else
            {
                float valueAdded = armorValues[i].stockArmor - armorValues[i].currentArmor;
                addedArmor -= valueAdded;
                armorValues[i].stockArmor = armorValues[i].currentArmor;
                stockArmor += valueAdded;
            }
        }
    }

    public void Update()
    {
        //if  (lifeBarFeedBack.value < lifeBar.value)     lifeBarFeedBack.value = lifeBar.value;
        //else lifeBarFeedBack.value =                    Mathf.MoveTowards(lifeBarFeedBack.value, lifeBar.value, Time.deltaTime);
        //if  (armorBarFeedBack.value < armorBar.value)   armorBarFeedBack.value = armorBar.value;
        //else armorBarFeedBack.value =                   Mathf.MoveTowards(armorBarFeedBack.value, armorBar.value, Time.deltaTime);

        //if (Input.GetKeyDown(KeyCode.Space)) PlayerTookDamage(300, 100);
        //if (Input.GetKeyDown(KeyCode.Space)) Player.Instance.TakeDamage(25);


        for (int i = 0; i < dataHandlers.Length; i++)
        {
            dataHandlers[i].UpdateValues();
            dataHandlers[i].DoValues();
            dataHandlers[i].outlineLocal.effectDistance = new Vector2(1, -1) * dataHandlers[i].outlineSize;
        }

        for (int i = 0; i < armorValues.Length; i++)
        {
            armorValues[i].UpdateValues();
            armorValues[i].DoValues();
            armorValues[i].outlineLocal.effectDistance = new Vector2(1, -1) * armorValues[i].outlineSize;
        }

        bps -= dataLifebar.recoverBps * Time.unscaledDeltaTime;
        bps = Mathf.Clamp(bps, dataLifebar.startBps, dataLifebar.maxBps);

        timerBps -= Time.unscaledDeltaTime * bps;
        if (timerBps < 0)
        {
            timerBps += 1;
            for (int i = 0; i < nbLife; i++)
            {
                dataHandlers[i].Beat(i * 0.05f);
            }
        }

    }

    public void PlayerTookDamage(float armor, float life)
    {
        if (armor < stockArmor)
        {
            float damageValue = stockArmor - armor;
            bps += dataLifebar.addedBpsShield;

            for (int i = armorBars.Length - 1; i > -1; i--)
            {
                if (damageValue > 0)
                {
                    if (armorValues[i].currentArmor > damageValue)
                    {
                        armorValues[i].currentArmor -= damageValue;
                        armorValues[i].TakeDammage(damageValue / armorValues[i].stockArmor);
                        damageValue = 0;
                        // Feedback armor qui prend des dégats.
                    }
                    else
                    {
                        damageValue -= armorValues[i].currentArmor;
                        armorValues[i].currentArmor = 0;
                        armorValues[i].desactivate();
                        // Feedback armor qui casse.
                    }
                }
            }
            stockArmor = armor;
        }
        
        if (life < stockLife)
        {
            bps += dataLifebar.addedBps;
            for (int i = 0; i < nbLife; i++)
            {
                dataHandlers[i].TakeDammage((nbLife - i) * 0.05f);
            }
            if (nbLife > 0) nbLife--;
            dataHandlers[nbLife].desactivate();

            stockLife = life;
        }
    }

    public void EndGame()
    {
        gameOverRoot.SetActive(true);
    }

}
