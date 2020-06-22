using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EasterEggButton : MonoBehaviour
{

    [SerializeField] RectTransform rect = null;
    [SerializeField] EasterEggHandler.SpecialBonusType easterEggType = EasterEggHandler.SpecialBonusType.fanfaron;
    [SerializeField] CanvasGroup activatedCanvasGroup = null;
    [SerializeField] CanvasGroup desactivatedCanvasGroup = null;

    void Start()
    {
        if(EasterEggHandler.Instance != null)
        {
            bool enabled = false;
            switch (easterEggType)
            {
                case EasterEggHandler.SpecialBonusType.juggernaut:
                    gameObject.SetActive(EasterEggHandler.Instance.JuggernautUnlocked);
                    enabled = EasterEggHandler.Instance.JuggernautEnabled;
                    break;
                case EasterEggHandler.SpecialBonusType.aikant:
                    gameObject.SetActive(EasterEggHandler.Instance.AikantUnlocked);
                    enabled = EasterEggHandler.Instance.AikantEnabled;
                    break;
                case EasterEggHandler.SpecialBonusType.fanfaron:
                    gameObject.SetActive(EasterEggHandler.Instance.FanfaronUnlocked);
                    enabled = EasterEggHandler.Instance.FanfaronEnabled;
                    break;
                default:
                    break;
            }
            activatedCanvasGroup.alpha = enabled ? 1 : 0;
            desactivatedCanvasGroup.alpha = enabled ? 0 : 1;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public bool CheckIfMouseOver()
    {
        Vector2 mousePosition = MenuMain.Instance.GetCursorPos();

        if (gameObject.activeSelf && rect != null)
        {
            float distX = rect.sizeDelta.x / 2 * transform.localScale.x;
            float distY = rect.sizeDelta.y / 2 * transform.localScale.y;
            if (mousePosition.x < rect.position.x + distX && mousePosition.x > rect.position.x - distX && mousePosition.y < rect.position.y + distY && mousePosition.y > rect.position.y - distY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public bool Click()
    {
        if (CheckIfMouseOver())
        {
            if (EasterEggHandler.Instance != null)
            {
                bool enabled = false;
                switch (easterEggType)
                {
                    case EasterEggHandler.SpecialBonusType.juggernaut:
                        EasterEggHandler.Instance.JuggernautEnabled = !EasterEggHandler.Instance.JuggernautEnabled;
                        enabled = EasterEggHandler.Instance.JuggernautEnabled;
                        break;
                    case EasterEggHandler.SpecialBonusType.aikant:
                        EasterEggHandler.Instance.AikantEnabled = !EasterEggHandler.Instance.AikantEnabled;
                        enabled = EasterEggHandler.Instance.AikantEnabled;
                        break;
                    case EasterEggHandler.SpecialBonusType.fanfaron:
                        EasterEggHandler.Instance.FanfaronEnabled = !EasterEggHandler.Instance.FanfaronEnabled;
                        enabled = EasterEggHandler.Instance.FanfaronEnabled;
                        break;
                    default:
                        break;
                }
                activatedCanvasGroup.alpha = enabled ? 1 : 0;
                desactivatedCanvasGroup.alpha = enabled ? 0 : 1;
            }
            return true;
        }
        return false;
    }

}
