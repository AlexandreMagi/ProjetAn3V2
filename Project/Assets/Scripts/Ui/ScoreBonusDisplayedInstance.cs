using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBonusDisplayedInstance
{
    public Color currentColor = Color.red;
    private DataUiTemporaryText data = null;
    private float currentTimer = 0;
    public float scale = 0;
    private float currentAlpha = 1;
    public bool isPlacedOnWorld = false;
    public Vector3 posSave = Vector3.zero;

    public void OnCreation(DataUiTemporaryText _data)
    {
        data = _data;
        currentColor = _data.colorMain;
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
                currentColor = new Color(data.colorMain.r, data.colorMain.g, data.colorMain.b, currentAlpha);
        }
    }

}
