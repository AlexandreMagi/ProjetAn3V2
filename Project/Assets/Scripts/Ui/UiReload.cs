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

    GameObject bar = null;
    [SerializeField] Color barColor = Color.white;
    [SerializeField] Color extremityColor = Color.white;
    [SerializeField] Color checkBarColor = Color.green;
    [SerializeField] Color checkBarColorFailed = Color.red;
    [SerializeField] Color perfectSpotColor = Color.blue;
    GameObject extremityOne = null;
    GameObject extremityTwo = null;
    GameObject checkBar = null;
    GameObject perfectSpot = null;

    [SerializeField] Text bulletRemainingText = null;


    private void Start()
    {
        bar             = Instantiate(emptyUiBox, rootUiReloading.transform);
        extremityOne    = Instantiate(emptyUiBox, rootUiReloading.transform);
        extremityTwo    = Instantiate(emptyUiBox, rootUiReloading.transform);
        perfectSpot     = Instantiate(emptyUiBox, rootUiReloading.transform);
        checkBar        = Instantiate(emptyUiBox, rootUiReloading.transform);
        HideGraphics();
    }

    void Update()
    {
        Vector2 bulletAmount = Weapon.Instance.GetBulletAmmount();
        bulletRemainingText.text = bulletAmount.x + " / " + bulletAmount.y;
        //UpdateGraphics(Mathf.Sin(Time.time) / 2 + 0.5f, 0.7f, 0.05f);

    }

    public void HideGraphics()
    {
        //bar.SetActive(false);
        //extremityOne.SetActive(false);
        //extremityTwo.SetActive(false);
        //checkBar.SetActive(false);
        //perfectSpot.SetActive(false);
        rootUiReloading.SetActive(false);
    }

    public void DisplayGraphics()
    {
        //bar.SetActive(true);
        //extremityOne.SetActive(true);
        //extremityTwo.SetActive(true);
        //checkBar.SetActive(true);
        //perfectSpot.SetActive(true);
        rootUiReloading.SetActive(true);
        perfectSpot.SetActive(true);
    }

    public void UpdateGraphics(float currentLoading, float perfectPlacement, float perfectRange, bool hidePerfect)
    {
        if (hidePerfect)
            perfectSpot.SetActive(false);

        bar.GetComponent<RectTransform>().sizeDelta = barSize;
        extremityOne.GetComponent<RectTransform>().sizeDelta = extremitySize;
        extremityTwo.GetComponent<RectTransform>().sizeDelta = extremitySize;
        checkBar.GetComponent<RectTransform>().sizeDelta = checkBarSize;
        perfectSpot.GetComponent<RectTransform>().sizeDelta = new Vector2(barSize.x * perfectRange, perfectRangeHeight);

        float barSizeX = bar.GetComponent<RectTransform>().sizeDelta.x;

        MoveTo(extremityOne, bar.GetComponent<RectTransform>().position + new Vector3(barSizeX / 2, 0));
        MoveTo(extremityTwo, bar.GetComponent<RectTransform>().position - new Vector3(barSizeX / 2, 0));
        MoveTo(checkBar, bar.GetComponent<RectTransform>().position - new Vector3(barSizeX / 2, 0) + new Vector3(barSizeX, 0) * currentLoading);
        MoveTo(perfectSpot, bar.GetComponent<RectTransform>().position - new Vector3(barSizeX / 2, 0) + new Vector3(barSizeX, 0) * perfectPlacement);

        ChangeColor(bar, barColor);
        ChangeColor(extremityOne, extremityColor);
        ChangeColor(extremityTwo, extremityColor);
        ChangeColor(checkBar, hidePerfect? checkBarColorFailed: checkBarColor);
        ChangeColor(perfectSpot, perfectSpotColor);
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
