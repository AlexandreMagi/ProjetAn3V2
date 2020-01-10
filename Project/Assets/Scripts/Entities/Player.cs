using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity, ISpecialEffects
{
    public static Player Instance{get; private set;}

    public void OnExplosion(float dammage, float propulsion, float stunValue)
    {
        TakeDamage(dammage);
    }

    void Awake()
    {
        Instance = this;
    }
}
