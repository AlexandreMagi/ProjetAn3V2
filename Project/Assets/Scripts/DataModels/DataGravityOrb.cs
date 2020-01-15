using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Gravity/DataGravityOrb")]
public class DataGravityOrb : ScriptableObject
{
    [Header("Initial pull settings")]

    [PropertyRange(0f, 50f)]
    public float gravityBullet_AttractionRange = 0f;

    [PropertyRange(0f, 1000f)]
    public float pullForce = 0f;

    [PropertyRange(0f, 1f)]
    public float timeBeforeHold = 0f;

    [Header("Lock pull settings")]

    public float lockTime = 0f;

    [PropertyRange(0f, 1f)]
    public float waitingTimeBetweenAttractions = 0f;

    [PropertyRange(0f, 50f)]
    public float holdRange = 0f;

    [PropertyRange(0f, 1000f)]
    public float holdForce = 0f;

    [Header("Variance settings")]

    public bool isSticky = false;

    public bool isExplosive = false;

    public LayerMask layerMask;

    [ShowIf("isExplosive")]
    public Vector3 offsetExplosion = Vector3.zero;

    [PropertyRange(0f, 5000f), ShowIf("isExplosive")]
    public float explosionForce = 0f;

    [Header("FloatExplo settings")]

    [ShowWhen("isExplosive")]
    public bool isFloatExplosion = false;

    [PropertyRange(0f, 3f), ShowIf("isFloatExplosion")]
    public float timeBeforeFloatActivate = 0f;

    [PropertyRange(0f, 1000f), ShowIf("isFloatExplosion")]
    public float upwardsForceOnFloat = 0f;

    [EnableIf("isFloatExplosion")]
    public bool isSlowedDownOnFloat = false;

    [PropertyRange(0f, 10f), ShowIf("isFloatExplosion")]
    public float floatTime = 0f;

    [Header("Slow Mo")]
    public bool zeroGIndependantTimeScale = true;
    [PropertyRange(0f, 0.999f)]
    public float slowMoPower;
    [PropertyRange(0f, 5f)]
    public float slowMoDuration;
    [PropertyRange(0f, 1f)]
    public float slowMoProbability;


    [Header("FX orb")]
    public string fxName = "VFX_Orbe";
    public string fxExplosionName = "VFX_OrbExplosion";
    public float fxSizeMultiplier = 0.2f;
}
