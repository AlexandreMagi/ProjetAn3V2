﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[SelectionBase]
public class Shooter : Enemy<DataShooter>, ISpecialEffects, IGravityAffect
{
   // private DataShooter shooterData;

    //Stimulus
    #region Stimulus
    #region Bullets
    /*public override void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray rayShot)
    {
        this.TakeDamage(dammage);
    }*/

    #endregion

    public void OnExplosion(Vector3 explosionOrigin, float explosionForce, float explosionRadius, float explosionDamage, float explosionStun, float explosionStunDuration, float liftValue = 0)
    {
        ReactSpecial<DataShooter, DataShooter>.DoExplosionDammage(this, explosionOrigin, explosionDamage, explosionRadius);
        ReactSpecial<DataShooter, DataShooter>.DoExplosionStun(this, explosionOrigin, explosionStun, explosionStunDuration, explosionRadius);
    }
    #endregion

    float considerPlayerIsMoving = 0;

    /// <summary>
    /// Différents états de l'ennemi
    /// </summary>
    enum State { Nothing, Rotating, Loading, Shooting, Stuned, Recovering };

    [Tooltip("Variable qui indique l'étât actuel de l'ennemi")]
    int state = 0;

    [Tooltip("Timer qui indique le temps passé à se préparer à charger")]
    float timerLoading = 0;
    [Tooltip("Timer qui indique le temps passé stun après avoir impacté le joueur")]
    float timerRecovering = 0;
    [Tooltip("Timer qui indique le avant le prochain tir dans la salve")]
    float timerbeforeNextAttack = 0;
    [Tooltip("Indique à quel tir on est dans la salve")]
    int bulletShot = 0;

    [SerializeField]
    GameObject canonPlacement = null;

    [SerializeField]
    float[] overrideBulletRotation = new float[0];

    bool playerMoving = false;

    bool canShoot = true;

    [SerializeField]
    Transform fxStunPos = null;

    List<ShooterBullet> allBullets = new List<ShooterBullet>();

    DataShooterBullet currentDataBullet = null;

    [SerializeField] Transform[] canonRoot = new Transform[0];
    [SerializeField] Vector3[] canonRootBasePos = new Vector3[0];
    [SerializeField] float distTravel = 1.2f;

    [SerializeField] Animator anmt = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state = (int)State.Nothing;
        entityData = entityData as DataShooter;

        //On crée un clone, en cas de modifications à la volée
        currentDataBullet = Instantiate(entityData.bulletData);
        canonRootBasePos = new Vector3[canonRoot.Length];
        for (int i = 0; i < canonRoot.Length; i++)
        {
            canonRootBasePos[i] = canonRoot[i].localPosition;
        }
    }

    public override void OnDistanceDetect(Transform possibleTarget, float distance)
    {
        base.OnDistanceDetect(possibleTarget, distance);
        if (possibleTarget && distance < entityData.distanceDetection && target == null)
        {
            target = possibleTarget;
            SpotTarget();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (considerPlayerIsMoving > 0)
        {
            considerPlayerIsMoving -= Time.deltaTime;
            if (considerPlayerIsMoving <= 0)
                playerMoving = false;
        }
        if (playerMoving && !entityData.shootEvenIfPlayerMoving)
        {
            if (!(state == (int)State.Stuned) && !(state == (int)State.Recovering))
            {
                ResetTimers();
                SpotTarget();
            }
        }

        if (target == null)
        {
            ResetTimers();
            state = (int)State.Nothing;
        }


        switch (state)
        {
            case (int)State.Nothing:
                break;
            case (int)State.Rotating:
                Vector3 vPos = new Vector3(target.position.x, transform.position.y, target.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(transform.position - vPos, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * entityData.rotationSpeed);

                anmt.SetTrigger("PrepareShoot");
                if (Quaternion.Angle(transform.rotation, targetRotation) < entityData.rotationMinimalBeforeCharge && (target.position.y - transform.position.y) < entityData.distanceYWithPlayerUpSupported)
                {
                    //PlayerLocked();
                    timerLoading += Time.deltaTime;
                    if (timerLoading > entityData.timeWaitBeforeShoot)
                    {
                        EndLoading(true);
                    }
                    else
                    {
                        for (int i = 0; i < canonRoot.Length; i++)
                        {
                            canonRoot[i].transform.localPosition = new Vector3(canonRootBasePos[i].x, canonRootBasePos[i].y, (1- timerLoading / entityData.timeWaitBeforeShoot) * distTravel);
                        }
                    }
                    //StartLoading();
                }
                else if (timerLoading > 0)
                {
                    timerLoading -= Time.deltaTime;
                    if (timerLoading < 0) timerLoading = 0;
                }
                break;
            case (int)State.Loading:


                anmt.SetTrigger("PrepareShoot");

                timerLoading += Time.deltaTime;
                if (timerLoading > entityData.timeWaitBeforeShoot)
                {
                    EndLoading(true);
                }
                break;
            case (int)State.Shooting:
                timerbeforeNextAttack += Time.deltaTime;
                if (timerbeforeNextAttack > entityData.timeBetweenBullet)
                {
                    timerbeforeNextAttack -= entityData.timeBetweenBullet;

                    if (bulletShot < canonRoot.Length)
                        canonRoot[bulletShot].transform.localPosition = new Vector3(canonRootBasePos[bulletShot].x, canonRootBasePos[bulletShot].y, distTravel);

                    bulletShot++;
                    if(canShoot)
                        Shoot();
                    //GetComponent<Animator>().SetTrigger("Shoot");
                    CustomSoundManager.Instance.PlaySound("SE_Shooter_Launch", "Effect", .5f);
                    CustomSoundManager.Instance.PlaySound("SE_MissileLaunch", "Effect", .7f);
                    if (bulletShot >= entityData.nbShootPerSalve)
                    {
                        // RESET DES VALEURS ET STUN
                        StartRecover();
                    }
                }
                break;
            case (int)State.Stuned:
                break;
            case (int)State.Recovering:
                timerRecovering += Time.deltaTime;
                if (timerRecovering > entityData.recoverTime)
                {
                    StopRecover();
                }
                break;
            default:
                Debug.LogWarning("Incorrect State on " + this.name + ". Please check the code. Current State = " + state);
                break;
        }

    }

    /// <summary>
    /// Commencement de la rotation du shooter vers le joueur
    /// </summary>
    void SpotTarget()
    {
        state = (int)State.Rotating;
        CustomSoundManager.Instance.PlaySound("SE_Shooter_Spot", "Effect", .6f);
    }

    /// <summary>
    /// Le chargeur a fini de s'orienter vers le joueur
    /// </summary>
    void PlayerLocked()
    {
        Vector3 pos = new Vector3(target.position.x, this.transform.position.y, target.position.z);
        transform.rotation = Quaternion.LookRotation(transform.position - pos, Vector3.up);
    }

    /// <summary>
    /// Le chargeur se prépare à charger
    /// </summary>
    void StartLoading()
    {
        state = (int)State.Loading;
        CustomSoundManager.Instance.PlaySound("SE_Shooter_Bip_Rockets", "Effect", 1);
    }

    /// <summary>
    /// Le chargeur arrete sa préparation (si bool true, ça lance la charge)
    /// </summary>
    /// <param name="bShoot"></param>
    void EndLoading(bool bShoot)
    {
        ResetTimers();
        if (bShoot)
            StartShoot();
    }

    /// <summary>
    /// Le tireur commence à tirer
    /// </summary>
    void StartShoot()
    {
        state = (int)State.Shooting;
        allBullets.Clear();
    }


    ///// <summary>
    ///// Le tireur arrête de tirer
    ///// </summary>
    //void StopShoot(bool bIsStun)
    //{
    //    ResetTimers();
    //    if (bIsStun)
    //    {
    //        IsStun();
    //    }
    //    else
    //    {
    //        SpotTarget();
    //    }
    //}

    /// <summary>
    /// Déclenche le stun
    /// </summary>
    protected override void IsStun(float stunDuration)
    {
        if (!isStun)
        {
            //GetComponent<Animator>().SetTrigger("StartStun");
            FxManager.Instance.PlayFx(entityData.fxWhenStun, fxStunPos, stunDuration);
        }
        base.IsStun(stunDuration);
        EndLoading(false);
        GetComponent<Rigidbody>().AddForce(transform.forward * entityData.stunRecoil, ForceMode.Impulse);
        state = (int)State.Stuned;
    }

    /// <summary>
    /// Arrête le stun
    /// </summary>
    protected override void StopStun()
    {
        //GetComponent<Animator>().SetTrigger("StopStun");
        base.StopStun();
        SpotTarget();
    }

    void ChangeColor(Material newMaterial)
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            if (transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>())
            {
                transform.GetChild(0).GetChild(i).GetComponent<MeshRenderer>().material = newMaterial;
            }
        }
    }


    /// <summary>
    /// Fonction qui lance le recover apres un impact
    /// </summary>
    void StartRecover()
    {
        ResetTimers();
        state = (int)State.Recovering;
    }

    /// <summary>
    /// Fin de recover après un impact
    /// </summary>
    void StopRecover()
    {
        ResetTimers();
        SpotTarget();

    }

    public override void OnMovementDetect()
    {
        playerMoving = true;
        considerPlayerIsMoving = entityData.playerMoveTimeReset;
    }

    void Shoot()
    {
        if (canShoot)
        {
            anmt.SetTrigger("Shoot");
            FxManager.Instance.PlayFx(entityData.muzzleFlashFx, canonPlacement.transform.position, canonPlacement.transform.rotation);
            CameraHandler.Instance.AddShake(0.5f, transform.position);
            for (int i = 0; i < entityData.nbBulletPerShoot; i++)
            {
                GameObject CurrBullet = Instantiate(entityData.bulletPrefabs,canonPlacement.transform.position,Quaternion.identity);
                allBullets.Add(CurrBullet.GetComponent<ShooterBullet>());
                float bulletRotation = (bulletShot - 1) < overrideBulletRotation.Length ? overrideBulletRotation[(bulletShot - 1)] : (bulletShot - 1) < entityData.specifyBulletRotation.Length ? entityData.specifyBulletRotation[(bulletShot - 1)] : Random.Range(0, 360);
                CurrBullet.GetComponent<ShooterBullet>().OnCreation(target.gameObject, canonPlacement.transform.position, entityData.amplitudeMultiplier, currentDataBullet, 2, this.gameObject, bulletRotation, entityData.amplitudeCap);
            }

        }
        
    }

    /// <summary>
    /// Reset Des timers
    /// </summary>
    void ResetTimers()
    {
        timerLoading = 0;
        timerRecovering = 0;
        timerbeforeNextAttack = 0;
        bulletShot = 0;
    }

    public override void TakeDamage(float value)
    {
        CustomSoundManager.Instance.PlaySound("SE_Shooter_Damage", "Effect", null, 1, false, 1, 0, 0, false);
        base.TakeDamage(value);
    }

    protected override void Die()
    {
        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.KillSwarmer, this.transform.position);
        CustomSoundManager.Instance.PlaySound("SE_Shooter_Death", "Effect", .6f);
        CameraHandler.Instance.AddShake(5, transform.position);

        foreach (var bullet in allBullets)
        {
            if (bullet != null)
                bullet.DesactivateBullet();
        }

        //Means it has been killed in some way and has not just attacked
        if (health <= 0)
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Vendetta, transform.position, this);

            if (SequenceHandler.Instance != null)
                SequenceHandler.Instance.OnEnemyKill();
        }

        if (entityData.spawnsPartsOnDeath)
            InstansiateDeadBody();

        base.Die();
    }

    void InstansiateDeadBody()
    {
        GameObject deadBodyClone;
        deadBodyClone = Instantiate(entityData.deadBody, transform.position, transform.rotation);
        deadBodyClone.transform.parent = null;
    }


    public void OnHitByOwnBullet()
    {
        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.BackToSender, transform.position);
    }

    public void SetBullets(DataShooterBullet data)
    {
        currentDataBullet = data;
    }

    #region Gravity
    public void OnGravityDirectHit()
    {
        canShoot = false;
    }

    public void OnPull(Vector3 position, float force)
    {
        canShoot = false;
    }

    public void OnRelease()
    {
        canShoot = true;
    }

    public void OnHold()
    {
        
    }

    public void OnZeroG()
    {
        
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        canShoot = true;
    }
    #endregion Gravity
}
