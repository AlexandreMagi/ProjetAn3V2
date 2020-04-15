using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Swarmer : Enemy<DataSwarmer>, IGravityAffect, ISpecialEffects
{
    private enum SwarmerState
    {
        FollowPath,
        LookingForTarget,
        HuntTarget,
        WaitingForAttack,
        Attacking,
        DodgingObstacle,
        CalculatingExit,
        GravityControlled,
        PlayingAnimation
    }
    //Basics
    Rigidbody rbBody = null;
    [ShowInInspector]
    SwarmerState currentState = SwarmerState.FollowPath;

    //Animation
    [SerializeField]
    bool playsAnimationOnStartUp = false;

    [SerializeField]
    SwarmerProceduralAnimation.AnimSwarmer animationToPlay;

    //Securities
    float timeBeingStuck = 0;

    //Path follow
    [ShowInInspector]
    Vector3 currentFollowPoint = Vector3.zero;
    [ShowInInspector]
    Pather pathToFollow = null;
    [ShowInInspector]
    int pathID = 0;

    //Attack variables
    float timerWait = 0;

    //Dodge phase variables
    float jumpElapsedTime = 0;
    bool isOutStepTwo = false;
    Vector3 oldForwardVector = Vector3.zero;
    Vector3 obstacleDodgePoint = Vector3.zero;

    //Obstruction gestion
    Vector3 lastKnownPosition = Vector3.zero;
    [SerializeField]
    LayerMask maskOfWall = default;

    //Gravity variables
    readonly float timePropel = .5f;
    float elapsedTime = 0;
    ParticleSystem currentParticleOrb = null;
    ParticleSystem currentOrbExplosion = null;
    ParticleSystem currentPullParticles = null;
    bool hasPlayedFxOnPull = false;

    //Death variables
    bool isDying = false;

    [Header("Animator")]
    [SerializeField] SwarmerProceduralAnimation animatorCustom = null;

    // Variable servant à changer la couleur du swarmer lors de l'attaque
    float attackStatePurcentage = 0;

    #region Stimulus
    public override void OnDistanceDetect(Transform p_target, float distance)
    {
        base.OnDistanceDetect(target, distance);

        if (distance < entityData.distanceToTargetEnemy)
        {
            target = p_target;
        }
    }

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        ReactSpecial<DataSwarmer, DataSwarmer>.DoProject(rbBody, explosionOrigin, explosionForce, explosionRadius, liftValue);
        ReactSpecial<DataSwarmer, DataSwarmer>.DoExplosionDammage(this, explosionOrigin, explosionDamage, explosionRadius);
        ReactSpecial<DataSwarmer, DataSwarmer>.DoExplosionStun(this, explosionOrigin, explosionStun, explosionStunDuration, explosionRadius);
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        ReactGravity<DataSwarmer>.DoPull(rbBody, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity<DataSwarmer>.DoFloat(rbBody, timeBeforeActivation, isSlowedDownOnFloat, floatTime, bIndependantFromTimeScale);

        Invoke("ReleaseFromFloat", floatTime);
    }

    public void ReleaseFromFloat()
    {
        currentState = SwarmerState.FollowPath;
    }

    public void OnGravityDirectHit()
    {
        ReactGravity<DataSwarmer>.DoFreeze(rbBody);
    }

    public void OnHold()
    {
        if (!currentParticleOrb)
            currentParticleOrb = FxManager.Instance.PlayFx(entityData.vfxToPlayWhenHoldByGrav, transform);
        if (currentParticleOrb && !currentParticleOrb.isEmitting)
            currentParticleOrb.Play();
    }

    public void OnPull(Vector3 position, float force)
    {
        bool isInTheAir = Physics.Raycast(transform.position, Vector3.down, entityData.rayCastRangeToConsiderAirbone, maskOfWall);

        currentState = SwarmerState.GravityControlled;

        ReactGravity<DataSwarmer>.DoPull(rbBody, position, force, isInTheAir);
        if (!hasPlayedFxOnPull)
        {
            hasPlayedFxOnPull = true;
            currentPullParticles = FxManager.Instance.PlayFx(entityData.vfxToPlayWhenPulledByGrav, transform);
        }
        animatorCustom.PlayAnim(SwarmerProceduralAnimation.AnimSwarmer.reset);
    }

    public void OnRelease()
    {
        ReactGravity<DataSwarmer>.DoUnfreeze(rbBody);
        if (currentParticleOrb)
        {
            currentParticleOrb.Stop();
        }
        currentOrbExplosion = FxManager.Instance.PlayFx(entityData.vfxToPlayWhenReleaseByGrav, transform);
    }

    public void OnZeroG()
    {
        ReactGravity<DataSwarmer>.DoSpin(rbBody);
        animatorCustom.PlayAnim(SwarmerProceduralAnimation.AnimSwarmer.reset);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform == target)
        {
            IEntity targetEntity = target.GetComponent<IEntity>();
            if (other.GetComponent<Player>() != null)
            {
                //PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.VendettaPrepare, Vector3.zero, this);
            }
            targetEntity.TakeDamage(entityData.damage);
            targetEntity.OnAttack(entityData.spriteToDisplayShield, entityData.spriteToDisplayLife);
            //health = 0;
            this.Die();
        }
    }

    public void OnManualActivation()
    {
        if(currentState == SwarmerState.PlayingAnimation)
        currentState = SwarmerState.LookingForTarget;
    }

    protected override void Die()
    {
        if (!isDying)
        {
            isDying = true;
            if (currentParticleOrb) currentParticleOrb.Stop();
            if (currentOrbExplosion) currentOrbExplosion.Stop();
            if (currentPullParticles) currentPullParticles.Stop();

            if (CameraHandler.Instance.GetDistanceWithCam(transform.position) > entityData.distanceMinWithCamToPlayVFX)
            {
                FxManager.Instance.PlayFx(entityData.fxWhenDieDecals, transform.position, Quaternion.identity);
            }
            FxManager.Instance.PlayFx(entityData.fxWhenDie, transform.position, Quaternion.identity);

            CameraHandler.Instance.AddShake(entityData.shakeOnDie, entityData.shakeOnDieTime);
            TeamsManager.Instance.RemoveFromTeam(this.transform, entityData.team);

            target = null;
            pathToFollow = null;
            currentFollowPoint = Vector3.zero;

            ParticleSystem[] releaseFx = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem fx in releaseFx)
            {
                if (fx.name == "VFXOrbRelease(Clone)")
                {
                    fx.Stop();
                }
            }

            if (this.transform.GetComponentInParent<Spawner>() != null)
            {
                this.transform.GetComponentInParent<Spawner>().ChildDied();
            }


            //Means it has been killed in some way and has not just attacked
            if (health <= 0)
            {
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Vendetta, this.transform.position, this);
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Kill, this.transform.position, this);
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.KillSwarmer, this.transform.position);

                if(currentState == SwarmerState.Attacking)
                {
                    MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.InExtremisKill);
                }
            }

            if (SequenceHandler.Instance != null)
                SequenceHandler.Instance.OnEnemyKill();

            if(entityData.spawnsPartsOnDeath)
                InstansiateDeadBody();

            CustomSoundManager.Instance.PlaySound("SE_Swarmer_Death", "Effect", null, 0.8f,false,1,0.3f);

            this.gameObject.SetActive(false);

        }

    }
    void InstansiateDeadBody()
    {
        GameObject deadBodyClone;
        deadBodyClone = Instantiate(entityData.deadBody, transform.position, transform.rotation);
        deadBodyClone.transform.parent = null;
    }
    #endregion

    // Start is called before the first frame update
    protected override void Start()
    {
        this.health = entityData.startHealth;
        rbBody = GetComponent<Rigidbody>();

        if (playsAnimationOnStartUp)
        {
            currentState = SwarmerState.PlayingAnimation;

            animatorCustom.PlayAnim(animationToPlay);
        }

        lastKnownPosition = transform.position;
        Invoke("MaybeGrunt", 1f);
    }

    protected override void Update()
    {
        base.Update();

        #region Securities
        //Kill security
        if (this.transform.position.y <= -40)
        {
            this.Die();
        }

        //Rotation security
        this.transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);

        if (currentState != SwarmerState.PlayingAnimation)
        {
            //Blocking security
            #region BlockGestion
            timeBeingStuck += Time.deltaTime;

            if (timeBeingStuck >= entityData.initialTimeToConsiderCheck)
            {
                if (Vector3.Distance(transform.position, lastKnownPosition) <= entityData.considerStuckThreshhold)
                {

                    if (timeBeingStuck >= entityData.maxBlockedRetryPathTime && currentState == SwarmerState.DodgingObstacle)
                    {
                        currentState = SwarmerState.FollowPath;
                    }

                    if (timeBeingStuck >= entityData.maxBlockedSuicideTime && currentState != SwarmerState.GravityControlled)
                    {
                        this.Die();
                    }
                }
                else
                {
                    timeBeingStuck = 0;
                    lastKnownPosition = transform.position;
                }
            }
            #endregion
            #endregion

            //Distance to attack check
            if (target != null && CheckDistance() && Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 0.5f) && transform.position.y < target.position.y + 1 && currentState != SwarmerState.GravityControlled && currentState != SwarmerState.Attacking)
            {
                if (currentState != SwarmerState.WaitingForAttack)
                    animatorCustom.PlayAnim(SwarmerProceduralAnimation.AnimSwarmer.prepare);
                currentState = SwarmerState.WaitingForAttack;
                rbBody.velocity = Vector3.zero;
            }

            if (jumpElapsedTime > 0)
            {
                jumpElapsedTime -= Time.deltaTime;
                if (jumpElapsedTime < 0) jumpElapsedTime = 0;
            }
        }

        
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        //Base vectors
        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized * entityData.sideDetectionSight;
        Vector3 left = transform.TransformDirection(Vector3.left).normalized * entityData.sideDetectionSight;
        Vector3 right = transform.TransformDirection(Vector3.right).normalized * entityData.sideDetectionSight;
        Vector3 adaptedPosition = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);

        //State comportement
        switch (currentState)
        {
            #region FollowPath
            case SwarmerState.FollowPath:
                //Debug ray
                Debug.DrawRay(transform.position + Vector3.up * .5f, currentFollowPoint - (transform.position + Vector3.up * .5f), Color.cyan);

                if(pathToFollow != null && currentFollowPoint != Vector3.zero)
                {
                    //Displacement
                    MoveTowardsTarget(currentFollowPoint);

                    //Path verifications
                    if (CheckObjectiveDistance() && CheckObjectiveAngle())
                    {
                        //Advance in the path
                        pathID++;

                        currentFollowPoint = pathToFollow.GetPathAt(pathID);

                        //If end of path
                        if (currentFollowPoint == Vector3.zero)
                        {
                            //Debug.Log("End of path");

                            currentState = SwarmerState.LookingForTarget;
                        }
                        else
                        {
                            //Variance in path definition
                            if(entityData.varianceInPath > 0)
                            {
                                currentFollowPoint = ApplyVarianceToPosition(currentFollowPoint, transform.position);
                            }
                            
                        }
                    }

                    //Obstacle dodge intelligence
                    if (entityData.hasDodgeIntelligence && CheckForObstacles())
                    {
                        currentState = SwarmerState.CalculatingExit;
                    }
                    
                }

                else
                {
                    //If no current direction
                    currentState = SwarmerState.LookingForTarget;
                }

                break;

            #endregion

            #region WaitingForAttack
            case SwarmerState.WaitingForAttack:

                //Waiting some time before attacking
                timerWait += Time.fixedDeltaTime;
                if (timerWait > entityData.waitDuration)
                {
                    timerWait = 0;
                    if (target != null && CheckDistance())
                    {
                        currentState = SwarmerState.Attacking;

                        //Start attack
                        rbBody.AddForce(Vector3.up * entityData.jumpForce, ForceMode.Impulse);
                        CustomSoundManager.Instance.PlaySound("SE_Swarmer_Attack", "Effect", null, 0.4f, false, 1, 0.3f);
                        animatorCustom.PlayAnim(SwarmerProceduralAnimation.AnimSwarmer.jump);
                    }
                    else
                        currentState = SwarmerState.FollowPath;

                }
                else
                {
                    if (target != null)
                    {
                        //Rotation
                        Quaternion look = Quaternion.LookRotation(new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position);

                        transform.rotation = Quaternion.Slerp(transform.rotation, look, 5 * Time.fixedDeltaTime);
                    }
                    else
                    {
                        currentState = SwarmerState.LookingForTarget;
                    }
                }
                break;
            #endregion

            #region Attacking
            case SwarmerState.Attacking:
                if (target != null)
                {
                    MoveTowardsTarget(target.position, entityData.speedMultiplierWhenAttacking);

                    if (!CheckDistance())
                    {
                        currentState = SwarmerState.LookingForTarget;
                        animatorCustom.PlayAnim(SwarmerProceduralAnimation.AnimSwarmer.reset);
                    }
                }
                break;
            #endregion

            #region LookingForTarget
            case SwarmerState.LookingForTarget:
                if(target != null)
                {
                    currentState = SwarmerState.HuntTarget;
                }
                break;
            #endregion

            #region HuntTarget
            case SwarmerState.HuntTarget:
                if(target != null)
                {
                    MoveTowardsTarget(target.position);

                    if (entityData.hasDodgeIntelligence && CheckForObstacles())
                    {
                        currentState = SwarmerState.CalculatingExit;
                    }
                }
                else
                {
                    currentState = SwarmerState.LookingForTarget;
                }

                break;
            #endregion

            #region CalculatingExit
            case SwarmerState.CalculatingExit:

                Debug.DrawRay(adaptedPosition, Vector3.up, Color.green);
                Debug.DrawRay(adaptedPosition, (Vector3.up + forward) * entityData.jumpHeight, Color.red);

                //Vérification de la possibilité du saut
                if (
                    !Physics.Raycast(adaptedPosition, Vector3.up, out _, entityData.jumpHeight, maskOfWall) &&
                    !Physics.Raycast(adaptedPosition, (Vector3.up + forward) * entityData.jumpHeight, out _, entityData.jumpHeight + 1, maskOfWall) &&
                    jumpElapsedTime == 0
                   )
                {
                    jumpElapsedTime = entityData.jumpCooldownInitial;
                    rbBody.AddForce(Vector3.up * entityData.jumpDodgeForce);

                    currentState = SwarmerState.FollowPath;
                    //Debug.Log("jump");
                }
                //Si saut impossible, lancement de la manoeuvre d'évitement d'obstacle
                else
                {
                    if (jumpElapsedTime == 0)
                    {
                        //CANNOT JUMP, MUST DODGE OBSTACLE
                        bool hasFoundExit = false;
                        float currentStep = 1;
                        int iStep = 1;
                        bool isRightSide = true;

                        //Check for path tries
                        for (int i = 0; i < entityData.numberOfSideTries * 2; i++)
                        {
                            isRightSide = !isRightSide;
                            if (i % 2 == 0)
                            {
                                iStep++;
                            }

                            Vector3 rayInitialPosition = adaptedPosition + (isRightSide ? right : left) * iStep * entityData.tryStep;
                            Debug.DrawRay(rayInitialPosition, forward, Color.cyan);

                            if (!Physics.Raycast(rayInitialPosition, forward, out _, entityData.frontalDetectionSight + entityData.extraLengthByStep * iStep, maskOfWall) && !Physics.Raycast(adaptedPosition, (isRightSide ? right : left), out _, iStep * entityData.distanceDodgeStep, maskOfWall))
                            {
                                currentStep = iStep;
                                hasFoundExit = true;

                                break;
                            }
                        }

                        if (hasFoundExit)
                        {
                            //DO MOVE
                            //Debug.Log("Sortie trouvée");
                            isOutStepTwo = false;
                            oldForwardVector = forward + forward * entityData.extraLengthByStep * currentStep;
                            currentState = SwarmerState.DodgingObstacle;
                            obstacleDodgePoint = adaptedPosition + (isRightSide ? right : left) * currentStep * entityData.tryStep;
                        }
                        else
                        {
                            currentState = SwarmerState.FollowPath;
                        }
                    }
                }
                break;
            #endregion

            #region DodgingObstacle
            case SwarmerState.DodgingObstacle:
                MoveTowardsTarget(obstacleDodgePoint);

                bool moveRight = false;

                //Rotation
                Quaternion lookDirection = Quaternion.LookRotation(new Vector3(obstacleDodgePoint.x, transform.position.y, obstacleDodgePoint.z) - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, 5 * Time.fixedDeltaTime);

                //dodge to the right
                Debug.DrawRay(adaptedPosition + left * .5f, forward * entityData.sideDetectionSight, Color.magenta);
                if (Physics.Raycast(adaptedPosition + left * .5f, forward, out _, entityData.sideDetectionSight, maskOfWall))
                {
                    moveRight = true;
                }

                //dodge to the left
                Debug.DrawRay(adaptedPosition + right * .5f, forward * entityData.sideDetectionSight, Color.magenta);
                if (Physics.Raycast(adaptedPosition + right * .5f, forward, out _, entityData.sideDetectionSight, maskOfWall))
                {
                    if (!moveRight)
                        rbBody.AddForce(left * entityData.dodgeSlideForce);
                }
                else if (moveRight)
                {
                    rbBody.AddForce(right * entityData.dodgeSlideForce);
                }



                //Si on atteint l'étape de l'évitement
                if (Vector3.Distance(transform.position, obstacleDodgePoint) <= entityData.distanceDodgeStep)
                {
                    if (isOutStepTwo)
                    {
                        currentState = SwarmerState.FollowPath;
                        isOutStepTwo = false;
                        //Debug.Log("End of dodge step");
                    }
                    else
                    {
                        obstacleDodgePoint += oldForwardVector;
                        isOutStepTwo = true;
                        //Debug.Log("Step two");
                    }

                }
                break;
            #endregion

            #region GravityControlled
            case SwarmerState.GravityControlled:
                elapsedTime += Time.fixedDeltaTime;

                if (elapsedTime >= timePropel)
                {
                    ReactGravity<DataSwarmer>.DoSpin(rbBody);
                }
                break;
            #endregion

            #region PlayingAnimation
            case SwarmerState.PlayingAnimation:
                
                //Well.. Plays animation..

                break;
            #endregion

            default:
                break;
        }

        attackStatePurcentage = Mathf.MoveTowards(attackStatePurcentage, (currentState == SwarmerState.Attacking) ? 1 : 0, Time.fixedDeltaTime / entityData.timeToChangeColorWhileAttacking);
       
    }

    public void ResetSwarmer(DataEntity _entityData)
    {
        ParticleSystem[] releaseFx = GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem fx in releaseFx)
        {
            if (fx.name == "VFXOrbRelease(Clone)")
            {
                fx.Stop();
            }
        }
        isDying = false;
        entityData = _entityData as DataSwarmer;
        timeBeingStuck = 0;
        lastKnownPosition = transform.position;
        health = entityData.startHealth;
        TeamsManager.Instance.RemoveFromTeam(this.transform, entityData.team);
        TeamsManager.Instance.RegistertoTeam(this.transform, entityData.team);
        this.GetComponentInChildren<Renderer>().material = entityData.mat;
        target = null;
        currentState = SwarmerState.FollowPath;
        timerWait = 0;
        rbBody = GetComponent<Rigidbody>();
        rbBody.velocity = Vector3.zero;
        if (currentParticleOrb) currentParticleOrb.Stop();
        hasPlayedFxOnPull = false;

        if (Random.Range(0, 100) < 30)
            CustomSoundManager.Instance.PlaySound("SE_Swarmer_Spawn", "Effect",null,.3f,false,1,.3f);
        Invoke("MaybeGrunt", 1f);

        animatorCustom.PlayAnim(SwarmerProceduralAnimation.AnimSwarmer.reset);
        //InitColor();
    }


    public void SetPathToFollow(Pather path)
    {
        pathToFollow = path;

        pathID = 0;

        if (pathToFollow)
        {
            currentFollowPoint = pathToFollow.GetPathAt(0);
        }
        else
        {
            currentState = SwarmerState.LookingForTarget;
        }
    }

    void MoveTowardsTarget(Vector3 p_target, float speedMultiplier = 1f)
    {
        //Direction
        Vector3 direction = (new Vector3(p_target.x, transform.position.y, p_target.z) - transform.position).normalized;

        bool isInTheAir = Physics.Raycast(transform.position, Vector3.down, entityData.rayCastRangeToConsiderAirbone, maskOfWall);

        rbBody.AddForce(direction * entityData.speed + Vector3.up * Time.fixedDeltaTime * entityData.upScale * (isInTheAir ? .2f : 1) * speedMultiplier);
        //transform.Translate(direction * entityData.speed * Time.deltaTime * (isInTheAir ? .2f : 1), Space.World);


        //Debug
        Debug.DrawRay(transform.position, direction, Color.red);


        //Rotation
        Quaternion lookDirection = Quaternion.LookRotation(new Vector3(p_target.x, transform.position.y, p_target.z) - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, 5 * Time.fixedDeltaTime);
    }

    bool CheckObjectiveDistance()
    {
        return Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(currentFollowPoint.x, currentFollowPoint.z)) < entityData.distanceBeforeNextPath;
    }

    bool CheckObjectiveAngle()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized;
        //Debug.Log(Vector2.Angle(new Vector2(forward.x, forward.z), new Vector2(currentFollowPoint.x, currentFollowPoint.z)));

        return (Mathf.Abs(Vector2.Angle(new Vector2(forward.x, forward.z), new Vector2(currentFollowPoint.x - transform.position.x, currentFollowPoint.z - transform.position.z))) < entityData.angleToIgnorePath);
    }

    bool CheckDistance()
    {
        if (Vector3.Distance(transform.position, target.position) < entityData.distanceBeforeAttack)
            return true;
        else
            return false;
    }

    Vector3 ApplyVarianceToPosition(Vector3 posToVariate, Vector3 posBefore)
    {
        //Debug.Log($"Position to variate : {posToVariate} - Position to work with : {posBefore}");

        //Vectors of direction
        Vector3 direction = (posToVariate - posBefore);
        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized * entityData.sideDetectionSight;
        Vector3 left = transform.TransformDirection(Vector3.left).normalized * entityData.sideDetectionSight;
        Vector3 right = transform.TransformDirection(Vector3.right).normalized * entityData.sideDetectionSight;
        Vector3 initialPositionOfRayLeft = transform.position + Vector3.up * .5f + forward + left * .5f;
        Vector3 initialPositionOfRayRight = transform.position + Vector3.up * .5f + forward + right * .5f;

        float varianceAngle;
        Vector3 angledDirection;

        //DeathLock prevention
        int varianceIterations = 0;
        int maxIterations = 5;
        bool loopBroken = false;

        do
        {
            //Debug.Log("Loop");

            varianceIterations++;
            varianceAngle = Random.Range(-entityData.varianceInPath, entityData.varianceInPath);
            angledDirection = Quaternion.AngleAxis(varianceAngle, Vector3.up) * direction;

            if (varianceIterations > maxIterations)
            {
                loopBroken = true;
                break;
            };

        } while (!(Physics.Raycast(transform.position, posBefore+angledDirection - initialPositionOfRayLeft, 50f, maskOfWall) || Physics.Raycast(transform.position, posBefore+angledDirection - initialPositionOfRayRight, 50f, maskOfWall)));

        return posBefore + (loopBroken?Vector3.zero:angledDirection);
    }

    bool CheckForObstacles()
    {
        //Basic vectors
        float angle;
        Vector3 forward = transform.TransformDirection(Vector3.forward).normalized * entityData.sideDetectionSight;
        Vector3 adaptedPosition = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);

        //Debug ray
        Debug.DrawRay(adaptedPosition, forward, Color.blue);

        if (currentState != SwarmerState.HuntTarget)
            angle = Vector3.Angle(forward, currentFollowPoint - transform.position);
        else
            angle = Vector3.Angle(forward, target.position - transform.position);

        return (Physics.Raycast(adaptedPosition, forward, out _, entityData.frontalDetectionSight, maskOfWall) && angle <= 10);
    }

    void MaybeGrunt()
    {
        if (gameObject.activeSelf)
        {
            if (Random.Range(0, 100) < 5)
                CustomSoundManager.Instance.PlaySound("SE_Swarmer_Grunt", "Effect", null, .5f, false, 1, .3f);
            Invoke("MaybeGrunt", 1f);
        }
    }

    public float GetDamage()
   {
        return entityData.damage;
   }
}
