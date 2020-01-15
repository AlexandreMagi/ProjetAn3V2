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

    [SerializeField]
    Slider lifeBar = null;
    [SerializeField]
    Slider lifeBarFeedBack = null;
    [SerializeField]
    Slider armorBar = null;
    [SerializeField]
    Slider armorBarFeedBack = null;
    [SerializeField]
    Text lifeText = null;
    [SerializeField]
    Text armorText = null;

    public void UpdateLifeDisplay(float value, float realValue)
    {
        lifeBar.value = value;
        lifeText.text = Mathf.CeilToInt(realValue).ToString();
    }

    public void UpdateArmorDisplay(float value, float realValue)
    {
        armorBar.value = value;
        armorText.text = Mathf.CeilToInt(realValue).ToString();
    }

    public void Update()
    {
        if  (lifeBarFeedBack.value < lifeBar.value)     lifeBarFeedBack.value = lifeBar.value;
        else lifeBarFeedBack.value =                    Mathf.MoveTowards(lifeBarFeedBack.value, lifeBar.value, Time.deltaTime);
        if  (armorBarFeedBack.value < armorBar.value)   armorBarFeedBack.value = armorBar.value;
        else armorBarFeedBack.value =                   Mathf.MoveTowards(armorBarFeedBack.value, armorBar.value, Time.deltaTime);
    }

}
