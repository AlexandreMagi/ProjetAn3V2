using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataSwarmer")]
public class DataSwarmer : DataEnemy
{
    [Header("Swarmer var")]
    public float nVarianceInPath;

    public float tTimeBeforeNextPath;

    public float fDistanceBeforeNextPath;

    public float speed;

    public float damage;

    public float upScale;

    public float distanceToTargetEnemy;

    public float fDistanceMelee;

    public float fWaitDuration = 0.3f;
    public float fDistanceBeforeAttack = 6f;
    public float fJumpForce = 80f;
    public float fSpeedMultiplierWhenAttacking = 4;


    public Material mat;
}
