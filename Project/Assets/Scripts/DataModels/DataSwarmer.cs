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

    public float speed;

    public float damage;

    public float upScale;

    public float distanceToTargetEnemy;

    public float distanceMelee;

    public float waitDuration = 0.3f;
    public float distanceBeforeAttack = 6f;
    public float jumpForce = 80f;
    public float speedMultiplierWhenAttacking = 4;


    public Material mat;
}
