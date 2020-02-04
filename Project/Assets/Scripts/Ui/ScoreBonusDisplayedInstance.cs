using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void OnCreation(DataUiTemporaryText _data, Color _color)
    {
        data = _data;
        savedColor = _color;
        currentColor = _color;
    }

    public void IsPlacedInWorld(bool _isPlacedOnWorld, Vector3 _posSave)
    {
        isPlacedOnWorld = _isPlacedOnWorld;
        posSave = _posSave;
    }

    public void UpdateValues()
    {
        scale = Mathf.MoveTowards(scale, 1, Time.unscaledDeltaTime / data.timeToScaleToMax);

        currentTimer += Time.unscaledDeltaTime;
        if (currentTimer > data.timeStayVisible)
        {
            currentAlpha -= Time.unscaledDeltaTime / data.timeToFade;
            if (currentAlpha < 0)
                UiScoreBonusDisplay.Instance.deleteSpot(this);
            else
                currentColor = new Color(savedColor.r, savedColor.g, savedColor.b, currentAlpha);
        }
    }

}
