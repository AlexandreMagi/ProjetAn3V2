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

    [RangeAttribute(1, 50)]
    public int bulletMax;
    public float reloadingTime = 1;
    public float perfectPlacement = 0.7f;
    public float perfectRandom = 0.3f;
    public float perfectRange = 0.05f;
    public float reloadingStartShake = 0.3f;
    public float reloadingShake = 0.3f;
    public float reloadingPerfectShake = 0.5f;
    public float reloadingPerfectRecoil = 0.5f;
    public float reloadingPerfectSlowmo = 0.8f;
    public float reloadingPerfectSlowmoDur = 0.5f;
    public int bulletAddedIfPerfect = 3;

    [RangeAttribute(1, 50)]
    public float gravityOrbCooldown;
    public bool grabityOrbCooldownRelativeToTime = false;

    [Header("AnimCam WeaponCharged")]
    public AnimationCurve AnimValue = AnimationCurve.Linear(0, 0, 1, 0);
    public float animTime = 5;
    public float fovModifier = 1;
}
