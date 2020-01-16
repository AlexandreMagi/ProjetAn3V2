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
    }

    public void AddSprite (DataUiTemporarySprite dataSend)
    {
        GameObject newSprite = Instantiate(emptyUiBox, transform);
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

}
