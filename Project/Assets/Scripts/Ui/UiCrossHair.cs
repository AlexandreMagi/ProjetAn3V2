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
        _instance = this;
    }

    #endregion

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

    [SerializeField]
    Transform rootCrosshair = null;
    [SerializeField]
    GameObject waitGameObject = null;
    private void Start()
    {
        UiCrosshairs = new GameObject[crosshairs.Length];
        dataHandlerCrosshairs = new CrosshairInstance[crosshairs.Length];
        for (int i = 0; i < crosshairs.Length; i++)
        {
            UiCrosshairs[i] = Instantiate(baseForCrosshair, rootCrosshair.transform);
            UiCrosshairs[i].GetComponent<Image>().sprite = crosshairs[i];
            dataHandlerCrosshairs[i] = new CrosshairInstance(dataCrosshairs[i], UiCrosshairs[i].GetComponent<RectTransform>(), UiCrosshairs[i].GetComponent<Image>(), UiCrosshairs[i].GetComponent<Outline>());
        }
        animCharge = Animator.StringToHash(animTriggerCharge);
        animRelease = Animator.StringToHash(animTriggerRelease);
        animUICrossHair = fxUICrossHair.GetComponent<Animator>();
    }

    public void UpdateCrossHair(Vector2 mousePosition)
    {

        for (int i = 0; i < UiCrosshairs.Length; i++)
        {
            dataHandlerCrosshairs[i].UpdateValues();
            dataHandlerCrosshairs[i].UpdateTransformsAndColors();

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePosition + dataHandlerCrosshairs[i].offset, this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
            UiCrosshairs[i].transform.position = transform.TransformPoint(pos);

            if (Weapon.Instance)
            UiCrosshairs[i].SetActive(//Weapon.Instance.GetBulletAmmount().x > 0 && 
                !Weapon.Instance.GetIfReloading());
        }
        Vector2 posCrFx;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, mousePosition, this.gameObject.GetComponent<Canvas>().worldCamera, out posCrFx);
        fxUICrossHair.transform.position = transform.TransformPoint(posCrFx);
    }

    public void WaitFunction()
    {
        waitGameObject.SetActive(true);
        rootCrosshair.gameObject.SetActive(false);
        if (waitGameObject.activeSelf)
            waitGameObject.GetComponent<Animator>().SetTrigger("pop");
        UINewWait.Instance.TriggerWait();
    }

    public void StopWaitFunction()
    {
        if (waitGameObject.activeSelf)
            waitGameObject.GetComponent<Animator>().SetTrigger("depop");
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
    }

    public void PlayerShot(float value, bool chargedShot)
    {
        if (chargedShot) animUICrossHair.SetTrigger(animRelease);

        for (int i = 0; i < dataHandlerCrosshairs.Length; i++)
        {
            dataHandlerCrosshairs[i].PlayerShot(value);
        }
    }

    public void JustFinishedCharging() { animUICrossHair.SetTrigger(animCharge); }


}
