using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Gravity/DataGravityOrb")]
public class DataGravityOrb : ScriptableObject
{
    [Header("Initial pull settings")]

    [Range(0f, 50f)]
    public float fGravityBullet_AttractionRange = 0f;

    [Range(0f, 1000f)]
    public float fPullForce = 0f;

    [Range(0f, 1f)]
    public float fTimeBeforeHold = 0f;

    [Header("Lock pull settings")]

    public float fLockTime = 0f;

    [Range(0f, 1f)]
    public float fWaitingTimeBetweenAttractions = 0f;

    [Range(0f, 50f)]
    public float fHoldRange = 0f;

    [Range(0f, 1000f)]
    public float fHoldForce = 0f;

    [Header("Variance settings")]

    public bool bIsSticky = false;

    public bool bIsExplosive = false;

    public LayerMask layerMask;

    [ShowWhen("bIsExplosive")]
    public Vector3 v3OffsetExplosion = Vector3.zero;

    [Range(0f, 5000f), ShowWhen("bIsExplosive")]
    public float fExplosionForce = 0f;

    [Header("FloatExplo settings")]

    [ShowWhen("bIsExplosive")]
    public bool bIsFloatExplosion = false;

    [Range(0f, 3f), ShowWhen("bIsFloatExplosion")]
    public float tTimeBeforeFloatActivate = 0f;

    [Range(0f, 1000f), ShowWhen("bIsFloatExplosion")]
    public float bUpwardsForceOnFloat = 0f;

    [ShowWhen("bIsFloatExplosion")]
    public bool bIsSlowedDownOnFloat = false;

    [Range(0f, 10f), ShowWhen("bIsFloatExplosion")]
    public float tFloatTime = 0f;

    [Header("Slow Mo")]
    public bool bZeroGIndependantTimeScale = true;
    [RangeAttribute(0f, 0.999f)]
    public float fSlowMoPower;
    [RangeAttribute(0f, 5f)]
    public float fSlowMoDuration;
    [RangeAttribute(0f, 1f)]
    public float fSlowMoProbability;
}
