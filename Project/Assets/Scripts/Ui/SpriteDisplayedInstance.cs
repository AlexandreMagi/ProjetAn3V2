using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteDisplayedInstance
{
    private Color baseColor = Color.red;
    private DataUiTemporarySprite data = null;
    public Color currentColor = Color.red;
    private float currentTimer = 0;
    public float scale = 0;
    private float currentAlpha = 1;
    public Image imageComponent = null;

    public void OnCreation(DataUiTemporarySprite _data, Image _imageComponent)
    {
        data = _data;
        baseColor = new Color(
            Random.Range(data.colorRandomOne.r, data.colorRandomTwo.r),
            Random.Range(data.colorRandomOne.g, data.colorRandomTwo.g),
            Random.Range(data.colorRandomOne.b, data.colorRandomTwo.b));
        currentColor = baseColor;
        imageComponent = _imageComponent;
    }

    public void UpdateValues()
    {
        scale = Mathf.MoveTowards(scale, 1, Time.unscaledDeltaTime / data.timeToScaleToMax);

        currentTimer += Time.unscaledDeltaTime;
        if (currentTimer > data.timeStayVisible)
        {
            currentAlpha -= Time.unscaledDeltaTime / data.timeToFade;
            if (currentAlpha < 0)
                UiDamageHandler.Instance.deleteSpot(this);
            else
                currentColor = new Color(baseColor.r, baseColor.g, baseColor.b, currentAlpha);
        }
    }

}
