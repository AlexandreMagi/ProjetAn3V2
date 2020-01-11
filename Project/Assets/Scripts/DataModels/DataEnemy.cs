using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataEnemy : DataEntity
{
    [Header("Enemy var")]
    public float stunResistanceJauge = 1;
    public bool stayLockedOnTarget = false;
    public float timeBeforeCheckForAnotherTarget = 5;
}
