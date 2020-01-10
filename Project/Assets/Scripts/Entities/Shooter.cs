using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooter : Enemy, IBulletAffect
{
    private DataShooter shooterData;

    //Stimulus
    #region Stimulus
    #region Bullets
    public void OnHit(DataWeaponMod mod)
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


    /// <summary>
    /// Différents états de l'ennemi
    /// </summary>
    enum State { Nothing, Rotating, Loading, Shooting, Stuned, Recovering };

    [Tooltip("Variable qui indique l'étât actuel de l'ennemi")]
    int nState = 0;

    [Tooltip("Timer qui indique le temps passé à se préparer à charger")]
    float fETimerLoading = 0;
    [Tooltip("Timer qui indique le temps passé stun après avoir impacté le joueur")]
    float fETimerRecovering = 0;
    [Tooltip("Timer qui indique le avant le prochain tir dans la salve")]
    float fETimerbeforeNextAttack = 0;
    [Tooltip("Indique à quel tir on est dans la salve")]
    int nBulletShot = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        nState = (int)State.Nothing;
        shooterData = entityData as DataShooter;
    }

    void FixedUpdate()
    {

    }


}
