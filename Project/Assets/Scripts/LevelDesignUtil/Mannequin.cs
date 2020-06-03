using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : Entity<DataProp>, IGravityAffect, IBulletAffect
{
    [SerializeField]
    bool mustBeKilledInZeroG = false;


    bool isFloating = false;
    [HideInInspector] public bool isAffectedByGravity = false;

    float floatTimeLeft = 0;

    bool willDie = false;

    [SerializeField] Renderer[] renderers;

    [SerializeField,ColorUsage(true, true)]
    Color colorBase = Color.black;
    [SerializeField, ColorUsage(true, true)]
    Color colorWhenZeroG = Color.white;

    Rigidbody rb = null;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();

    }

    public override void TakeDamage(float value)
    {
        if(((mustBeKilledInZeroG && isFloating) || !mustBeKilledInZeroG ) && !willDie)
        {
            willDie = true;
            Die();
        }
    }

    public override void ForceKill()
    {
        if (((mustBeKilledInZeroG && isFloating) || !mustBeKilledInZeroG) && !willDie)
        {

            willDie = true;
            Die();
        }
    }

    protected override void Die()
    {
        FxManager.Instance.PlayFx(entityData.fxPlayedOnDestroy, transform.position, Quaternion.identity);
        if (DeadBodyPartManager.Instance != null) DeadBodyPartManager.Instance.RequestPop(entityData.fractureType, transform.position, transform.up * .5f);
        MannequinManager parentManger = GetComponentInParent<MannequinManager>();

        if (parentManger)
        {
            GetComponentInParent<MannequinManager>().ChildDied();





            base.Die();
        }
       
    }

    #region Gravity
    public void OnGravityDirectHit()
    {
        ReactGravity<DataProp>.DoFreeze(rb);
        isAffectedByGravity = true;
    }

    public void OnZeroGRelease()
    {
        isFloating = false;
    }

    public void OnHold()
    {
        //Nothing happens on hold
    }

    public void OnPull(Vector3 origin, float force)
    {
        ReactGravity<DataProp>.DoPull(rb, origin, force, isFloating);
        isAffectedByGravity = true;
    }

    public void OnRelease()
    {
        ReactGravity<DataProp>.DoUnfreeze(rb);

        Debug.Log(renderers.Length + " OnRelease");
        /*foreach (Renderer _renderer in renderers)
        {
            _renderer.materials[0].SetColor("_Reveallightcolor", colorBase);
        }*/
    }

    public void OnZeroG()
    {
        ReactGravity<DataProp>.DoSpin(rb);

        foreach (Renderer _renderer in renderers)
        {
            Debug.Log(_renderer.materials[0]);
            _renderer.materials[1].SetColor("_Reveallightcolor", colorWhenZeroG);
        }
    }

    public void SetTimerToRelease(float timeSent) { Invoke("CompleteRelease", timeSent + 2.5f); }
    void CompleteRelease() { isAffectedByGravity = false; isFloating = false; }


    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        //Die();
        
        ReactGravity<DataProp>.DoPull(rb, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity<DataProp>.DoFloat(rb, timeBeforeActivation, isSlowedDownOnFloat, floatTime, bIndependantFromTimeScale);

        isFloating = true;
        floatTimeLeft = floatTime;

    }
    #endregion

    #region Bullets
    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray shotRay)
    {
        if (mustBeKilledInZeroG && isFloating)
        {
            rb.AddExplosionForce(250, position, 1);
            TakeDamage(mod.bullet.damage);
        }
           
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
    }

    public void OnBulletClose()
    {
    }
    #endregion

    void Update()
    {
        if(floatTimeLeft > 0)
        {
            floatTimeLeft -= Time.deltaTime;

            if(floatTimeLeft <= 0)
            {
                floatTimeLeft = 0;
                isFloating = false;
            }
        }
    }


}
