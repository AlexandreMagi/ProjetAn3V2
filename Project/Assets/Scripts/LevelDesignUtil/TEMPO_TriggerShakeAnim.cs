using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEMPO_TriggerShakeAnim : MonoBehaviour
{

    [SerializeField]
    float shakeForce = 10f;

    [SerializeField]
    float shakeDuration = 0.5f;

    void TriggerShake()
    {
        CameraHandler.Instance.AddShake(shakeForce, shakeDuration);
    }

}
