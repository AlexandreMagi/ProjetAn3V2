using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataShooter")]
public class DataShooter : DataEnemy
{
    [Header("Rotation")]
    public float rotationSpeed;
    public float rotationMinimalBeforeCharge;
    public float playerMoveTimeReset;
    public float distanceDetection;

    [Header("Tir")]
    public float timeWaitBeforeShoot;
    public int nbShootPerSalve;
    public int nbBulletPerShoot = 1;
    public float timeBetweenBullet;
    public float amplitudeMultiplier;
    public float recoverTime;
    public bool shootEvenIfPlayerMoving = false;
    public DataShooterBullet bulletData;
    public GameObject bulletPrefabs;
    public string muzzleFlashFx = "VFX_MuzzleFlashShooter";

    [Header("Specific Stun")]
    public float stunRecoil;

}
