using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UiReload : MonoBehaviour
{
    #region singleton

    private static UiReload _instance;

    public static UiReload Instance
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

    Vector2 barSize = new Vector2(1300, 20);
    Vector2 extremitySize = new Vector2(25, 100);
    Vector2 checkBarSize = new Vector2(25, 100);
    float perfectRangeHeight = 50;

    [SerializeField] GameObject emptyUiBox = null;
    [SerializeField] GameObject rootUiReloading = null;
    [SerializeField] DataReloadGraph reloadData = null;

    GameObject bar = null;
    GameObject extremityOne = null;
    GameObject extremityTwo = null;
    GameObject checkBar = null;
    GameObject perfectSpot = null;
    [SerializeField] GameObject reloadingText = null;

    [SerializeField] Text bulletRemainingText = null;
    [SerializeField] GameObject reloadText = null;

    float reducingPurcentage = 0;
    bool reducing = false;
    float perfectAnimPurcentage = 0;
    bool perfectAnim = false;

    private void Start()
    {
        bar             = Instantiate(emptyUiBox, rootUiReloading.transform);
        extremityOne    = Instantiate(emptyUiBox, rootUiReloading.transform);
        extremityTwo    = Instantiate(emptyUiBox, rootUiReloading.transform);
        perfectSpot     = Instantiate(emptyUiBox, rootUiReloading.transform);
        checkBar        = Instantiate(emptyUiBox, rootUiReloading.transform);
        HideGraphics(false);
    }

    void Update()
    {
        Vector2 bulletAmount = Weapon.Instance.GetBulletAmmount();

        reloadText.SetActive(bulletAmount.x == 0);

        bulletRemainingText.text = bulletAmount.x + " / " + bulletAmount.y;
        //UpdateGraphics(Mathf.Sin(Time.time) / 2 + 0.5f, 0.7f, 0.05f);

        if (reducing)
        {
            reducingPurcentage += Time.unscaledDeltaTime / reloadData.reducingTime;
            if (reducingPurcentage > 1)
            {
                reducingPurcentage = 1;
                reducing = false;
                bar.SetActive(false);
                extremityOne.SetActive(false);
                extremityTwo.SetActive(false);
                checkBar.SetActive(false);
                reloadingText.SetActive(false);
                //rootUiReloading.SetActive(false);
            }
        }
        if (perfectAnim)
        {
            perfectAnimPurcentage += Time.unscaledDeltaTime / reloadData.perfectAnimtime;
            if (perfectAnimPurcentage > 1)
            {
                perfectAnimPurcentage = 0;
                perfectAnim = false;
            }
        }

        float totalScaleValue = Mathf.Sin(Time.unscaledTime * reloadData.idleSpeed) * reloadData.idleMagnitude;
        float baseScale = (1 - reducingPurcentage);
        ChangeScale(bar, totalScaleValue, baseScale);
        ChangeScale(reloadText, totalScaleValue, 1);
        ChangeScale(extremityOne, totalScaleValue, baseScale);
        ChangeScale(extremityTwo, totalScaleValue, baseScale);
        ChangeScale(checkBar, totalScaleValue, baseScale);
        ChangeScale(reloadingText, totalScaleValue, baseScale);
        ChangeScale(perfectSpot, totalScaleValue, baseScale + reloadData.scaleAnimOnPerfectIndicator.Evaluate(perfectAnimPurcentage) * reloadData.perfectAnimScaleMultiplier);

    }

    void ChangeScale (GameObject obj, float scale, float basescale)
    {
        obj.transform.localScale = Vector3.one * (scale + basescale);
    }

    public void HideGraphics(bool didPerfect)
    {
        //bar.SetActive(false);
        //extremityOne.SetActive(false);
        //extremityTwo.SetActive(false);
        //checkBar.SetActive(false);
        //perfectSpot.SetActive(false);

        reducing = true;
        perfectAnim = didPerfect;

        //rootUiReloading.SetActive(false);
    }

    public void DisplayGraphics()
    {
        bar.SetActive(true);
        extremityOne.SetActive(true);
        extremityTwo.SetActive(true);
        checkBar.SetActive(true);
        //perfectSpot.SetActive(true);
        //rootUiReloading.SetActive(true);
        perfectSpot.SetActive(true);
        reloadingText.SetActive(true);
        reducingPurcentage = 0;
    }

    public void UpdateGraphics(float currentLoading, float perfectPlacement, float perfectRange, bool hidePerfect)
    {
        if (hidePerfect && !perfectAnim)
            perfectSpot.SetActive(false);

        Vector2 scaleValue = new Vector2(reloadData.horizontalScaleAnimAatSpawn.Evaluate(currentLoading / reloadData.animDuration), reloadData.verticalScaleAnimAatSpawn.Evaluate(currentLoading / reloadData.animDuration));

        bar.GetComponent<RectTransform>().sizeDelta = barSize * scaleValue;
        extremityOne.GetComponent<RectTransform>().sizeDelta = extremitySize * scaleValue;
        extremityTwo.GetComponent<RectTransform>().sizeDelta = extremitySize * scaleValue;
        checkBar.GetComponent<RectTransform>().sizeDelta = checkBarSize * scaleValue;
        perfectSpot.GetComponent<RectTransform>().sizeDelta = new Vector2(barSize.x * perfectRange, perfectRangeHeight) * scaleValue;

        float barSizeX = bar.GetComponent<RectTransform>().sizeDelta.x * bar.transform.localScale.x;

        MoveTo(extremityOne, bar.GetComponent<RectTransform>().position + new Vector3(barSizeX / 2, 0));
        MoveTo(extremityTwo, bar.GetComponent<RectTransform>().position - new Vector3(barSizeX / 2, 0));
        MoveTo(checkBar, bar.GetComponent<RectTransform>().position - new Vector3(barSizeX / 2, 0) + new Vector3(barSizeX, 0) * currentLoading);
        MoveTo(perfectSpot, bar.GetComponent<RectTransform>().position - new Vector3(barSizeX / 2, 0) + new Vector3(barSizeX, 0) * perfectPlacement);

        ChangeColor(bar, reloadData.barColor);
        ChangeColor(extremityOne, reloadData.extremityColor);
        ChangeColor(extremityTwo, reloadData.extremityColor);
        ChangeColor(checkBar, hidePerfect? reloadData.checkBarColorFailed : reloadData.checkBarColor);
        ChangeColor(perfectSpot, reloadData.perfectSpotColor);


    }

    void MoveTo(GameObject obj, Vector3 posInScreen)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, posInScreen, this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
        obj.transform.position = transform.TransformPoint(pos);
    }

    void ChangeColor (GameObject obj , Color mainColor)
    {
        obj.GetComponent<Image>().color = mainColor;
    }

}
