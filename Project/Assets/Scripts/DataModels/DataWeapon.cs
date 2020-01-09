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
    [RangeAttribute(1, 50)]
    public float gravityOrbCooldown;
    public bool grabityOrbCooldownRelativeToTime = false;
}
