using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataShooterBullet")]
public class DataShooterBullet : ScriptableObject
{
    public AnimationCurve bulletTrajectory;
    public AnimationCurve bulletRotation;
    public AnimationCurve circleScale;

    public float bulletSpeed = 5;
    public float randomSpeedAdded = 2;
    public int bulletDammage = 10;
    public float shakeAtImpact = 10;
    public float rotationSpeed = 90;
    public float circleScaleMultiplier = 1;
    public bool randomRotationAtStart = false;

    public float explosionRange = 5;

    public float forceAppliedOnImpact = 500;
    public float stunValue = 1.2f;

    public float timeBeforeCollisionAreActived = 1;

    public Vector3 randomFrom = -Vector3.one;
    public Vector3 randomTo = Vector3.one;


}
