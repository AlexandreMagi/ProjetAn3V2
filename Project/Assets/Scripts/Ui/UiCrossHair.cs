using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiCrossHair : MonoBehaviour
{
    #region singleton

    private static UiCrossHair _instance;

    public static UiCrossHair Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        if (beInstance)
            _instance = this;
    }

    public void SetAsInstance() { _instance = this; }

    #endregion

    [SerializeField] bool beInstance = true;
    [SerializeField] bool unlockAllAtStart = false;
    [SerializeField] Canvas cvs = null;

    [SerializeField]
    GameObject baseForCrosshair = null;
    [SerializeField]
    Sprite[] crosshairs = new Sprite[0];

    [SerializeField]
    GameObject fxUICrossHair = null;
    Animator animUICrossHair = null;
    [SerializeField]
    string animTriggerCharge = "";
    [SerializeField]
    string animTriggerRelease = "";

    int animCharge;
    int animRelease;

    [SerializeField]
    DataCrossHair[] dataCrosshairs = new DataCrossHair[0];

    CrosshairInstance[] dataHandlerCrosshairs = new CrosshairInstance[0];
    GameObject[] UiCrosshairs = new GameObject[0];

    RectTransform UiHitMarker = null;
    [SerializeField]
    Sprite hitMarkerSprite = null;
    [SerializeField]
    AnimationCurve hitMarkerPop = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    float hitMarkerTimeAnim = 0.2f;
    [SerializeField]
    float hitMarkerMultiplierAnim = 100;
    float hitMarkerAnimPurcentage = 1;

    [SerializeField]
    Image crossHairVignetage = null;

    [SerializeField]
    Transform rootCrosshair = null;
    [SerializeField]
    GameObject waitGameObject = null;

    [SerializeField] Color singleShotColor = Color.black;
    [SerializeField] Color chargedShotColor = Color.black;

    private void Start()
    {
        UiCrosshairs = new GameObject[crosshairs.Length];
        dataHandlerCrosshairs = new CrosshairInstance[crosshairs.Length];
        for (int i = 0; i < crosshairs.Length; i++)
        {
            UiCrosshairs[i] = Instantiate(baseForCrosshair, rootCrosshair.transform);
            UiCrosshairs[i].GetComponent<Image>().sprite = crosshairs[i];
            dataHandlerCrosshairs[i] = new CrosshairInstance(dataCrosshairs[i], UiCrosshairs[i].GetComponent<RectTransform>(), UiCrosshairs[i].GetComponent<Image>(), UiCrosshairs[i].GetComponent<Outline>());
            if (dataCrosshairs[i].crosshairPopsWhen == DataCrossHair.activatedIf.start) dataHandlerCrosshairs[i].unlocked = true;
        }
        animCharge = Animator.StringToHash(animTriggerCharge);
        animRelease = Animator.StringToHash(animTriggerRelease);
        animUICrossHair = fxUICrossHair.GetComponent<Animator>();

        if (UiHitMarker != null)
        {
            UiHitMarker = Instantiate(baseForCrosshair, rootCrosshair.transform).GetComponent<RectTransform>();
            UiHitMarker.GetComponent<Image>().sprite = hitMarkerSprite;
            UiHitMarker.sizeDelta = Vector2.zero;
        }

        StartCoroutine(doUnlock());

    }

    IEnumerator doUnlock()
    {
        yield return new WaitForSecondsRealtime(.2f);

        if (unlockAllAtStart) UnlockAllCrosshair();
        else UpdateCursorUnlocks();

        yield break;
    }

    public void UpdateCrossHair(Vector2 mousePosition)
    {

        Vector2 pos = Vector2.zero;
        for (int i = 0; i < UiCrosshairs.Length; i++)
        {
            dataHandlerCrosshairs[i].UpdateValues();
            dataHandlerCrosshairs[i].UpdateTransformsAndColors();

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePosition + dataHandlerCrosshairs[i].offset, cvs.worldCamera, out pos);
            UiCrosshairs[i].transform.position = transform.TransformPoint(pos);

            /*if (Weapon.Instance)
            UiCrosshairs[i].SetActive(//Weapon.Instance.GetBulletAmmount().x > 0 && 
                !Weapon.Instance.GetIfReloading());*/
        }

        if (UiHitMarker != null)
        {
            if (hitMarkerAnimPurcentage < 1 && UiHitMarker != null)
            {
                UiHitMarker.sizeDelta = Vector2.one * hitMarkerPop.Evaluate(hitMarkerAnimPurcentage) * hitMarkerMultiplierAnim;
                hitMarkerAnimPurcentage += Time.unscaledDeltaTime / hitMarkerTimeAnim;
                UiHitMarker.transform.position = transform.TransformPoint(pos);
                if (hitMarkerAnimPurcentage > 1)
                {
                    hitMarkerAnimPurcentage = 1;
                    UiHitMarker.sizeDelta = Vector2.zero;
                }
            }
        }
            


        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePosition, cvs.worldCamera, out pos);
        fxUICrossHair.transform.position = transform.TransformPoint(pos);
        if (crossHairVignetage != null)
        {
            crossHairVignetage.gameObject.transform.position = transform.TransformPoint(pos);
            if (Main.Instance == null || Main.Instance.TCActivated)
                crossHairVignetage.color = Color.Lerp(singleShotColor, chargedShotColor, Weapon.Instance.GetChargeValue());
            else
                crossHairVignetage.color = new Color(0, 0, 0, 0);
        }

    }

    void UnlockAllCrosshair()
    {
        for (int i = 0; i < dataHandlerCrosshairs.Length; i++) { dataHandlerCrosshairs[i].unlocked = true; }
    }

    public void UpdateCursorUnlocks()
    {
        if (Main.Instance != null && !unlockAllAtStart)
        {
            for (int i = 0; i < dataCrosshairs.Length; i++)
            {
                switch (dataCrosshairs[i].crosshairPopsWhen)
                {
                    case DataCrossHair.activatedIf.start:
                        dataHandlerCrosshairs[i].unlocked = true;
                        break;
                    case DataCrossHair.activatedIf.reload:
                        if (Main.Instance.playerCanReload) dataHandlerCrosshairs[i].unlocked = true;
                        break;
                    case DataCrossHair.activatedIf.reloadPerfect:
                        if (Main.Instance.playerCanPerfectReload) dataHandlerCrosshairs[i].unlocked = true;
                        break;
                    case DataCrossHair.activatedIf.shotgun:
                        if (Main.Instance.playerCanShotgun) dataHandlerCrosshairs[i].unlocked = true;
                        break;
                    case DataCrossHair.activatedIf.orb:
                        if (Main.Instance.playerCanOrb) dataHandlerCrosshairs[i].unlocked = true;
                        break;
                    case DataCrossHair.activatedIf.zeroG:
                        if (Main.Instance.playerCanZeroG) dataHandlerCrosshairs[i].unlocked = true;
                        break;
                }
            }

        }
        else
            for (int i = 0; i < dataHandlerCrosshairs.Length; i++) { dataHandlerCrosshairs[i].unlocked = true; }
    }

    public void WaitFunction()
    {
        //waitGameObject.SetActive(true);
        rootCrosshair.gameObject.SetActive(false);
        //if (waitGameObject.activeSelf)
        //    waitGameObject.GetComponent<Animator>().SetTrigger("pop");
        UINewWait.Instance.TriggerWait();
    }

    public void StopWaitFunction()
    {
        //if (waitGameObject.activeSelf)
        //    waitGameObject.GetComponent<Animator>().SetTrigger("depop");
        //waitGameObject.SetActive(false);
        rootCrosshair.gameObject.SetActive(true);
        UINewWait.Instance.RemoveWait();
    }

    public void PlayerHasOrb(bool haveOrb)
    {
        for (int i = 0; i < dataHandlerCrosshairs.Length; i++)
        {
            dataHandlerCrosshairs[i].haveOrb = haveOrb;
        }
    }

    public void PlayerHitSomething(float value)
    {
        for (int i = 0; i < dataHandlerCrosshairs.Length; i++)
        {
            dataHandlerCrosshairs[i].PlayerHitSomething(value);
        }
        hitMarkerAnimPurcentage = 0;
    }

    public void PlayerShot(float value, bool chargedShot)
    {
        if (chargedShot && (Main.Instance == null || Main.Instance.TCActivated)) animUICrossHair.SetTrigger(animRelease);

        for (int i = 0; i < dataHandlerCrosshairs.Length; i++)
        {
            dataHandlerCrosshairs[i].PlayerShot(value);
        }
    }

    public void JustFinishedCharging() { if (Main.Instance == null || Main.Instance.TCActivated) animUICrossHair.SetTrigger(animCharge); }


}
