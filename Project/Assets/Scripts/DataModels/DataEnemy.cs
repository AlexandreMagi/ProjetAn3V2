using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataEnemy : DataEntity
{
    [Header("Enemy var")]
    public float stunResistanceJauge = 1;
    public bool stayLockedOnTarget = false;
    public float timeBeforeCheckForAnotherTarget = 5;
    public string fxWhenDie = "fx_name_death_ennemi";
    public string fxWhenDieDecals = "fx_name_death_decal_ennemi";
    public string fxWhenStun = "VFX_Stun";
    public string fxWhenTakeDamage = "VFX_BloodProjected";
    public Material matWhenTakeDammage = null;
    public float matChangeTime = 0.1f;

    public float shakeWhenTakeDamageForce = 0;
    public float shakeWhenTakeDamageDuration = 0;
}
