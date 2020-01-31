using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDamageHandler : MonoBehaviour
{
    #region Singleton
    private static UiDamageHandler _instance;
    public static UiDamageHandler Instance
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
#endregion

    List<GameObject> spritesDisplayed = new List<GameObject>();
    List<SpriteDisplayedInstance> spritesHandler = new List<SpriteDisplayedInstance>();
    [SerializeField] GameObject emptyUiBox = null;

    [SerializeField] DataUiTemporarySprite dataToSend;

    [SerializeField]
    Transform rootDammage = null;

    [SerializeField]
    DataDamageFb damageFeedbackData = null;
    float stateTimeRemaining = 0;
    float shieldBreakFlashTime = 0;
    [SerializeField]
    GameObject statePanel = null;
    [SerializeField]
    GameObject flashPanel = null;
    [SerializeField]
    GameObject shieldBreakFlash = null;
    [SerializeField]
    GameObject shieldPanel = null;
    [SerializeField]
    GameObject zeroGPanel = null;

    private void Update()
    {
        for (int i = spritesDisplayed.Count-1; i > -1; i--)
        {
            spritesHandler[i].UpdateValues();
            if (i < spritesDisplayed.Count && spritesHandler[i] != null)
            {
                spritesDisplayed[i].GetComponent<Image>().color = spritesHandler[i].currentColor;
                spritesDisplayed[i].transform.localScale = Vector3.one * spritesHandler[i].scale;
            }
        }

        // ###### SHIELD ET VIE ###### //

        if (stateTimeRemaining > 0)
        {
            Player player = GameObject.FindObjectOfType<Player>();
            if (player != null && player.getArmor() > 0)
            {
                shieldPanel.SetActive(true);
                shieldPanel.GetComponent<Image>().color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, stateTimeRemaining / damageFeedbackData.stateTime);
                statePanel.GetComponent<Image>().color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, stateTimeRemaining / damageFeedbackData.stateTime);
                flashPanel.GetComponent<Image>().color = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, damageFeedbackData.flashAlpha);
            }
            else
            {
                statePanel.GetComponent<Image>().color = new Color(Color.red.r, Color.red.g, Color.red.b, stateTimeRemaining / damageFeedbackData.stateTime);
                flashPanel.GetComponent<Image>().color = new Color(Color.red.r, Color.red.g, Color.red.b, damageFeedbackData.flashAlpha);
            }

            statePanel.SetActive(true);
            flashPanel.SetActive(true);
            stateTimeRemaining -= Time.unscaledDeltaTime;

            if (stateTimeRemaining < 0)
            {
                statePanel.SetActive(false);
                shieldPanel.SetActive(false);
            }
            if (stateTimeRemaining < damageFeedbackData.stateTime - damageFeedbackData.flashTime)
            flashPanel.SetActive(false);
        }
        if (shieldBreakFlashTime > 0)
        {
            shieldBreakFlash.SetActive(true);
            shieldBreakFlashTime -= Time.unscaledDeltaTime;
            if (shieldBreakFlashTime < 0)
                shieldBreakFlash.SetActive(false);
        }
    }

    public void ClearScreen()
    {

    }

    public void UpdateZeroGScreen(DataZeroGOnPlayer datasend, float purcentage, bool onZeroG)
    {
        zeroGPanel.SetActive(onZeroG);
        Color zeroGColor = zeroGPanel.GetComponent<Image>().color;
        zeroGPanel.GetComponent<Image>().color = new Color(zeroGColor.r, zeroGColor.g, zeroGColor.b, datasend.screenOpacity.Evaluate(purcentage));
    }

    public void AddSprite (DataUiTemporarySprite dataSendShield, DataUiTemporarySprite dataSendLife)
    {
        Player player = GameObject.FindObjectOfType<Player>();
        DataUiTemporarySprite dataSend = null;
        if (player != null && player.getArmor() > 0) dataSend = dataSendShield;        
        else dataSend = dataSendLife;


        GameObject newSprite = Instantiate(emptyUiBox, rootDammage.transform);
        newSprite.GetComponent<Image>().sprite = dataSend.spriteToSend;
        newSprite.transform.Rotate(0, 0, Random.Range(0f, 360f), Space.Self);
        newSprite.GetComponent<RectTransform>().sizeDelta = Vector2.one * Random.Range(dataSend.sizeRandom.x, dataSend.sizeRandom.y);
        newSprite.transform.localScale = Vector3.zero;
        Outline stockoutline = newSprite.AddComponent<Outline>();
        stockoutline.effectDistance = new Vector2 (-1,1) * dataSend.outlineDistance;
        stockoutline.effectColor = dataSend.colorOutline;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, new Vector2 (Screen.width * (0.5f + Random.Range(-dataSend.rangePositionRandom, dataSend.rangePositionRandom)), Screen.height * (0.5f + Random.Range(-dataSend.rangePositionRandom, dataSend.rangePositionRandom))), this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
        newSprite.transform.position = transform.TransformPoint(pos);

        spritesDisplayed.Add(newSprite);

        SpriteDisplayedInstance newOne = new SpriteDisplayedInstance();
        newOne.OnCreation(dataSend);
        spritesHandler.Add(newOne);

        stateTimeRemaining = damageFeedbackData.stateTime;
    }

    public void deleteSpot(SpriteDisplayedInstance spriteInstance)
    {
        for (int i = 0; i < spritesHandler.Count; i++)
        {
            if (spritesHandler[i] == spriteInstance)
            {
                spritesHandler.RemoveAt(i);
                GameObject stock = spritesDisplayed[i];
                spritesDisplayed.RemoveAt(i);
                Destroy(stock);
            }
        }
    }

    public void ShieldBreak()
    {
        shieldBreakFlashTime = damageFeedbackData.shieldBreakFlash;
    }

}
