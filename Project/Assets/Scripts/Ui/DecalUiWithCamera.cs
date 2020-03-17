using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalUiWithCamera : MonoBehaviour
{

    [SerializeField] RectTransform[] allMovableRoot = new RectTransform[0];
    [SerializeField] float multiplierPos = 100;
    [SerializeField] float clampValue = 40;
    [SerializeField] float lerpSpeedFollow = 5;

    // Update is called once per frame
    void Update()
    {
        Vector2 pointDelayOnRotation = CameraHandler.Instance.pointDelayOnRotation();
        pointDelayOnRotation = new Vector2(pointDelayOnRotation.x / Screen.width, pointDelayOnRotation.y / Screen.height);
        pointDelayOnRotation = pointDelayOnRotation * 2 - Vector2.one;

        pointDelayOnRotation = new Vector2(Mathf.Clamp(pointDelayOnRotation.x * multiplierPos, -clampValue, clampValue), Mathf.Clamp(pointDelayOnRotation.y * multiplierPos, -clampValue, clampValue));

        float decalYFromStep = CameraHandler.Instance.decalFromStepY() * 100;

        for (int i = 0; i < allMovableRoot.Length; i++)
        {
            allMovableRoot[i].transform.localPosition = Vector2.Lerp (allMovableRoot[i].transform.localPosition, pointDelayOnRotation + Vector2.up * decalYFromStep, Time.deltaTime * lerpSpeedFollow);
        }
    }
}
