using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataHover")]
public class DataHover : DataEnemy
{
    public float shieldApplyRange;

    public float shieldRadius;

    public float timeShieldCast;
}
