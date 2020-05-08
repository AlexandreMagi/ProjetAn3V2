using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataWeaponMod")]
public class DataWeaponMod : ScriptableObject
{
    [Header("Bullet Data")]
    public DataBullet bullet;

    [Header("Shake Cam")]
    [RangeAttribute(0f, 50f)]
    public float shakePerShot;
    [RangeAttribute(0f, 50f)]
    public float shakeTimePerShot;
    [RangeAttribute(0f, 50f)]
    public float shakePerHit;
    [RangeAttribute(0f, 1.5f)]
    public float recoilPerShot;
    [RangeAttribute(0f, 1.5f)]
    public float stopTimeAtImpact;

    [Header("Bullet properties")]
    [RangeAttribute(1, 100)]
    public int bulletPerShoot = 1;
    [RangeAttribute(0, 100)]
    public int bulletCost = 1;
    [RangeAttribute(0f, 0.5f)]
    public float bulletImprecision;

    [Header("Other")]
    public float shootValueUiRecoil = 1;
    public float hitValueUiRecoil = 1;
    public string soundPlayed = "shootSound";
    public bool firstBulletAlwaysPrecise = true;
    public float firstBulletDamageMultiplier = 1;
}
