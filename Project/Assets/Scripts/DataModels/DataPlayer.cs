using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataPlayer")]
public class DataPlayer : DataEntity
{
    public float armor;
    public float damageShakeMultiplier = 40;
    public float shakeAtArmorDestruction = 3;
    public string shakeAtArmorFx = "VFX_ShieldBreak";
    public float stopTimeAtShieldBreak = 0.2f;
    public float stopTimeAtDammage = 0.05f;

    public float damageScaleShieldMultiplier = 1;
    public float damageScaleLifeMultiplier = 1.2f;
}
