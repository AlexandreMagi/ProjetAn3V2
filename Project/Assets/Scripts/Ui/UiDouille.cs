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

    // --- ShakeVariables
    public Vector3 addedPosViaShake = Vector3.zero;
    float timeCounter = 0; // Temps custom
    float trauma; // Valeur de base pour les calculs
    public float Trauma { get { return trauma; } set { trauma = Mathf.Clamp01(value); } } // Accesseur qui clamp trauma entre 0 et 1
    DataReloadGraph stockData = null;
    Vector2 randomSeedsForShake = Vector2.zero;

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
        stockData = data;
        randomSeedsForShake = new Vector2(Random.Range(0, 100), Random.Range(0, 100));
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

    public void UpdateShakeValue()
    {
        if (Trauma > 0)
        {
            float powedTrauma = Mathf.Pow(Trauma, stockData.traumaPow);
            timeCounter += Time.unscaledDeltaTime * Mathf.Pow(Trauma, stockData.traumaSpeedPow) * stockData.traumaMult;
            addedPosViaShake = GetPerlinVectorThree() * stockData.traumaMag * powedTrauma;
            Trauma -= Time.unscaledDeltaTime * Mathf.Pow(stockData.traumaDecay, stockData.traumaDecayPow);
        }
        else
        {
            addedPosViaShake = Vector3.zero;
        }
    }

    #region Methodes qui permettent de get les valeurs via perlin

    /// <summary>
    /// Récupère une valeur sur un perlin noise
    /// </summary>
    /// <param name="seed"></param>
    /// <returns></returns>
    float GetFloat(float seed) { return (Mathf.PerlinNoise(seed, timeCounter) - 0.5f) * 2f; }

    /// <summary>
    /// Get un vector 3 en fonction de perlin noises
    /// </summary>
    /// <returns></returns>
    Vector3 GetPerlinVectorThree() { return new Vector3(GetFloat(randomSeedsForShake.x), GetFloat(randomSeedsForShake.y), 0); }

    #endregion

}
