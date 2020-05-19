using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBonusDisplayedInstance
{
    public Color currentColor = Color.red;
    Color savedColor = Color.red;
    private DataUiTemporaryText data = null;
    private float currentTimer = 0;
    public float scale = 0;
    private float currentAlpha = 1;
    public bool isPlacedOnWorld = false;
    public Vector3 posSave = Vector3.zero;
    public Text text = null;
    public GameObject go = null;
    public RectTransform rt = null;

    public void OnCreation(DataUiTemporaryText _data, Color _color, Text _text, GameObject _go, RectTransform _rt)
    {
        data = _data;
        savedColor = _color;
        currentColor = _color;
        text = _text;
        go = _go;
        rt = _rt;
    }

    public void IsPlacedInWorld(bool _isPlacedOnWorld, Vector3 _posSave)
    {
        isPlacedOnWorld = _isPlacedOnWorld;
        posSave = _posSave;
    }

    public void UpdateValues(Vector2 mousePosition)
    {
        scale = Mathf.MoveTowards(scale, 1, Time.unscaledDeltaTime / data.timeToScaleToMax);

        float distanceWithCursor = Vector2.Distance(rt.position, mousePosition) / Screen.width;

        float alphaMultiplier = 1;
        if (distanceWithCursor < data.minDistDetectMouse) alphaMultiplier = 0;
        else if (distanceWithCursor < data.maxDistDetectMouse) alphaMultiplier = (distanceWithCursor - data.minDistDetectMouse) / (data.maxDistDetectMouse - data.minDistDetectMouse);
        alphaMultiplier = Mathf.Lerp(data.minAlphaMutliplier, data.maxAlphaMutliplier, alphaMultiplier);

        currentTimer += Time.unscaledDeltaTime;
        if (currentTimer > data.timeStayVisible)
        {
            currentAlpha -= Time.unscaledDeltaTime / data.timeToFade;
            if (currentAlpha < 0)
                UiScoreBonusDisplay.Instance.deleteSpot(this);
            else
            {
                currentColor = new Color(savedColor.r, savedColor.g, savedColor.b, currentAlpha * alphaMultiplier);
            }
        }
        else
        {
            currentColor = new Color(savedColor.r, savedColor.g, savedColor.b, currentAlpha * alphaMultiplier);
        }
    }

}
