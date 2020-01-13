using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Gravity/DataGravityOrb")]
public class DataGravityOrb : ScriptableObject
{
    [Header("Initial pull settings")]

    [PropertyRange(0f, 50f)]
    public float fGravityBullet_AttractionRange = 0f;

    [PropertyRange(0f, 1000f)]
    public float fPullForce = 0f;

    [PropertyRange(0f, 1f)]
    public float fTimeBeforeHold = 0f;

    [Header("Lock pull settings")]

    public float fLockTime = 0f;

    [PropertyRange(0f, 1f)]
    public float fWaitingTimeBetweenAttractions = 0f;

    [PropertyRange(0f, 50f)]
    public float fHoldRange = 0f;

    [PropertyRange(0f, 1000f)]
    public float fHoldForce = 0f;

    [Header("Variance settings")]

    public bool bIsSticky = false;

    public bool bIsExplosive = false;

    public LayerMask layerMask;

    [EnableIf("bIsExplosive")]
    public Vector3 v3OffsetExplosion = Vector3.zero;

    [PropertyRange(0f, 5000f), EnableIf("bIsExplosive")]
    public float fExplosionForce = 0f;

    [Header("FloatExplo settings")]

    [ShowWhen("bIsExplosive")]
    public bool bIsFloatExplosion = false;

    [PropertyRange(0f, 3f), EnableIf("bIsFloatExplosion")]
    public float tTimeBeforeFloatActivate = 0f;

    [PropertyRange(0f, 1000f), EnableIf("bIsFloatExplosion")]
    public float bUpwardsForceOnFloat = 0f;

    [EnableIf("bIsFloatExplosion")]
    public bool bIsSlowedDownOnFloat = false;

    [PropertyRange(0f, 10f), EnableIf("bIsFloatExplosion")]
    public float tFloatTime = 0f;

    [Header("Slow Mo")]
    public bool bZeroGIndependantTimeScale = true;
    [PropertyRange(0f, 0.999f)]
    public float fSlowMoPower;
    [PropertyRange(0f, 5f)]
    public float fSlowMoDuration;
    [PropertyRange(0f, 1f)]
    public float fSlowMoProbability;
}
