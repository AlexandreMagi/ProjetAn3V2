using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataPlayer")]
public class DataPlayer : DataEntity
{
    public float armor;
    public float damageShakeMultiplier = 40;
}
