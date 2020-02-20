using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDouille
{
    public Vector2 pSVelocity = new Vector2(-400, 400);
    public float pSGravity = 1500;
    public float pSRotation = 0;
    public float pSSize = 100;
    public Color pSColor = Color.Lerp(Color.white, Color.black, 0.7f);
    float pSRotate = 180;

    RectTransform rect = null;
    Image img = null;

    public void InitValues(DataReloadGraph data, RectTransform _rect, Image _img)
    {
        pSGravity = Random.Range(data.gravityRandom.x, data.gravityRandom.y);
        pSRotate = Random.Range(data.rotateRandom.x, data.rotateRandom.y);
        pSVelocity = new Vector2(Random.Range(data.velocityXRandom.x, data.velocityXRandom.y), Random.Range(data.velocityYRandom.x, data.velocityYRandom.y));
        pSSize = Random.Range(data.sizeRandom.x, data.sizeRandom.y);
        img = _img;
        rect = _rect;
    }

    public UiDouille(DataReloadGraph data, RectTransform _rect, Image _img)
    {
        InitValues(data, _rect, _img);
    }

    public void UpdateValues()
    {
        pSRotation += pSRotate * Time.unscaledDeltaTime;
        if (pSRotation > 360)
            pSRotation -= 360;
        if (pSRotation < -360)
            pSRotation += 360;
        pSVelocity.y -= pSGravity * Time.unscaledDeltaTime;
    }
}
