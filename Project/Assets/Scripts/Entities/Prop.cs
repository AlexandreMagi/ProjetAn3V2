using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Entity, IGravityAffect
{
    bool isAirbone = false;
    float timePropel = .5f;
    float elapsedTime = 0;

    DataProp propData;

    protected override void Start()
    {
        base.Start();

        propData = entityData as DataProp;
    }

    #region Gravity
    public void OnGravityDirectHit()
    {
        ReactGravity.DoFreeze(this);
    }

    public void OnHold()
    {
        //Nothing happens on hold
    }

    public void OnPull(Vector3 origin, float force)
    {
        ReactGravity.DoPull(this, origin, force, isAirbone);
    }

    public void OnRelease()
    {
        ReactGravity.DoUnfreeze(this);
    }

    public void OnZeroG()
    {
        //ReactGravity.DoSpin(this);
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        ReactGravity.DoPull(this, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity.DoFloat(this, timeBeforeActivation, isSlowedDownOnFloat, tFloatTime, bIndependantFromTimeScale);
    }
    #endregion

    protected virtual void FixedUpdate()
    {

        //Check for airbone and makes it spin if in the air
        if (isAirbone)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timePropel)
            {
                //ReactGravity.DoSpin(this);

                //Check si touche le sol
                elapsedTime = 0;
                if (Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 1f))
                {
                    isAirbone = false;
                }

            }

        }

    }
}
