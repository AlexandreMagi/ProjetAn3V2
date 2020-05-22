using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataShooter")]
public class DataShooter : DataEnemy
{
    //[Header("Feedback")]
    //public bool spawnsPartsOnDeath = true;
    //public GameObject deadBody = null;

    [Header("Rotation")]
    public float rotationSpeed;
    public float rotationMinimalBeforeCharge;
    public float playerMoveTimeReset;
    public float distanceDetection;
    public float distanceYWithPlayerUpSupported = 4;

    [Header("Tir")]
    public float timeWaitBeforeShoot;
    public int nbShootPerSalve;
    public float[] specifyBulletRotation;
    public int nbBulletPerShoot = 1;
    public float timeBetweenBullet;
    public float amplitudeMultiplier;
    public float amplitudeCap = 0;
    public float recoverTime;
    public bool shootEvenIfPlayerMoving = false;
    public DataShooterBullet bulletData;
    public GameObject bulletPrefabs;
    public string muzzleFlashFx = "VFX_MuzzleFlashShooter";

    [Header("Specific Stun")]
    public float stunRecoil;

}
