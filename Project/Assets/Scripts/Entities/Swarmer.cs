﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Swarmer : Enemy<DataSwarmer>, IGravityAffect, IBulletAffect, ISpecialEffects
{

    bool isAirbone = false;
    float timePropel = .5f;
    float elapsedTime = 0;
    float timerWait = 0;

    Pather pathToFollow = null;
    int pathID = 0;

    Transform currentFollow;

    Vector3 v3VariancePoisitionFollow;

    Rigidbody rbBody;

    bool isChasingTarget;

    enum State { Basic, Waiting, Attacking };
    int nState = 0;


    public float _health = 100;
    public Transform _target = null;
    public int _team = 0;

    //Stimulus
    #region Stimulus
    #region Gravity
    public void OnGravityDirectHit()
    {
        ReactGravity<DataSwarmer>.DoFreeze(rbBody);
    }

    public void ResetSwarmer(DataEntity _entityData)
    {
        entityData = _entityData as DataSwarmer;
        health = entityData.startHealth;
        TeamsManager.Instance.RegistertoTeam(this.transform, entityData.team);
        this.GetComponentInChildren<Renderer>().material = entityData.mat;
        target = null;
        isChasingTarget = false;
        nState = (int)State.Basic;
        timerWait = 0;
        rbBody = GetComponent<Rigidbody>();
        rbBody.velocity = Vector3.zero;
    }

    public void OnHold()
    {
        //Nothing happens on hold
    }

    public void OnPull(Vector3 origin, float force)
    {
        ReactGravity<DataSwarmer>.DoPull(rbBody, origin, force, isAirbone);
    }

    public void OnRelease()
    {
        ReactGravity<DataSwarmer>.DoUnfreeze(rbBody);
    }

    public void OnZeroG()
    {
        ReactGravity<DataSwarmer>.DoSpin(rbBody);
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        ReactGravity<DataSwarmer>.DoPull(rbBody, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity<DataSwarmer>.DoFloat(rbBody, timeBeforeActivation, isSlowedDownOnFloat, tFloatTime, bIndependantFromTimeScale);
    }
    #endregion

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        ReactSpecial<DataSwarmer, DataSwarmer>.DoProject(rbBody, explosionOrigin, explosionForce, explosionRadius, liftValue);
        ReactSpecial<DataSwarmer, DataSwarmer>.DoExplosionDammage(this, explosionOrigin, explosionDamage, explosionRadius);
        ReactSpecial<DataSwarmer, DataSwarmer>.DoExplosionStun(this, explosionOrigin, explosionStun, explosionStunDuration, explosionRadius);
    }

    #region Bullets
    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
        this.TakeDamage(mod.bullet.damage);
    }

    public void OnHitShotGun()
    {
        
    }

    public void OnHitSingleShot()
    {
        
    }

    public void OnBulletClose()
    {
        throw new System.NotImplementedException();
    }

    public void OnCursorClose()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Detection

    protected override void Update()
    {
        base.Update();
        _health = health;
        _target = target;
        _team = entityData.team;

        if (this.transform.position.y <= -5)
        {
            this.Die();
        }
    }

    public override void OnMovementDetect()
    {
      
    }

    public override void OnDangerDetect()
    {
      
    }

    public override void OnDistanceDetect(Transform targetToHunt, float distance)
    {
        if (pathToFollow == null || distance < entityData.distanceToTargetEnemy)
        {
            isChasingTarget = true;
            target = targetToHunt;
            currentFollow = target;
        }
        
    }
    #endregion
    protected override void Die()
    {
        FxManager.Instance.PlayFx("VFX_Death", transform.position, Quaternion.identity);

        TeamsManager.Instance.RemoveFromTeam(this.transform, entityData.team);
        this.gameObject.SetActive(false);
        target = null;
        pathToFollow = null;
        currentFollow = null;
        //base.Die();        

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.transform == target)
        {
            target.GetComponent<Entity<DataSwarmer>>().TakeDamage(entityData.damage);
            this.Die();
        }
    }

    #endregion
    // Start is called before the first frame update
    protected override void Start()
    {
        //base.Start();
        //enemyData = entityData as DataSwarmer;
        //rbBody = GetComponent<Rigidbody>();
        //TeamsManager.Instance.RegistertoTeam(this.transform, enemyData.team);
    }

    public void SetPathToFollow(Pather path)
    {
        pathToFollow = path;

        currentFollow = path.GetPathAt(0);

        v3VariancePoisitionFollow = currentFollow.position;
    }

    protected virtual void FixedUpdate()
    {

        //Check for airbone and makes it spin if in the air
        if (isAirbone)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timePropel)
            {
                ReactGravity<DataSwarmer>.DoSpin(rbBody);

                //Check si touche le sol
                elapsedTime = 0;
                if (Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 1f))
                {
                    isAirbone = false;
                }

            }

        }


        //Pathfinding
        if (currentFollow != null && entityData != null && rbBody.useGravity && !isAirbone)
        {

            if (nState == (int)State.Basic)
            {
                if (isChasingTarget && target != null)
                {
                    v3VariancePoisitionFollow = target.position;
                }

                //TODO : Follow the path
                Vector3 direction = (new Vector3(v3VariancePoisitionFollow.x, transform.position.y, v3VariancePoisitionFollow.z) - transform.position).normalized;

                rbBody.AddForce(direction * entityData.speed + Vector3.up * Time.fixedDeltaTime * entityData.upScale);


                if (!isChasingTarget && pathToFollow != null)
                {
                    if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(v3VariancePoisitionFollow.x, v3VariancePoisitionFollow.z)) < entityData.fDistanceBeforeNextPath)
                    {
                        currentFollow = pathToFollow.GetPathAt(pathID++);
                        if (currentFollow == null) pathID--;

                        if (currentFollow != null && currentFollow != target)
                        {
                            //Debug.Log("Proc variance, variance = "+swarmer.nVarianceInPath+"%");
                            //Debug.Log("Variance = "+ (swarmer.nVarianceInPath / 100 * Random.Range(-2f, 2f)));

                            v3VariancePoisitionFollow = new Vector3(
                                currentFollow.position.x + (entityData.nVarianceInPath / 100 * Random.Range(-2f, 2f)),
                                currentFollow.position.y,
                                currentFollow.position.z + (entityData.nVarianceInPath / 100 * Random.Range(-2f, 2f))
                            );

                            //Debug.Log("Initial pos X: " + currentFollow.position.x + " - Varied pos X : " + v3VariancePoisitionFollow.x);
                        }
                        else
                        {
                            currentFollow = target;
                        }

                    }
                    
                }
                if (target != null && CheckDistance() && Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 0.5f) && transform.position.y < target.position.y + 1)
                {
                    nState = (int)State.Waiting;
                    rbBody.velocity = Vector3.zero;
                    //GetComponent<Animator>().SetTrigger("PrepareToJump");
                }
            }
            else if (nState == (int)State.Waiting)
            {
                timerWait += Time.deltaTime;
                if (timerWait > entityData.fWaitDuration)
                {
                    timerWait = 0;
                    if (target != null && CheckDistance())
                    {
                        nState = (int)State.Attacking;
                        //GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
                        //GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);
                        rbBody.AddForce(Vector3.up * entityData.fJumpForce, ForceMode.Impulse);
                        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Swarmer_Attack", false, 0.4f, 0.3f);
                    }
                    else
                        nState = (int)State.Basic;

                }
            }
            else if (nState == (int)State.Attacking)
            {
                //TODO : Follow the path
                if (target != null)
                {
                    Vector3 direction = (new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position).normalized;
                    rbBody.AddForce(direction * entityData.speed * entityData.fSpeedMultiplierWhenAttacking + Vector3.up * Time.fixedDeltaTime * entityData.upScale);
                    if (!CheckDistance())
                    {
                        nState = (int)State.Basic;
                        //GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", Color.Lerp(Color.yellow, Color.red, 0.5f));
                        //GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(Color.yellow, Color.red, 0.5f));
                    }
                }
            }
        }
    }

    bool CheckDistance()
    {
        if (Vector3.Distance(transform.position, target.position) < entityData.fDistanceBeforeAttack)
            return true;
        else
            return false;
    }
}
