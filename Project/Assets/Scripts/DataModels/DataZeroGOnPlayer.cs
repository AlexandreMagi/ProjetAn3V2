using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataZeroGOnPlayer")]
public class DataZeroGOnPlayer : ScriptableObject
{

    public AnimationCurve upAxisAnim = AnimationCurve.Linear(0, 0, 1, 1);
    public float upAxisValue = 1;


    public AnimationCurve rollSpeedController = AnimationCurve.Linear(0, 0, 1, 1);
    public float rollSpeed;

    public AnimationCurve screenOpacity = AnimationCurve.Linear(0, 0, 1, 1);

    public float animTime = 1;

    public float rotRecoverSpeed;

    public bool animIndependentFromTimescale = true;

}
