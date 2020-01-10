﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected float health;

    [SerializeField]
    protected DataEntity entityData;

    protected virtual void Start()
    {
        health = entityData.startHealth;

        TeamsManager.Instance.RegistertoTeam(this.transform, this.entityData.team);
    }

    protected virtual void Die()
    {
        //Destroy(this.gameObject);
        TeamsManager.Instance.RemoveFromTeam(this.transform, this.entityData.team);

        this.gameObject.SetActive(false);
    }

    protected virtual void TakeDamage(float value)
    {
        health -= value;
        if (health <= 0) Die();
    }

    protected virtual void Heal(float value)
    {
        health += value;
        if (health > entityData.maxHealth) health = entityData.maxHealth;
    }
}
