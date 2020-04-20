using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataPlayer")]
public class DataPlayer : DataEntity
{
    public float armor;
    public float maxArmor;
    public float damageShakeMultiplier = 40;
    public float shakeAtArmorDestruction = 3;
    public string shakeAtArmorFx = "VFX_ShieldBreak";
    public float stopTimeAtShieldBreak = 0.2f;
    public float stopTimeAtDammage = 0.05f;

    public float damageScaleShieldMultiplier = 1;
    public float damageScaleLifeMultiplier = 1.2f;

    public float respawnExplosionRadius = 20;
    public float respawnExplosionForce = 15000;
    public float respawnExplosionDamage = 100;
    public float respawnExplosionStun = 500;
    public float respawnExplosionStunDuration = 1;
    public float respawnExplosionLiftValue = 1;
    public string respawnExplosionFx = "VFX_ExplosionShooterBullet";
}
