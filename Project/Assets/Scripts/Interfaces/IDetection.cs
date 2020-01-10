using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetection
{
    void OnAudioDetect();

    void OnSightDetect();

    void OnDistanceDetect(Transform target);

}
