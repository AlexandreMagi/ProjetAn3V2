using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualHandler : MonoBehaviour
{
    /*
    [SerializeField] GameObject prefabModel = null;
    [SerializeField] float ecartTotal = 7f;
    [SerializeField] float scaleXTotal = 10f;
    [SerializeField] Vector2Int selectedFrequencies = new Vector2Int(0, 4);
    */
    [SerializeField] int puissance = 16;
    //GameObject[] allBars = null;
    float[] spectrum = null;

    [SerializeField] bool recordOnCurve = false;
    [SerializeField] AnimationCurve curveDebug = null;
    float savedCurrSoundValue = 0;

    [SerializeField] float refValueForBeat = 5;

    bool hasBeat = false;

    [SerializeField] float multiplierValue = 20;

    [SerializeField] Renderer[] allRenderers = null;

    [SerializeField] AnimationCurve animCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float beatTime = 0.5f;
    float currPurcentageBeat = 1;

    void Start()
    {
        //allBars = new GameObject[puissance];
        //for (int i = 0; i < puissance; i++)
        //{
        //    allBars[i] = Instantiate(prefabModel, transform.position + Vector3.right * i * (ecartTotal/ puissance), transform.rotation);
        //}
        //prefabModel.SetActive(false);

        spectrum = new float[puissance];
    }

    void Update()
    {
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
        float currSoundValue = 0;
        //for (int i = 0; i < puissance; i++)
        //{
        //    allBars[i].transform.localScale = Vector3.Lerp(allBars[i].transform.localScale, new Vector3(scaleXTotal/ puissance, 0.1f + 0.9f * spectrum[i] * 25, 1), Time.unscaledDeltaTime * 16);
        //    //if (i >= selectedFrequencies.x && i <= selectedFrequencies.y) currSoundValue += spectrum[i] / (selectedFrequencies.y - selectedFrequencies.x);

        //}
        currSoundValue = spectrum[0];
        currSoundValue *= multiplierValue;
        currSoundValue = Mathf.Clamp01(currSoundValue);
        savedCurrSoundValue = Mathf.Lerp(savedCurrSoundValue, currSoundValue, Time.deltaTime * 8);

        if (savedCurrSoundValue > refValueForBeat && !hasBeat) Beat();
        if (savedCurrSoundValue < refValueForBeat && hasBeat) hasBeat = false;

        if (recordOnCurve)
            curveDebug.AddKey(Time.time, savedCurrSoundValue);

        if (currPurcentageBeat < 1)
        {
            currPurcentageBeat += Time.deltaTime / beatTime;
            if (currPurcentageBeat > 1)
            {
                currPurcentageBeat = 1;
            }
        }

        if (allRenderers!= null)
        {
            foreach (var _renderer in allRenderers)
            {
                _renderer.material.SetFloat("_RevealLightEnabled", animCurve.Evaluate(currPurcentageBeat));
            }
        }

    }


    void Beat()
    {
        hasBeat = true;
        if (currPurcentageBeat == 1)
        {
            currPurcentageBeat = 0;
        }
        //Debug.Log("Beat");
    }
}
