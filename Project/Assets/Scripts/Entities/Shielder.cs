using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : Enemy<DataShielder>, IGravityAffect
{
    List<Transform> allies;

    float timeLeftForShieldApply;
    bool mustFollowTarget;

    bool willDodge = false;

    float timeBeforeRecoveryEnd = 0;

    Rigidbody rbBody;

    #region State
    public enum ShielderState
    {
        LookingForTarget,
        FollowingTarget,
        CastingShield,
        ProtectingTarget,
        RecoveryDodge,
        Dying
    }

    private ShielderState currentState;



    #endregion

    protected override void Start()
    {
        base.Start();
        rbBody = GetComponent<Rigidbody>();
        currentState = ShielderState.LookingForTarget;
    }

    #region Stimulus

    #region Gravity
    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        
    }

    public void OnZeroGRelease()
    {
        //Nothing happens on hold
    }

    public void OnGravityDirectHit()
    {
        
    }

    public void OnHold()
    {
        
    }

    public void OnPull(Vector3 position, float force, bool fleesPlayer = false)
    {
        
    }

    public void OnRelease()
    {
        
    }

    public void OnZeroG()
    {
        
    }
    #endregion //Gravity

    #region Detection

    public override void OnDistanceDetect(Transform target, float distance)
    {
        if(distance < entityData.distanceToStartFollowingAlly)
        {
            this.target = target;
        }
    }

    public override void OnCursorClose(Vector3 positionOfCursor)
    {
        Debug.Log("Cursor is close !");

        //Position comparaison || Comparaison to screen
        Vector2 thisObjectPos = CameraHandler.Instance.renderingCam.WorldToScreenPoint(this.transform.position);
        Vector2 hitPos = CameraHandler.Instance.renderingCam.WorldToScreenPoint(positionOfCursor);

        Vector2 directionToFlee = (thisObjectPos - hitPos).normalized;

        if (!willDodge && currentState != ShielderState.RecoveryDodge)
        {
            StartCoroutine(Dodge(directionToFlee));
            willDodge = true;
        }

    }

    #endregion //Detection

    #endregion //Stimulus

    protected override void Update()
    {
        float distanceToTarget = 0;
        if(target != null)
        {
            distanceToTarget = Vector2.Distance(new Vector2(target.transform.position.x, target.transform.position.z), new Vector2(this.transform.position.x, this.transform.position.z));
        }

        Fly();

        /////// STUN
        #region StunAndFeedback
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
        #endregion //StunFeed


        ////// LOOKING FOR TARGET STATE
        #region LookingForTarget
        if (currentState == ShielderState.LookingForTarget)
        {
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
        #endregion //LFT    


        ////// CASTING SHIELD STATE
        #region CastingShield
        if(currentState == ShielderState.CastingShield)
        {
            //transform.LookAt(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - target.position), entityData.targetLockFollowSpeed * Time.deltaTime);
            

            if(timeLeftForShieldApply <= 0)
            {
                CastShieldOnTarget();
                //currentState = ShielderState.ProtectingTarget;
            }
            else
            {
                timeLeftForShieldApply -= Time.deltaTime;
            }

        }
        #endregion //Shield

        ////// DEAD STATE AND RETURN TO LFT
        if(currentState != ShielderState.Dying)
        {
            if (target != null)
            {
                if (currentState == ShielderState.LookingForTarget)
                {
                    //Debug.Log("Ally to shield found !");
                    currentState = ShielderState.FollowingTarget;
                }
            }
            else if (currentState != ShielderState.LookingForTarget)
            {
                currentState = ShielderState.LookingForTarget;
            }
        }
        else
        {
            #region Dying
            #endregion //Dying
        }

        /////// FOLLOWING STATE
        #region Follow
        if(currentState == ShielderState.FollowingTarget)
        {
            if (distanceToTarget <= entityData.shieldApplyRange)
            {
                //Debug.Log("In range for shield");
                currentState = ShielderState.CastingShield;
                timeLeftForShieldApply = entityData.timeShieldCast;
                mustFollowTarget = false;
            }
            else
            {
                transform.rotation = Quaternion.LookRotation(transform.position - target.position, Vector3.up);
                mustFollowTarget = true;

            }
        }
        #endregion //Follow

        /////// RECOVERY STATE
        #region Dodge
        if (currentState == ShielderState.RecoveryDodge)
        {
            if(timeBeforeRecoveryEnd <= 0)
            {
                currentState = ShielderState.LookingForTarget;
            }
            else
            {
                timeBeforeRecoveryEnd -= Time.deltaTime;
            }
        }
        #endregion

        /////// DISTANCE CHECK TO BREAK LINK
        if (distanceToTarget > entityData.distanceToStartFollowingAlly && currentState != ShielderState.FollowingTarget)
        {
            currentState = ShielderState.FollowingTarget;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            mustFollowTarget = false;
            //Debug.Log("Lost sight of target, following it now.");
        }
    }
     

    /// <summary>
    /// Fixed update. For movement
    /// </summary>
    public void FixedUpdate()
    {
        if (mustFollowTarget)
        {
            Vector3 direction = new Vector3(target.position.x - transform.position.x, 0, target.position.z - transform.position.z).normalized;
            rbBody.AddForce(direction * entityData.movementSpeed * Time.deltaTime);
        }
    }

    private void Fly()
    {
        //this.transform.position = Vector3.up;
        Vector3 displacement = new Vector3(0, Mathf.Cos(Time.time * entityData.floatSpeed) * entityData.floatAmplitude, 0);
        this.transform.Translate(displacement * Time.deltaTime, Space.World);
    }

    protected override void CheckForTargets()
    {
        //Recherche de cible à attaquer
        allies = TeamsManager.Instance.GetTeam(this.entityData.team);

        if (allies.Count > 0)
        {
            if (allies[0] != this.transform)
            {
                distanceToClosest = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(allies[0].position.x, allies[0].position.z));
                possibleTarget = allies[0];
            }
            else
            {
                distanceToClosest = entityData.distanceToStartFollowingAlly + 1;
            }

            if (allies.Count > 1)
            {
                for (int i = 1; i < allies.Count; i++)
                {
                    if (allies[i] != this.transform)
                    {
                        float distanceTemp = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(allies[i].position.x, allies[i].position.z));
                        if (distanceTemp < distanceToClosest)
                        {
                            distanceToClosest = distanceTemp;
                            possibleTarget = allies[i];
                        }
                    }

                }
            }

            OnDistanceDetect(possibleTarget, distanceToClosest);
        }
    }

    private void CastShieldOnTarget()
    {
        //Debug.Log("Shield applied");
    }

    private IEnumerator Dodge(Vector3 directionToFlee)
    {
        yield return new WaitForSecondsRealtime(entityData.timeBeforeDodgeStart);

        rbBody.AddForce(directionToFlee * entityData.dodgeSpeed);

        willDodge = false;
        timeBeforeRecoveryEnd = entityData.recoveryTime;
        currentState = ShielderState.RecoveryDodge;

        yield break;
    }

    protected override void Die()
    {
        currentState = ShielderState.Dying;

        rbBody.useGravity = true;
        rbBody.drag = 0;
        //rbBody.AddForce(new Vector3(0,1,0) * entityData.deathPropulsionForce);
    }

    private void TrueDeath()
    {
        base.Die();
    }
}
