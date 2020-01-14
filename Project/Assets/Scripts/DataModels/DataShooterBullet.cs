﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataShooterBullet")]
public class DataShooterBullet : DataEnemy
{
    public AnimationCurve bulletTrajectory;
    public AnimationCurve bulletRotation;
    public AnimationCurve circleScale;

    public float bulletSpeed = 5;
    public float randomSpeedAdded = 2;
    public float shakeAtImpact = 10;
    public float rotationSpeed = 90;
    public float circleScaleMultiplier = 1;
    public bool randomRotationAtStart = false;

    public LayerMask layerAffected;

    public float timeBeforeCollisionAreActived = 1;

    public Vector3 randomFrom = -Vector3.one;
    public Vector3 randomTo = Vector3.one;

    [Header("Shooter explosion settings")]
    public float explosionForce = 500;
    public float explosionRadius = 5;
    public float explosionDamage = 50;
    public float explosionStun = 1.2f;
    public float explosionStunDuration = 1;
    public float liftValue = 0;


}
