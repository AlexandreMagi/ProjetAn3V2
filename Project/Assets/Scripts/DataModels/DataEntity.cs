using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DataEntity : ScriptableObject
{
    public float maxHealth;
    public float startHealth;
    public int team;

    public float shakeOnDie;
    public float shakeOnDieTime;
    public GameObject fracturedProp;
    public float fracturedForceOnDie = 100;

    public DataUiTemporarySprite spriteToDisplayShield;
    public DataUiTemporarySprite spriteToDisplayLife;

    public override string ToString() 
    {
        return $"Team : {team}, maxHealth : {maxHealth}";
            
            }
}
