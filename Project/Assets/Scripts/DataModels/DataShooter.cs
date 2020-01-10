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

    [Header("Tir")]
    public float timeWaitBeforeShoot;
    public int nbShootPerSalve;
    public int nbBulletPerShoot = 1;
    public float timeBulletBetweenSalve;
    public float amplitudeMultiplier;
    public float recoverTime;
    public bool shootEvenIfPlayerMoving = false;

    [Header("Specific Stun")]
    public float StunRecoil;
}
