using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetection
{
    void OnMovementDetect();

    void OnDangerDetect();

    void OnDistanceDetect(Transform target, float distance);

    void OnCursorClose(Vector3 position);
}
