using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy<T> : Entity<T>, IDetection, IBulletAffect where T : DataEnemy
{
    float currentStunLevel = 0;
    float timeRemaingingStun = 0;
    protected bool isStun = false;

    List<Transform> enemies;
    float distanceToClosest;
    Transform possibleTarget;

    protected Transform target;

    float timerCheckTarget = 0;
    float checkEvery = 0.2f;

    float timeRemainingInMatFeedback = 0;

    float currentTargetTimer = 0;

    Material[] meshMaterials = new Material[0];

    public virtual void OnMovementDetect()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDangerDetect()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnDistanceDetect(Transform target, float distance)
    {
        //throw new System.NotImplementedException();
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        entityData = entityData as T;

        //InitColor();
    }

    protected void InitColor()
    {
        meshMaterials = new Material[transform.GetChild(0).childCount];
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>())
            {
                meshMaterials[i] = transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material;
            }
        }
    }

    public void AddStun(float ammount, float stunDuration)
    {
        if (!isStun)
        {
            currentStunLevel += ammount;
            if (currentStunLevel > entityData.stunResistanceJauge)
            {
                IsStun(stunDuration);
            }
        }
    }

    public override void TakeDamage(float value)
    {
        base.TakeDamage(value);
        timeRemainingInMatFeedback += entityData.matChangeTime;
    }

    protected virtual void IsStun(float stunDuration)
    {
        currentStunLevel = 0;
        timeRemaingingStun = stunDuration;
        isStun = true;
    }

    protected virtual void ChangeColor(bool isOtherMat)
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>())
            {
                transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material = isOtherMat ? entityData.matWhenTakeDammage : meshMaterials[i];
            }
        }
    }

    protected virtual void Update()
    {


        if (timeRemainingInMatFeedback >= 0) timeRemainingInMatFeedback -= Time.unscaledDeltaTime;
        timeRemainingInMatFeedback = Mathf.Clamp(timeRemainingInMatFeedback, 0, 1);
        //ChangeColor(timeRemainingInMatFeedback > 0);

        if (timeRemaingingStun > 0)
        {
            timeRemaingingStun -= Time.deltaTime;
            if (timeRemaingingStun <= 0)
            {
                isStun = false;
                StopStun();
            }
        }

        if (!entityData.stayLockedOnTarget)
        {
            if (target) currentTargetTimer += Time.deltaTime;

            if (currentTargetTimer > entityData.timeBeforeCheckForAnotherTarget)
            {
                currentTargetTimer -= entityData.timeBeforeCheckForAnotherTarget;
                target = null;
            }
        }

        timerCheckTarget += Time.deltaTime;
        if (timerCheckTarget > checkEvery)
        {
            timerCheckTarget -= checkEvery;
            if (target == null)
                CheckForTargets();
        }

        if (target != null && !target.gameObject.activeSelf) target = null;
        
       

    }

    protected virtual void StopStun()
    {
        
    }

    protected override void Die()
    {
        base.Die();

        FxManager.Instance.PlayFx(entityData.fxWhenDie, transform.position, Quaternion.identity);
    }

    protected void CheckForTargets()
    {
        //Recherche de cible à attaquer
        enemies =  TeamsManager.Instance.GetAllEnemiesFromTeam(this.entityData.team, new int[]{2});
        if (enemies.Count > 0)
        {
            distanceToClosest = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(enemies[0].position.x, enemies[0].position.z));
            possibleTarget = enemies[0];

            if (enemies.Count > 1)
            {
                for (int i = 1; i < enemies.Count; i++)
                {
                    float distanceTemp = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(enemies[i].position.x, enemies[i].position.z));
                    if (distanceTemp < distanceToClosest)
                    {
                        distanceToClosest = distanceTemp;
                        possibleTarget = enemies[i];
                    }
                }
            }

            OnDistanceDetect(possibleTarget, distanceToClosest);
        }
    }

    public virtual void OnHit(DataWeaponMod mod, Vector3 position)
    {

    }

    public virtual void OnHitShotGun()
    {
        Weapon.Instance.OnShotGunHitTarget();
    }

    public virtual void OnHitSingleShot()
    {

    }

    public virtual void OnBulletClose()
    {

    }

    public virtual void OnCursorClose()
    {

    }
}
