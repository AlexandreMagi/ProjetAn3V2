﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterAnimationEffect : MonoBehaviour
{

    [SerializeField]
    GameObject[] objectToDisable = null;

    [SerializeField]
    GameObject[] objectToEnable = null;

    [SerializeField]
    float timeBeforeStart, shakeForce, shakeDuration;

    void disableGameobject()
    {
        if (objectToDisable != null)
            foreach (GameObject obj in objectToDisable)
            {
                obj.SetActive(false);
            }
    }
    void enableGameobject()
    {

        if (objectToEnable != null)
            foreach (GameObject obj in objectToEnable)
            {
                obj.SetActive(true);
            }
    }

    void Shake()
    {
        Debug.Log("Shake");
        TriggerUtil.TriggerShake(timeBeforeStart, shakeForce, shakeDuration);
    }
}
