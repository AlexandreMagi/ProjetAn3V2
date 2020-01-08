using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleManager : MonoBehaviour
{
    private static TimeScaleManager _instance;
    public static TimeScaleManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    List<Vector3> slowMoRequest = new List<Vector3>();
    bool slowMoEnable = true;
    bool stopTime = false;

    public void AddSlowMo(float power, float duration, float delay = 0, float probability = 1)
    {
        if (Random.Range(0f, 1f) < probability)
            slowMoRequest.Add(new Vector3(power, duration, delay));
    }

    public void Pause()
    {
        slowMoEnable = false;
        Time.timeScale = 1;
    }

    public void Stop()
    {
        slowMoRequest = new List<Vector3>();
        Time.timeScale = 1;
    }

    public void Play()
    {
        slowMoEnable = true;
    }

    private void Update()
    {
        UpdateSlowMo();
    }

    private void UpdateSlowMo()
    {
        if (slowMoEnable)
        {
            float currentSlowMo = 0;
            for (int i = slowMoRequest.Count - 1; i > 0; i--)
            {
                if (slowMoRequest[i].z <= 0)
                {
                    slowMoRequest[i] = slowMoRequest[i] - Vector3.forward * Time.unscaledDeltaTime;
                }
                else
                {
                    slowMoRequest[i] = slowMoRequest[i] - Vector3.up * Time.unscaledDeltaTime;
                    if (slowMoRequest[i].y < 0)
                        slowMoRequest.RemoveAt(i);
                    if (slowMoRequest[i] != null && slowMoRequest[i].x > currentSlowMo)
                        currentSlowMo = slowMoRequest[i].x;
                }
            }
            Time.timeScale = 1 - currentSlowMo;
        }
    }

}
