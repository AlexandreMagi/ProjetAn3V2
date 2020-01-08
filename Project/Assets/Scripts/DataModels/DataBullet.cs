using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataBullet")]
public class DataBullet : ScriptableObject
{
    [Header("Base Parameters")]
    [RangeAttribute(1, 100)]
    public int damage;
    [RangeAttribute(0f, 50f)]
    public float stunValue;

    [Header("Weapon's fades")]
    public bool dammageFadeWithDistance;
    public bool stunFadeWithDistance;
    [RangeAttribute(0f, 50f)]
    public float distanceDammageFade;
    [RangeAttribute(0f, 50f)]
    public float distanceStunFade;

    [Header("Weapon's feedback on Time")]
    public bool activateStopTimeAtImpact;
    [RangeAttribute(0f, 1f)]
    public float timeStopAtImpact;
    [RangeAttribute(0f, 1f)]
    public float timeStopProbability = 1;

    public bool activateSlowMoAtImpact;
    [RangeAttribute(0f, 0.999f)]
    public float slowMoPower;
    [RangeAttribute(0f, 5f)]
    public float slowMoDuration;
    [RangeAttribute(0f, 1f)]
    public float slowMoProbability;
}
