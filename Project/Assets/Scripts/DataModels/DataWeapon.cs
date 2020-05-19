using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataWeapon")]
public class DataWeapon : ScriptableObject
{
    [Header("Weapon Mods")]
    public DataWeaponMod baseShot;
    public DataWeaponMod chargedShot;
    public LayerMask layerMaskHit;

    [Header("Base Parameters")]
    public bool chargeSpeedIndependantFromTimeScale = true;
    [RangeAttribute(0.001f, 5)]
    public float chargeTime;
    public bool canReloadAnytime = false;

    [RangeAttribute(1, 50)]
    public int bulletMax;
    public float reloadCooldown = 0.5f;
    public float reloadingTime = 1;
    public float perfectPlacement = 0.7f;
    public float perfectRandom = 0.3f;
    public float perfectRange = 0.05f;
    public float reloadingStartShake = 0.3f;
    public float reloadingShake = 0.3f;
    public float reloadingPerfectShake = 0.5f;
    public float reloadingMissTryShake = 0.5f;
    public float reloadingMissTryRecoil = 1;
    public float reloadingPerfectRecoil = 0.5f;
    public float reloadingPerfectSlowmo = 0.8f;
    public float reloadingPerfectSlowmoDur = 0.5f;
    public int bulletAddedIfPerfect = 3;

    public float recoilIfNoBullet = 2;
    public float shakeIfNoBullet = 0.2f;

    [RangeAttribute(1, 50)]
    public float gravityOrbCooldown;
    public bool grabityOrbCooldownRelativeToTime = false;
    public bool gravityOrbCanBeReactivated = false;

    [Header("AnimCam WeaponCharged")]
    public AnimationCurve AnimValue = AnimationCurve.Linear(0, 0, 1, 0);
    public float animTime = 5;
    public float fovModifier = 1;

    [Header("Light parameters")]
    public float baseAngle = 55;
    public float chargedAngle = 80;
    public float baseIntensity = 0.6f;
    public float chargedIntensity = 0.8f;
    public float baseRange = 15;
    public float chargedRange = 25;

    public float distanceIntensityMultiplier = 1;
    public float distanceRangeMultiplier = 1;

    public float distanceMax = 10;

    [Range (0f,1f)]
    public float distanceImpactPurcentageOnValueIntensity = 0.5f;
    [Range(0f, 1f)]
    public float distanceImpactPurcentageOnValueRange = 0.5f;

    public float distanceMultiplierLerpSpeed = 8;
    public LayerMask maskCheckDistanceForLight = 0;


    [Header("Minigun parameters")]


    public float minigunMinRateOfFire = 12;
    public float minigunMaxRateOfFire = 12;
    public float minigunRoFTimeToGoUp = 1;
    public float minigunRoFTimeToGoDown = 1;


    public float timeToMaxImprecision = 1;
    public float timeToMinImprecision = 0.5f;
    public float minImprecision = 0;
    public float maxImprecision = 0.1f;
    public float minImprecisionFrequency = 0f;
    public float maxImprecisionFrequency = 2f;
}
