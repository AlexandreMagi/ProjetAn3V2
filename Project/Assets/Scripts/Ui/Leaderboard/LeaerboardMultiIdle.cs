using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaerboardMultiIdle : MonoBehaviour
{

    [Header("Move")]
    [SerializeField] Transform[] objectsToIdleInWave = null;
    [SerializeField] float timeDelayMoveBetween = 0.5f;
    [SerializeField] float idleMoveSpeed = 3;
    [SerializeField] float idleMoveMagnitude = 3;

    Vector3[] baseLocalPos = null;

    [Header("Scale")]
    [SerializeField] Transform[] objectsToIdleInScale = null;
    [SerializeField] float timeDelayScaleBetween = 0.5f;
    [SerializeField] float idleScaleSpeed = 3;
    [SerializeField] float idleScaleMagnitude = 3;

    [Header("Weight")]
    [SerializeField] float currWeight = 0;
    [SerializeField] float weightGoTo = 0;
    [SerializeField] float weightTimeTransition = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        baseLocalPos = new Vector3[objectsToIdleInWave.Length];
        for (int i = 0; i < objectsToIdleInWave.Length; i++)
        {
            baseLocalPos[i] = objectsToIdleInWave[i].localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // --- Wave
        for (int i = 0; i < objectsToIdleInWave.Length; i++) 
        { 
            objectsToIdleInWave[i].localPosition = baseLocalPos[i] + Vector3.Lerp(Vector3.zero, Vector3.up * Mathf.Sin(Time.unscaledTime * idleMoveSpeed + timeDelayMoveBetween * i) * idleMoveMagnitude, currWeight);
        }

        // --- Scale
        for (int i = 0; i < objectsToIdleInScale.Length; i++)
        {
            objectsToIdleInScale[i].localScale = Vector3.one + Vector3.Lerp(Vector3.zero, Vector3.one * Mathf.Sin(Time.unscaledTime * idleScaleSpeed + timeDelayScaleBetween * i) * idleScaleMagnitude, currWeight);
        }

        currWeight = Mathf.MoveTowards(currWeight, weightGoTo, Time.unscaledDeltaTime / weightTimeTransition);
    }

    public void ChangeWeightSettings(float _weightGoTo, float _weightTimeTransition = 1, float _currWeight = -1)
    {
        if (_currWeight >= 0) currWeight = _currWeight;
        weightGoTo = _weightGoTo;
        weightTimeTransition = _weightTimeTransition;
    }

}
