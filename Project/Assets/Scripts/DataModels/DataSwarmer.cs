using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataSwarmer")]
public class DataSwarmer : DataEnemy
{
    [Header("Swarmer var")]
    public float varianceInPath;

    public float timeBeforeNextPath;

    public float distanceBeforeNextPath;

    public float speed;

    public float damage;

    public float upScale;

    public float distanceToTargetEnemy;

    public float distanceMelee;

    public float waitDuration = 0.3f;
    public float distanceBeforeAttack = 6f;
    public float jumpForce = 80f;
    public float speedMultiplierWhenAttacking = 4;

    public bool targetsPlayerAtEndOfPath = true;

    [Header("AI variables")]
    public float frontalDetectionSight = 2;
    public float jumpHeight = 3;
    public float jumpDodgeForce = 2500;
    public float jumpCooldownInitial = .5f;

    public float numberOfSideTries = 4;
    public float tryStep = 1;
    public float distanceDodgeStep = 1;
    public float extraLengthByStep = .2f;
    public float sideDetectionSight = 1;
    public float dodgeSlideForce = 350;

    public float timeForUpwardsTransition = .2f;
    public float maxBlockedRetryPathTime = 2f;
    public float maxBlockedSuicideTime = 4f;
    public float considerStuckThreshhold = 1f;
    public float initialTimeToConsiderCheck = .5f;

    public float maxHeightToChaseWaypoint = .3f;

    public float rayCastRangeToConsiderAirbone = .7f;

    public float pushForce = 150;
    public float upwardsPushForce = 50;

    [Header("Material")]
    public Material mat;

    public string vfxToPlayWhenPulledByGrav = "VFX_Orbe";
    public string vfxToPlayWhenHoldByGrav = "VFX_Orbe";
    public string vfxToPlayWhenReleaseByGrav = "VFX_Orbe";
}
