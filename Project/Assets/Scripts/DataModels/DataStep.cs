using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataStep")]
public class DataStep : ScriptableObject
{
    public AnimationCurve Curve = null;
    public float MultipyValue = 1;
    public float SpeedMultiplier = 1;
    public bool IsInvertedAtEachLoop = false;
    public float Decal = 0;
    public float OffsetValue = 0;
}
