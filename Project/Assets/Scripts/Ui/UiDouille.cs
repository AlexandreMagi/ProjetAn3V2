using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiDouille
{
    public Vector2 pSVelocity = new Vector2(-400, 400);
    public float pSGravity = 1500;
    public float pSRotation = 0;
    public float pSSize = 100;
    public Color pSColor = Color.Lerp(Color.white, Color.black, 0.7f);
    float pSRotate = 180;

    public void InitValues(DataReloadGraph data)
    {
        pSGravity = Random.Range(data.gravityRandom.x, data.gravityRandom.y);
        pSRotate = Random.Range(data.rotateRandom.x, data.rotateRandom.y);
        pSVelocity = new Vector2(Random.Range(data.velocityXRandom.x, data.velocityXRandom.y), Random.Range(data.velocityYRandom.x, data.velocityYRandom.y));
        pSSize = Random.Range(data.sizeRandom.x, data.sizeRandom.y);
    }

    public UiDouille(DataReloadGraph data)
    {
        InitValues(data);
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
