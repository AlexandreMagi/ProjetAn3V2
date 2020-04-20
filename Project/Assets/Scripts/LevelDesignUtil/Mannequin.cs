using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : Entity<DataEntity>, IGravityAffect
{
    [SerializeField]
    bool mustBeKilledInZeroG = false;

    bool isFloating = false;
    bool isAffectedByGravity = false;

    float floatTimeLeft = 0;

    Rigidbody rb = null;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
    }

    public override void TakeDamage(float value)
    {
        if((mustBeKilledInZeroG && isFloating) || !mustBeKilledInZeroG)
        {
            Die();
        }
    }

    protected override void Die()
    {
        GetComponentInParent<MannequinManager>().ChildDied();
        base.Die();
    }

    #region Gravity
    public void OnGravityDirectHit()
    {
        ReactGravity<DataProp>.DoFreeze(rb);
        isAffectedByGravity = true;
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
    }

    public void OnZeroG()
    {
        //ReactGravity.DoSpin(this);
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
