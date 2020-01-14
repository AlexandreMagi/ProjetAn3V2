﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity<DataPlayer>, ISpecialEffects
{

    float armor = 0;
  //  private DataPlayer playerData;

    public static Player Instance{get; private set;}

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
    //    ReactSpecial.DoExplosionDammage(this, explosionOrigin, explosionDamage, explosionRadius);
    }

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        entityData = entityData as DataPlayer;
        armor = entityData.armor;
        UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
        UiLifeBar.Instance.UpdateLifeDisplay(health / entityData.maxHealth, health);
    }

    protected virtual void Update()
    {

    }

    public override void TakeDamage(float value)
    {
        if (value >= armor)
        {
            value -= armor;
            armor = 0;
        }
        else
        {
            armor -= value;
            value = 0;
        }
        UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
        UiLifeBar.Instance.UpdateLifeDisplay((health - value) / entityData.maxHealth, health - value);
        base.TakeDamage(value);
    }

    public void GainArmor(float value)
    {
        armor += value;
        UiLifeBar.Instance.UpdateArmorDisplay(armor / entityData.armor, armor);
    }
}
