using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataDamageFb")]
public class DataDamageFb : ScriptableObject
{
    public float flashTime = 0.1f;
    public float flashAlpha = .6f;
    public float stateTime = 1;

    public float shieldBreakFlash = 0.2f;

}
