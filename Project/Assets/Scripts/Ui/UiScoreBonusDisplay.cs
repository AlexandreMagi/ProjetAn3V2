using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    List<Text> textDisplayed = new List<Text>();
    List<ScoreBonusDisplayedInstance> scoresBonusHandler = new List<ScoreBonusDisplayedInstance>();
    [SerializeField] GameObject emptyUiText = null;

    [SerializeField] DataUiTemporaryText dataToSend = null;

    [SerializeField]
    Transform rootScoreBonus = null;


    [SerializeField] UIParticuleSystem thumbsUpVFX = null;
    [SerializeField] UIParticuleSystem thumbsDownVFX = null;

    [SerializeField] bool desactivateUiBonusDisplay = false;

    int currCheer = 0;
    [SerializeField] int neededCheer = 3;
    bool canComment = false;
    [SerializeField] float timeBeforeCanCommentAgain = 20;
    [SerializeField] float timeBeforeCheerReset = 2;
    float timerResetComment = 60;
    float timerResetCheer = 0;

    private void Update()
    {
        desactivateUiBonusDisplay = !(Weapon.Instance == null || !Weapon.Instance.IsMinigun);
        for (int i = textDisplayed.Count - 1; i > -1; i--)
        {
            scoresBonusHandler[i].UpdateValues(Main.Instance.GetCursorPos());
            if (i < textDisplayed.Count && scoresBonusHandler[i] != null)
            {
                textDisplayed[i].color = scoresBonusHandler[i].currentColor;
                textDisplayed[i].transform.localScale = Vector3.one * scoresBonusHandler[i].scale;
                if (scoresBonusHandler[i].isPlacedOnWorld) MoveSprite(textDisplayed[i].gameObject, scoresBonusHandler[i]);
            }
        }

        if (timerResetComment > 0)
        {
            timerResetComment -= Time.unscaledDeltaTime;
            if (timerResetComment < 0)
            {
                timerResetComment = 0;
                canComment = true;
            }
        }

        if (timerResetCheer > 0)
        {
            timerResetCheer -= Time.unscaledDeltaTime;
            if (timerResetCheer < 0)
            {
                timerResetCheer = 0;
                currCheer = 0;
            }
        }
    }

    public void BonusAcquired()
    {
        if (!desactivateUiBonusDisplay)
            thumbsUpVFX.Play();
    }
    public void MalusAcquired()
    {
        if(!desactivateUiBonusDisplay)
            thumbsDownVFX.Play();
    }


    public void AddScoreBonus(string textSend, bool good)
    {
        if (!desactivateUiBonusDisplay)
        {
            ScoreBonusDisplayedInstance handler;
            GameObject newText = createObject(textSend, good, out handler);
            Vector2 pos;
            Vector2 posInput = new Vector2(Screen.width * (0.5f + Random.Range(-dataToSend.randomPos, dataToSend.randomPos)), Screen.height * (0.5f + Random.Range(-dataToSend.randomPos, dataToSend.randomPos)));
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, posInput, GetComponent<Canvas>().worldCamera, out pos);
            newText.transform.position = transform.TransformPoint(pos);
            MaybePlayCheer();
        }
    }


    public void AddScoreBonus(string textSend, bool good, Vector3 posInit, float randomPosAdded = 0)
    {
        if (!desactivateUiBonusDisplay)
        {
            ScoreBonusDisplayedInstance handler;
            GameObject newText = createObject(textSend, good, out handler);
            handler.IsPlacedInWorld(true, posInit + new Vector3(Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded)));
            MoveSprite(newText, handler);
            MaybePlayCheer();
        }
    }

    public void MaybePlayCheer()
    {
        if (Random.Range(0f, 100f) < 30f)
        {
            /*if (Random.Range(0f, 100f) < 50)
                CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Crowd_Cheer", false, 0.3f, 0.1f);
            else
                CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Crowd_Cheer2", false, 0.3f, 0.1f);*/
            if (Random.Range(0f, 100f) < 50)
                CustomSoundManager.Instance.PlaySound("Crowd_Cheer", "PublicAmbiant", null, 0.6f, false, 1, 0.1f);
            else
                CustomSoundManager.Instance.PlaySound("Crowd_Cheer2", "PublicAmbiant", null, 0.6f, false, 1, 0.1f);

            if (currCheer == 0)
            {
                timerResetCheer = timeBeforeCheerReset;
            }

             currCheer++;

            if (currCheer >= neededCheer && canComment)
            {
                canComment = false;
                timerResetComment = timeBeforeCanCommentAgain;
                if (Main.Instance.EnableComments)
                {
                    if (Random.Range(0f, 100f) < 50)
                    {
                        Main.Instance.PlaySoundWithDelay("PresA_Belle_Action_A", "Comment", Main.Instance.CommentAVolume, 0);
                        SubtitleManager.Instance.SetSubtitle("What a play !", 0, 4, 0);
                    }
                    else
                    {
                        Main.Instance.PlaySoundWithDelay("PresA_Belle_Action_B", "Comment", Main.Instance.CommentBVolume, 0);
                        SubtitleManager.Instance.SetSubtitle("Wow ! Did you see that !", 0, 4, 0);
                    }
                    if (Random.Range(0f, 100f) < 50)
                    {
                        Main.Instance.PlaySoundWithDelay("PresB_Belle_Action_A", "Comment", Main.Instance.CommentAVolume, 1);
                        SubtitleManager.Instance.SetSubtitle("That was impressive !", 1, 3, 1);
                    }
                    else
                    {
                        Main.Instance.PlaySoundWithDelay("PresB_Belle_Action_B", "Comment", Main.Instance.CommentBVolume, 1.5f);
                        SubtitleManager.Instance.SetSubtitle("That was worhty of Death Live !", 1, 4, 1.5f);
                    }
                }
            }
        }
    }


    public void AddScoreBonus(string textSend, bool good, Vector3 posInit, Color SpecificColor, float randomPosAdded = 0)
    {
        if (!desactivateUiBonusDisplay)
        {
            ScoreBonusDisplayedInstance handler;
            GameObject newText = createObject(textSend, good, SpecificColor, out handler);
            handler.IsPlacedInWorld(true, posInit + new Vector3(Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded), Random.Range(-randomPosAdded, randomPosAdded)));
            MoveSprite(newText, handler);
        }
    }

    void MoveSprite(GameObject textObject, ScoreBonusDisplayedInstance handler)
    {
        Vector2 pos;
        Vector3 posScreen = CameraHandler.Instance.GetCurrentCam().WorldToScreenPoint(handler.posSave);
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

    GameObject createObject(string textSend, bool good, out ScoreBonusDisplayedInstance newOne)
    {
        GameObject newText = Instantiate(emptyUiText, rootScoreBonus.transform);
        Text textThis = newText.GetComponent<Text>();
        textThis.text = textSend;
        textThis.color = good ? dataToSend.colorGood : dataToSend.colorBad;
        textThis.fontSize = Mathf.RoundToInt(dataToSend.fontSize);

        newText.transform.localScale = Vector3.zero;
        Outline stockoutline = newText.AddComponent<Outline>();
        stockoutline.effectDistance = new Vector2(-1, 1) * dataToSend.outlineDistance;
        stockoutline.effectColor = dataToSend.colorOutline;

        textDisplayed.Add(textThis);

        newOne = new ScoreBonusDisplayedInstance();
        newOne.OnCreation(dataToSend, textThis.color, textThis, newText, newText.GetComponent<RectTransform>());
        scoresBonusHandler.Add(newOne);
        return newText;
    }
    GameObject createObject(string textSend, bool good, Color specificColor, out ScoreBonusDisplayedInstance newOne)
    {
        GameObject newText = Instantiate(emptyUiText, rootScoreBonus.transform);
        Text textThis = newText.GetComponent<Text>();
        textThis.text = textSend;
        textThis.color = good ? dataToSend.colorGood : dataToSend.colorBad;
        textThis.fontSize = Mathf.RoundToInt(dataToSend.fontSize);

        newText.transform.localScale = Vector3.zero;
        Outline stockoutline = newText.AddComponent<Outline>();
        stockoutline.effectDistance = new Vector2(-1, 1) * dataToSend.outlineDistance;
        stockoutline.effectColor = dataToSend.colorOutline;

        textDisplayed.Add(textThis);

        newOne = new ScoreBonusDisplayedInstance();
        newOne.OnCreation(dataToSend, specificColor, textThis, newText, newText.GetComponent<RectTransform>());
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
                GameObject stock = textDisplayed[i].gameObject;
                textDisplayed.RemoveAt(i);
                Destroy(stock);
            }
        }
    }

}

