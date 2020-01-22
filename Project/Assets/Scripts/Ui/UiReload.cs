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

    float timerShot = 1;

    float currentRemainingTextScale = 1;

    GameObject[] bulletSprites = new GameObject[0];

    float holaValue = 0;

    private void Start()
    {
        bar             = Instantiate(emptyUiBox, rootUiReloading.transform);
        extremityOne    = Instantiate(emptyUiBox, rootUiReloading.transform);
        extremityTwo    = Instantiate(emptyUiBox, rootUiReloading.transform);
        perfectSpot     = Instantiate(emptyUiBox, rootUiReloading.transform);
        checkBar        = Instantiate(emptyUiBox, rootUiReloading.transform);
        HideGraphics(false);

        bulletSprites = new GameObject[reloadData.pullOfBullet];
        int bulletCost = Weapon.Instance.GetChargedWeaponBulletCost();
        int numberOfSeparation = Mathf.CeilToInt(reloadData.pullOfBullet / bulletCost) - 1;
        float distanceBetweenBullet = (reloadData.purcentageUsedY - reloadData.spaceEveryThreeBullet * numberOfSeparation) * Screen.height / reloadData.pullOfBullet;


        for (int i = 0; i < reloadData.pullOfBullet; i++)
        {
            bulletSprites[i] = Instantiate(emptyUiBox, transform);
            bulletSprites[i].GetComponent<Image>().sprite = reloadData.bulletSprite;

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, new Vector2 (0, i * distanceBetweenBullet + reloadData.spaceEveryThreeBullet * Screen.height * Mathf.CeilToInt(i / bulletCost)) + new Vector2 (Screen.width * reloadData.decalBulletSprites.x, Screen.height * reloadData.decalBulletSprites.y), this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
            bulletSprites[i].transform.position = transform.TransformPoint(pos);
        }

    }

    void Update()
    {

        // ######################################################################################################################## //
        // ################################################## BULLET DISPLAY ###################################################### //
        // ######################################################################################################################## //

        #region bulletDisplay
        Vector2 bulletAmount = Weapon.Instance.GetBulletAmmount();

        reloadText.SetActive(bulletAmount.x == 0);

        if (timerShot < 1)
            timerShot += Time.unscaledDeltaTime / reloadData.scaleAnimBulletTime;
        if (timerShot > 1)
            timerShot = 1;

        bulletRemainingText.color = bulletAmount.x == 0 ? reloadData.noBulletColor : bulletAmount.x > reloadData.shortNumberOfBullet ? reloadData.highOnBulletColor : reloadData.shortOnBulletColor;

        if (holaValue < reloadData.pullOfBullet -1 + reloadData.holaRange) holaValue += Time.unscaledDeltaTime * (reloadData.pullOfBullet - 1 + reloadData.holaRange) / reloadData.holaFeedbackTime;

        float aimedSize;
        for (int i = 0; i < reloadData.pullOfBullet; i++)
        {
            if (Mathf.Abs(i- holaValue) < reloadData.holaRange)
            {
                bulletSprites[i].transform.localScale = Vector3.one + Vector3.one * reloadData.holaEffectOnBullet.Evaluate((holaValue - i + reloadData.holaRange)/ reloadData.holaRange) * reloadData.holaScaleMultiplier;
            }
            else
            {
                bulletSprites[i].transform.localScale = Vector3.one;
            }

            if (i < bulletAmount.x)
            {
                bulletSprites[i].SetActive(true);
                if (i + 1 == bulletAmount.x) aimedSize = reloadData.selectedSize;
                else aimedSize = reloadData.baseSize;

                bulletSprites[i].GetComponent<Image>().color = bulletAmount.x == 0 ? reloadData.noBulletColor : bulletAmount.x > reloadData.shortNumberOfBullet ? reloadData.highOnBulletColor : reloadData.shortOnBulletColor;
                if (i > bulletAmount.y-1) bulletSprites[i].GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                bulletSprites[i].SetActive(false);
                aimedSize = reloadData.shotSize;
            }
            bulletSprites[i].GetComponent<RectTransform>().sizeDelta = Vector2.Lerp (bulletSprites[i].GetComponent<RectTransform>().sizeDelta, Vector2.one * aimedSize, Time.unscaledDeltaTime * reloadData.sizeChangeSpeed);
        }

        if (bulletAmount.x > 0) currentRemainingTextScale = Mathf.MoveTowards(currentRemainingTextScale, 1, reloadData.scaleRecoverSpeed * Time.unscaledDeltaTime);
        else currentRemainingTextScale = Mathf.MoveTowards(currentRemainingTextScale, reloadData.scaleIfNoBullet, reloadData.scaleEmptySpeed * Time.unscaledDeltaTime);

        bulletRemainingText.transform.localScale = Vector3.one * currentRemainingTextScale + Vector3.one * currentRemainingTextScale * reloadData.scaleAnimBulletTextShot.Evaluate(timerShot) * reloadData.scaleAnimBulletValue ;

        bulletRemainingText.text = bulletAmount.x + " / " + bulletAmount.y;
        #endregion

        // ######################################################################################################################## //
        // ################################################## RELOAD DISPLAY ###################################################### //
        // ######################################################################################################################## //

        #region reloadDisplay
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
        #endregion

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

        holaValue = -reloadData.holaRange;
        //rootUiReloading.SetActive(false);
    }

    public void PlayerShot()
    {
        timerShot = 0;
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
