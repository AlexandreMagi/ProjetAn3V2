using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooter : Enemy, IBulletAffect
{
    private DataShooter shooterData;

    //Stimulus
    #region Stimulus
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
    #endregion

    float considerPlayerIsMoving = 0;

    /// <summary>
    /// Différents états de l'ennemi
    /// </summary>
    enum State { Nothing, Rotating, Loading, Shooting, Stuned, Recovering };

    [Tooltip("Variable qui indique l'étât actuel de l'ennemi")]
    int state = 0;

    [Tooltip("Timer qui indique le temps passé à se préparer à charger")]
    float eTimerLoading = 0;
    [Tooltip("Timer qui indique le temps passé stun après avoir impacté le joueur")]
    float eTimerRecovering = 0;
    [Tooltip("Timer qui indique le avant le prochain tir dans la salve")]
    float eTimerbeforeNextAttack = 0;
    [Tooltip("Indique à quel tir on est dans la salve")]
    int bulletShot = 0;

    [SerializeField]
    GameObject canonPlacement = null;

    bool playerMoving = false;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        state = (int)State.Nothing;
        shooterData = entityData as DataShooter;
    }

    public override void OnDistanceDetect(Transform possibleTarget, float distance)
    {
        base.OnDistanceDetect(possibleTarget, distance);
        if (possibleTarget && distance < shooterData.distanceDetection && target == null)
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
        if (playerMoving && !shooterData.shootEvenIfPlayerMoving)
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
                Vector3 vPos = new Vector3(target.position.x, this.transform.position.y, target.position.z);
                Quaternion targetRotation = Quaternion.LookRotation(transform.position - vPos, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * shooterData.rotationSpeed);
                if (Quaternion.Angle(transform.rotation, targetRotation) < shooterData.rotationMinimalBeforeCharge && (target.position.y - transform.position.y) < 2)
                {
                    PlayerLocked();
                    StartLoading();
                }
                break;
            case (int)State.Loading:
                eTimerLoading += Time.deltaTime;
                if (eTimerLoading > shooterData.timeWaitBeforeShoot)
                {
                    EndLoading(true);
                }
                break;
            case (int)State.Shooting:
                eTimerbeforeNextAttack += Time.deltaTime;
                if (eTimerbeforeNextAttack > shooterData.timeBetweenBullet)
                {
                    eTimerbeforeNextAttack -= shooterData.timeBetweenBullet;
                    bulletShot++;
                    Shoot();
                    //GetComponent<Animator>().SetTrigger("Shoot");
                    //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Launch", false, 0.5f);
                    if (bulletShot >= shooterData.nbShootPerSalve)
                    {
                        // RESET DES VALEURS ET STUN
                        StartRecover();
                    }
                }
                break;
            case (int)State.Stuned:
                break;
            case (int)State.Recovering:
                eTimerRecovering += Time.deltaTime;
                if (eTimerRecovering > shooterData.recoverTime)
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
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Spot", false, 0.6f);
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
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Bip_Rockets", false, 1);
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
            //GameObject.FindObjectOfType<C_Fx>().StunFx(FxPos, enemy.timeStunned);
        }
        base.IsStun(stunDuration);
        EndLoading(false);
        GetComponent<Rigidbody>().AddForce(transform.forward * shooterData.stunRecoil, ForceMode.Impulse);
        state = (int)State.Stuned;
    }

    /// <summary>
    /// Arrête le stun
    /// </summary>
    protected override void StopStun()
    {
        GetComponent<Animator>().SetTrigger("StopStun");
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
        considerPlayerIsMoving = shooterData.playerMoveTimeReset;
    }

    void Shoot()
    {
        //GameObject.FindObjectOfType<C_Camera>().AddShake(3);
        for (int i = 0; i < shooterData.nbBulletPerShoot; i++)
        {
            GameObject CurrBullet = Instantiate(shooterData.bulletPrefabs);
            CurrBullet.GetComponent<ShooterBullet>().OnCreation(target.gameObject, canonPlacement.transform.position, shooterData.amplitudeMultiplier, shooterData.bulletData);
        }
    }

    /// <summary>
    /// Reset Des timers
    /// </summary>
    void ResetTimers()
    {
        eTimerLoading = 0;
        eTimerRecovering = 0;
        eTimerbeforeNextAttack = 0;
        bulletShot = 0;
    }

    public override void TakeDamage(float value)
    {
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Damage", false, 1, 0, 0, false); ;
        base.TakeDamage(value);
    }

    protected override void Die()
    {
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "SE_Shooter_Death", false, .6f);
        //GameObject.FindObjectOfType<C_Fx>().BigEnnemiDied(transform.position);
        base.Die();
    }

}
