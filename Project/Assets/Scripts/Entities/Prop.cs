﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : Entity<DataProp>, IGravityAffect, IBulletAffect
{
    bool isAirbone = false;
    float timePropel = .5f;
    float elapsedTime = 0;

    Rigidbody rb;

  //  DataProp propData;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        entityData = entityData as DataProp;
    }

    #region Gravity
    public void OnGravityDirectHit()
    {
        ReactGravity<DataProp>.DoFreeze(rb);
    }

    public void OnHold()
    {
        //Nothing happens on hold
    }

    public void OnPull(Vector3 origin, float force)
    {
        ReactGravity<DataProp>.DoPull(rb, origin, force, isAirbone);
    }

    public void OnRelease()
    {
        ReactGravity<DataProp>.DoUnfreeze(rb);
    }

    public void OnZeroG()
    {
        //ReactGravity.DoSpin(this);
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        ReactGravity<DataProp>.DoPull(rb, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity<DataProp>.DoFloat(rb, timeBeforeActivation, isSlowedDownOnFloat, floatTime, bIndependantFromTimeScale);
    }
    #endregion

    #region Bullet
    public void OnHit(DataWeaponMod mod, Vector3 position)
    {

        ReactBullet.PushFromHit(this.GetComponent<Rigidbody>(), position, 2400, 5);
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

    public void OnCursorClose()
    {
       
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
