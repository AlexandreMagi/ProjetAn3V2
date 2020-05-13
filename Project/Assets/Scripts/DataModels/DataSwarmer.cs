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
    public float angleToIgnorePath = 120;

    public float speed;
    public float maximumSpeed = 350;
    [Range(0f, 1f), Tooltip("Le pourcentage de la vitesse de base du swarmer quand il est dans les airs. Plus il est haut, plus la vitesse de base sera conservée.")]
    public float percentSpeedInTheAir = .7f;

    public float damage;

    public float accelerationConversionRate = .1f;
    public float maximumVelocity = 10f;

    public float distanceToTargetEnemy;

    public float distanceMelee;

    public float waitDuration = 0.3f;
    public float distanceBeforeAttack = 6f;
    public float distanceApproximation = 1f;
    public float jumpForce = 80f;
    public float speedMultiplierWhenAttacking = 4;

    public bool spawnsPartsOnDeath = true;

    public GameObject deadBody = null;

    public bool targetsPlayerAtEndOfPath = true;

    [Header("AI variables")]
    public bool hasDodgeIntelligence = true;

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
    public float distanceMinWithCamToPlayVFX = 5;
    public float timeToChangeColorWhileAttacking = 0.5f;
}
