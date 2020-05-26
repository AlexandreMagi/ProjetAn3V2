using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighQualityButton : MonoBehaviour
{

    [SerializeField] RectTransform rect = null;

    [SerializeField] CanvasGroup cvsGroup = null;
    [SerializeField] float mouseOverAlpha = 1;
    [SerializeField] float mouseAwayAlpha = .1f;
    [SerializeField] float speedLerpAlpha = 8;

    [SerializeField] Color activatedColor = Color.blue;
    [SerializeField] Color unctivatedColor = Color.red;

    [SerializeField] string highQualityText = "High Quality";
    [SerializeField] string lowQualityText = "Low Quality";

    [SerializeField] Text qualityText = null;
    [SerializeField] Image buttonImage = null;

    bool isMouseOvered = false;

    bool HighQuality = true;

    void Start()
    {
        HighQuality = (QualityHandler.Instance == null || QualityHandler.Instance.isHighQuality);
    }

    void Update()
    {
        if (HighQuality)
        {
            qualityText.text = highQualityText;
            qualityText.color = activatedColor;
            buttonImage.color = activatedColor;
        }
        else
        {
            qualityText.text = lowQualityText;
            qualityText.color = unctivatedColor;
            buttonImage.color = unctivatedColor;
        }
        CheckIfMouseOver();
        if (cvsGroup != null) cvsGroup.alpha = Mathf.Lerp(cvsGroup.alpha, isMouseOvered ? mouseOverAlpha : mouseAwayAlpha, Time.unscaledDeltaTime * speedLerpAlpha);
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
                isMouseOvered = true;
                return true;
            }
            else
            {
                isMouseOvered = false;
                return false;
            }
        }
        return false;
    }

    public void Click()
    {
        if (CheckIfMouseOver())
        {
            HighQuality = !HighQuality;
        }
    }

    public void GoToGame()
    {
        if (QualityHandler.Instance != null)
        {
            QualityHandler.Instance.SetupQuality(HighQuality);
        }
    }

}
