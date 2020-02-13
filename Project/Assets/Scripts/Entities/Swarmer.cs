using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class Swarmer : Enemy<DataSwarmer>, IGravityAffect, ISpecialEffects
{
    bool isAirbone = false;
    float timePropel = .5f;
    float elapsedTime = 0;
    float timerWait = 0;

    float jumpElapsedTime = 0;
    bool isGettingOutOfObstacle = false;
    bool isOutStepTwo = false;
    Vector3 obstacleDodgePoint = Vector3.zero;
    Vector3 oldForwardVector = Vector3.zero;

    float timeBeingStuck = 0;
    Vector3 lastKnownPosition = Vector3.zero;

    [SerializeField]
    LayerMask maskOfWall;

    [SerializeField]
    GameObject deadBody = null;

    Pather pathToFollow = null;
    [ShowInInspector]
    int pathID = 0;

    [ShowInInspector]
    Transform currentFollow;

    Vector3 v3VariancePoisitionFollow;

    Rigidbody rbBody;

    [ShowInInspector]
    bool isChasingTarget;

    enum State { Basic, Waiting, Attacking };
    [ShowInInspector]
    State nState = State.Basic;

    ParticleSystem currentParticleOrb = null;
    bool hasPlayedFxOnPull = false;

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
        timeBeingStuck = 0;
        lastKnownPosition = transform.position;
        health = entityData.startHealth;
        TeamsManager.Instance.RemoveFromTeam(this.transform, entityData.team);
        TeamsManager.Instance.RegistertoTeam(this.transform, entityData.team);
        this.GetComponentInChildren<Renderer>().material = entityData.mat;
        target = null;
        isChasingTarget = false;
        nState = (int)State.Basic;
        timerWait = 0;
        rbBody = GetComponent<Rigidbody>();
        rbBody.velocity = Vector3.zero;
        if (currentParticleOrb) currentParticleOrb.Stop();
        hasPlayedFxOnPull = false;
        //InitColor();
    }

    public void OnHold()
    {
        if (!currentParticleOrb)
            currentParticleOrb = FxManager.Instance.PlayFx(entityData.vfxToPlayWhenHoldByGrav, transform);
        if (currentParticleOrb && !currentParticleOrb.isEmitting)
            currentParticleOrb.Play();
        //Nothing happens on hold
    }

    public void OnPull(Vector3 origin, float force)
    {
        ReactGravity<DataSwarmer>.DoPull(rbBody, origin, force, isAirbone);
        if (!hasPlayedFxOnPull)
        {
            hasPlayedFxOnPull = true;
            FxManager.Instance.PlayFx(entityData.vfxToPlayWhenPulledByGrav, transform);
        }
    }

    public void OnRelease()
    {
        ReactGravity<DataSwarmer>.DoUnfreeze(rbBody);
        if (currentParticleOrb) currentParticleOrb.Stop();
        FxManager.Instance.PlayFx(entityData.vfxToPlayWhenReleaseByGrav, transform);
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
    public override void OnHit(DataWeaponMod mod, Vector3 position)
    {
        this.TakeDamage(mod.bullet.damage);
    }
    #endregion

    #region Detection

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
        if (currentParticleOrb) currentParticleOrb.Stop();
        FxManager.Instance.PlayFx(entityData.fxWhenDie, transform.position, Quaternion.identity);

        CameraHandler.Instance.AddShake(entityData.shakeOnDie, entityData.shakeOnDieTime);
        TeamsManager.Instance.RemoveFromTeam(this.transform, entityData.team);
       
        target = null;
        pathToFollow = null;
        currentFollow = null;

        if(this.transform.GetComponentInParent<Spawner>() != null)
        {
            this.transform.GetComponentInParent<Spawner>().ChildDied();
        }
       

        //Means it has been killed in some way and has not just attacked
        if(health <= 0)
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Vendetta, Vector3.zero, this); 
        }

        if (SequenceHandler.Instance != null)
            SequenceHandler.Instance.OnEnemyKill();

        InstansiateDeadBody();

        this.gameObject.SetActive(false);

        //base.Die();        

    }

    void InstansiateDeadBody()
    {
        GameObject deadBodyClone;
        deadBodyClone = Instantiate(deadBody, transform.position, transform.rotation);
        deadBodyClone.transform.parent = null;

        //int rand;
        //int nbParts = 0;

        //Prop[] tList = deadBodyClone.GetComponentsInChildren<Prop>();
        //foreach (Prop t in tList)
        //{
        //    rand = Random.Range(0, 2);

        //    if (rand == 0)
        //        t.enabled = false;
        //    else if (rand == 1 && nbParts <= 2)
        //    {
        //        t.enabled = true;
        //        nbParts += 1;
        //    }
        //    else
        //        t.enabled = false;
        //}

        Rigidbody[] rbList = deadBodyClone.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in rbList)
        {
            rb.AddExplosionForce(1500f, deadBodyClone.transform.position, 1f);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.transform == target)
        {
            IEntity targetEntity = target.GetComponent<IEntity>();
            if(other.GetComponent<Player>() != null)
            {
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.VendettaPrepare, Vector3.zero, this);
            }
            targetEntity.TakeDamage(entityData.damage);
            targetEntity.OnAttack(entityData.spriteToDisplayShield, entityData.spriteToDisplayLife);
            //health = 0;
            this.Die();
        }
    }

    #endregion //STIMULUS

    // Start is called before the first frame update
    protected override void Start()
    {
        this.health = entityData.startHealth;
        //enemyData = entityData as DataSwarmer;
        rbBody = GetComponent<Rigidbody>();

        lastKnownPosition = transform.position;
        //TeamsManager.Instance.RegistertoTeam(this.transform, enemyData.team);
    }

    protected override void Update()
    {
        base.Update();

        if (this.transform.position.y <= -5)
        {
            this.Die();
        }

        timeBeingStuck += Time.deltaTime;

        if(timeBeingStuck >= entityData.initialTimeToConsiderCheck)
        {
            if (Vector3.Distance(transform.position, lastKnownPosition) <= entityData.considerStuckThreshhold)
            {
                if(timeBeingStuck >= entityData.maxBlockedRetryPathTime && isGettingOutOfObstacle)
                {
                    isGettingOutOfObstacle = false;
                }
                else
                {
                    pathID++;
                    if(pathToFollow != null)
                        currentFollow = pathToFollow.GetPathAt(pathID);
                    if (currentFollow == null)
                    {
                        pathID--;
                        isChasingTarget = true;
                        target = Player.Instance.transform;
                    }
                }

                if(timeBeingStuck >= entityData.maxBlockedSuicideTime && !isAirbone)
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

        
    }


    public void SetPathToFollow(Pather path)
    {
        pathToFollow = path;

        currentFollow = path.GetPathAt(0);

        pathID = 0;

        v3VariancePoisitionFollow = currentFollow.position;
    }

    protected virtual void FixedUpdate()
    {
        //Debug.DrawRay(transform.position, currentFollow.position, Color.magenta);

        if(jumpElapsedTime > 0)
        {
            jumpElapsedTime -= Time.fixedDeltaTime;

            if(jumpElapsedTime <= 0)
            {
                jumpElapsedTime = 0;
            }
        }

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
        else
        {
            this.transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
            //Ça, c'est le truc pour les rendre moins débilos
            //------------- JUMP OBSTACLES
            Vector3 forward = transform.TransformDirection(Vector3.forward) * entityData.frontalDetectionSight;
            Vector3 left = transform.TransformDirection(Vector3.left).normalized * entityData.sideDetectionSight;
            Vector3 right = transform.TransformDirection(Vector3.right).normalized * entityData.sideDetectionSight;
            Vector3 adaptedPosition = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
            Debug.DrawRay(adaptedPosition, forward, Color.blue);
            //Angle of ray compared to point of path
            float angle = 90;
            if(currentFollow)
                angle = Vector3.Angle(forward, currentFollow.position - transform.position);

            //Vérification frontale. Seulement valide si c'est "relativement" dans la direction où le mob veut aller.
            if (Physics.Raycast(adaptedPosition, forward, out _, entityData.frontalDetectionSight, maskOfWall) && angle <= 10)
            {
                //Debug.Log("Obstacle found.");
                Debug.DrawRay(adaptedPosition, Vector3.up, Color.green);
                Debug.DrawRay(adaptedPosition, (Vector3.up + forward) * entityData.jumpHeight, Color.red);

                //Vérification de la possibilité du saut
                if (
                    !Physics.Raycast(adaptedPosition, Vector3.up, out _, entityData.jumpHeight, maskOfWall) &&
                    !Physics.Raycast(adaptedPosition, (Vector3.up + forward) * entityData.jumpHeight, out _, entityData.jumpHeight+1, maskOfWall) &&
                    jumpElapsedTime == 0
                   )
                { 
                    jumpElapsedTime = entityData.jumpCooldownInitial;
                    rbBody.AddForce(Vector3.up * entityData.jumpDodgeForce);
                    //Debug.Log("jump");
                }
                //Si saut impossible, lancement de la manoeuvre d'évitement d'obstacle
                else
                {
                    if (!isGettingOutOfObstacle && jumpElapsedTime == 0)
                    {
                        //CANNOT JUMP, MUST DODGE OBSTACLE
                        bool hasFoundExit = false;
                        float currentStep = 1;
                        int iStep = 1;
                        bool isRightSide = true;
                        
                        //Check for path tries
                        for(int i = 0; i< entityData.numberOfSideTries * 2; i++)
                        {
                            isRightSide = !isRightSide;
                            if(i%2 == 0)
                            {
                                iStep++;
                            }

                            Vector3 rayInitialPosition = adaptedPosition + (isRightSide?right:left) * iStep * entityData.tryStep;
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
                            Debug.Log("Sortie trouvée");
                            isOutStepTwo = false;
                            oldForwardVector = forward + forward * entityData.extraLengthByStep * currentStep;
                            isGettingOutOfObstacle = true;
                            obstacleDodgePoint = adaptedPosition + (isRightSide ? right : left) * currentStep * entityData.tryStep;
                        }
                    }
                }
            }
        }

        if (isGettingOutOfObstacle)
        {
            //Movement
            Vector3 direction = (new Vector3(obstacleDodgePoint.x, transform.position.y, obstacleDodgePoint.z) - transform.position).normalized;
            rbBody.AddForce(direction * entityData.speed + Vector3.up * Time.fixedDeltaTime * entityData.upScale);

            bool moveRight = false;

            //Rotation
            Quaternion lookDirection = Quaternion.LookRotation(new Vector3(obstacleDodgePoint.x, transform.position.y, obstacleDodgePoint.z) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, 5 * Time.fixedDeltaTime);

            //Block check
            Vector3 forward = transform.TransformDirection(Vector3.forward).normalized * entityData.sideDetectionSight;
            Vector3 left = transform.TransformDirection(Vector3.left).normalized * entityData.sideDetectionSight;
            Vector3 right = transform.TransformDirection(Vector3.right).normalized * entityData.sideDetectionSight;
            Vector3 adaptedPosition = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);

            //dodge to the right
            Debug.DrawRay(adaptedPosition + left * .5f, forward * entityData.sideDetectionSight, Color.magenta);
            if (Physics.Raycast(adaptedPosition + left * .5f, forward, out _, entityData.sideDetectionSight, maskOfWall)) {
                moveRight = true;
            }

            //dodge to the left
            Debug.DrawRay(adaptedPosition + right * .5f, forward * entityData.sideDetectionSight, Color.magenta);
            if (Physics.Raycast(adaptedPosition + right * .5f, forward, out _, entityData.sideDetectionSight, maskOfWall))
            {
                if(!moveRight)
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
                    isGettingOutOfObstacle = false;
                    isOutStepTwo = false;
                    Debug.Log("End of dodge step");
                }
                else
                {
                    obstacleDodgePoint += oldForwardVector;
                    isOutStepTwo = true;
                    Debug.Log("Step two");
                }
                
            }
        }
        else { 
            //Pathfinding
            if ((currentFollow != null || target != null) && entityData != null && rbBody.useGravity && !isAirbone)
            {

                if (nState == State.Basic)
                {
                    if (isChasingTarget && target != null)
                    {
                        v3VariancePoisitionFollow = target.position;
                    }

                    //TODO : Follow the path
                    Vector3 direction = (new Vector3(v3VariancePoisitionFollow.x, transform.position.y, v3VariancePoisitionFollow.z) - transform.position).normalized;

                    rbBody.AddForce(direction * entityData.speed * (jumpElapsedTime > 0 ? .1f : 1) + Vector3.up * Time.fixedDeltaTime * entityData.upScale);


                    if (!isChasingTarget && pathToFollow != null)
                    {
                        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(v3VariancePoisitionFollow.x, v3VariancePoisitionFollow.z)) < entityData.distanceBeforeNextPath)
                        {
                            pathID++;
                            currentFollow = pathToFollow.GetPathAt(pathID);
                            if (currentFollow == null)
                            {
                                Debug.Log("End of path");
                                pathID--;
                                isChasingTarget = true;
                                target = Player.Instance.transform;
                            }

                            if (currentFollow != null && currentFollow != target)
                            {
                                //Debug.Log("Proc variance, variance = "+swarmer.varianceInPath+"%");
                                //Debug.Log("Variance = "+ (swarmer.varianceInPath / 100 * Random.Range(-2f, 2f)));

                                v3VariancePoisitionFollow = new Vector3(
                                    currentFollow.position.x + (entityData.varianceInPath / 100 * Random.Range(-2f, 2f)),
                                    currentFollow.position.y,
                                    currentFollow.position.z + (entityData.varianceInPath / 100 * Random.Range(-2f, 2f))
                                );

                                //Debug.Log("Initial pos X: " + currentFollow.position.x + " - Varied pos X : " + v3VariancePoisitionFollow.x);
                            }
                            else
                            {
                                currentFollow = target;
                            }

                        }
                        //Debug.Log("rotate");

                    }
                    if (target != null && CheckDistance() && Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 0.5f) && transform.position.y < target.position.y + 1)
                    {
                        nState = State.Waiting;
                        rbBody.velocity = Vector3.zero;
                        //GetComponent<Animator>().SetTrigger("PrepareToJump");
                    }
                }
                else if (nState == State.Waiting)
                {
                    timerWait += Time.deltaTime;
                    if (timerWait > entityData.waitDuration)
                    {
                        timerWait = 0;
                        if (target != null && CheckDistance())
                        {
                            nState = State.Attacking;
                            //GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
                            //GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.red);
                            rbBody.AddForce(Vector3.up * entityData.jumpForce, ForceMode.Impulse);
                            //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Swarmer_Attack", false, 0.4f, 0.3f);
                        }
                        else
                            nState = State.Basic;

                    }
                }
                else if (nState == State.Attacking)
                {
                    //TODO : Follow the path
                    if (target != null)
                    {
                        Vector3 direction = (new Vector3(target.position.x, transform.position.y, target.position.z) - transform.position).normalized;
                        rbBody.AddForce(direction * entityData.speed * entityData.speedMultiplierWhenAttacking + Vector3.up * Time.fixedDeltaTime * entityData.upScale);
                        if (!CheckDistance())
                        {
                            nState = State.Basic;
                            //GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", Color.Lerp(Color.yellow, Color.red, 0.5f));
                            //GetComponentInChildren<MeshRenderer>().material.SetColor("_EmissionColor", Color.Lerp(Color.yellow, Color.red, 0.5f));
                        }
                    }
                }

                //Rotation
                Quaternion lookDirection = Quaternion.LookRotation(new Vector3(v3VariancePoisitionFollow.x, transform.position.y, v3VariancePoisitionFollow.z) - transform.position);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, 5 * Time.fixedDeltaTime);
            }
        }

        
    }

    bool CheckDistance()
    {
        if (Vector3.Distance(transform.position, target.position) < entityData.distanceBeforeAttack)
            return true;
        else
            return false;
    }

    public float GetDamage()
    {
        return entityData.damage;
    }
}
