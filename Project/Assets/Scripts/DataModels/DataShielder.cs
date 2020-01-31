using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataShielder")]
public class DataShielder : DataEnemy
{
    [Header("Shielder data")]
    public float distanceToStartFollowingAlly;

    public float movementSpeed;

    [Header("Shield")]
    public float shieldApplyRange;

    public float shieldRadius;

    public float timeShieldCast;

    public float targetLockFollowSpeed;

    [Header("Dodge")]
    public float timeBeforeDodgeStart;

    public float dodgeSpeed;

    [Header("Float parameters")]
    public float floatSpeed;
    public float floatAmplitude;
}
