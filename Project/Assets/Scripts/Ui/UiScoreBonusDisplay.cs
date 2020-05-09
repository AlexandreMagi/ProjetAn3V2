using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScoreBonusDisplay : MonoBehaviour
{
    #region Singleton
    private static UiScoreBonusDisplay _instance;
    public static UiScoreBonusDisplay Instance
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

    List<GameObject> textDisplayed = new List<GameObject>();
    List<ScoreBonusDisplayedInstance> scoresBonusHandler = new List<ScoreBonusDisplayedInstance>();
    [SerializeField] GameObject emptyUiText = null;

    [SerializeField] DataUiTemporaryText dataToSend = null;

    [SerializeField]
    Transform rootScoreBonus = null;


    [SerializeField] UIParticuleSystem thumbsUpVFX = null;
    [SerializeField] UIParticuleSystem thumbsDownVFX = null;

    private void Update()
    {

        for (int i = textDisplayed.Count-1; i > -1; i--)
        {
            scoresBonusHandler[i].UpdateValues();
            if (i < textDisplayed.Count && scoresBonusHandler[i] != null)
            {
                textDisplayed[i].GetComponent<Text>().color = scoresBonusHandler[i].currentColor;
                textDisplayed[i].transform.localScale = Vector3.one * scoresBonusHandler[i].scale;
                if (scoresBonusHandler[i].isPlacedOnWorld) MoveSprite(textDisplayed[i], scoresBonusHandler[i]);
            }
        }
    }

    public void BonusAcquired()
    {
        thumbsUpVFX.Play();
    }
    public void MalusAcquired()
    {
        thumbsDownVFX.Play();
    }


    public void AddScoreBonus (string textSend, bool good)
    {
        ScoreBonusDisplayedInstance handler;
        GameObject newText = createObject(textSend, good, out handler);
        Vector2 pos;
        Vector2 posInput = new Vector2(Screen.width * (0.5f + Random.Range(-dataToSend.randomPos, dataToSend.randomPos)), Screen.height * (0.5f + Random.Range(-dataToSend.randomPos, dataToSend.randomPos)));
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, posInput, GetComponent<Canvas>().worldCamera, out pos);
        newText.transform.position = transform.TransformPoint(pos);
        MaybePlayCheer();
    }


    public void AddScoreBonus(string textSend, bool good, Vector3 posInit, float randomPosAdded = 0)
    {
        ScoreBonusDisplayedInstance handler;
        GameObject newText = createObject(textSend, good,out handler);
        handler.IsPlacedInWorld(true, posInit + new Vector3(Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded)));
        MoveSprite(newText, handler);
        MaybePlayCheer();
    }

    public void MaybePlayCheer()
    {
        if (Random.Range(0f,100f)< 30f)
        {
            /*if (Random.Range(0f, 100f) < 50)
                CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Crowd_Cheer", false, 0.3f, 0.1f);
            else
                CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Crowd_Cheer2", false, 0.3f, 0.1f);*/
            if (Random.Range(0f, 100f) < 50)
                CustomSoundManager.Instance.PlaySound("Crowd_Cheer", "PublicAmbiant", null, 0.3f,false,1, 0.1f);
            else
                CustomSoundManager.Instance.PlaySound("Crowd_Cheer2", "PublicAmbiant", null, 0.3f, false, 1, 0.1f);
        }
    }


    public void AddScoreBonus(string textSend, bool good, Vector3 posInit, Color SpecificColor, float randomPosAdded = 0)
    {
        ScoreBonusDisplayedInstance handler;
        GameObject newText = createObject(textSend, good, SpecificColor, out handler);
        handler.IsPlacedInWorld(true, posInit + new Vector3(Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded)));
        MoveSprite(newText, handler);
    }

    void MoveSprite(GameObject textObject, ScoreBonusDisplayedInstance handler)
    {
        Vector2 pos;
        Vector3 posScreen = CameraHandler.Instance.renderingCam.WorldToScreenPoint(handler.posSave);
        if (posScreen.z > 0)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, posScreen, GetComponent<Canvas>().worldCamera, out pos);
            textObject.transform.position = transform.TransformPoint(pos);
        }
        else
        {
            textObject.transform.position = -Vector3.one * Screen.width;
        }
    }

    GameObject createObject(string textSend, bool good,out ScoreBonusDisplayedInstance newOne)
    {
        GameObject newText = Instantiate(emptyUiText, rootScoreBonus.transform);
        Text textThis = newText.GetComponent<Text>();
        textThis.text = textSend;
        textThis.color = good? dataToSend.colorGood : dataToSend.colorBad;
        textThis.fontSize = Mathf.RoundToInt(dataToSend.fontSize);

        newText.transform.localScale = Vector3.zero;
        Outline stockoutline = newText.AddComponent<Outline>();
        stockoutline.effectDistance = new Vector2(-1, 1) * dataToSend.outlineDistance;
        stockoutline.effectColor = dataToSend.colorOutline;

        textDisplayed.Add(newText);

        newOne = new ScoreBonusDisplayedInstance();
        newOne.OnCreation(dataToSend, textThis.color);
        scoresBonusHandler.Add(newOne);
        return newText;
    }
    GameObject createObject(string textSend, bool good, Color specificColor, out ScoreBonusDisplayedInstance newOne)
    {
        GameObject newText = Instantiate(emptyUiText, rootScoreBonus.transform);
        Text textThis = newText.GetComponent<Text>();
        textThis.text = textSend;
        textThis.color = good? dataToSend.colorGood : dataToSend.colorBad;
        textThis.fontSize = Mathf.RoundToInt(dataToSend.fontSize);

        newText.transform.localScale = Vector3.zero;
        Outline stockoutline = newText.AddComponent<Outline>();
        stockoutline.effectDistance = new Vector2(-1, 1) * dataToSend.outlineDistance;
        stockoutline.effectColor = dataToSend.colorOutline;

        textDisplayed.Add(newText);

        newOne = new ScoreBonusDisplayedInstance();
        newOne.OnCreation(dataToSend, specificColor);
        scoresBonusHandler.Add(newOne);
        return newText;
    }

    public void deleteSpot(ScoreBonusDisplayedInstance spriteInstance)
    {
        for (int i = 0; i < scoresBonusHandler.Count; i++)
        {
            if (scoresBonusHandler[i] == spriteInstance)
            {
                scoresBonusHandler.RemoveAt(i);
                GameObject stock = textDisplayed[i];
                textDisplayed.RemoveAt(i);
                Destroy(stock);
            }
        }
    }

}
