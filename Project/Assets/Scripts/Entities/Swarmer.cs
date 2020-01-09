using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Swarmer : Enemy, IGravityAffect
{
    private DataSwarmer swarmerData;

    private bool isAirbone = false;
    private float fTimePropel = .5f;
    private float fElapsedTime = 0;

    //Stimulus
    public void OnGravityDirectHit()
    {
        ReactGravity.DoFreeze(this);
    }

    protected override void Die()
    {
        base.Die();
        FxManager.Instance.PlayFx("VFX_Death", transform.position, Quaternion.identity);
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
        ReactGravity.DoSpin(this);
    }

    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {
        ReactGravity.DoPull(this, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity.DoFloat(this, timeBeforeActivation, isSlowedDownOnFloat, tFloatTime, bIndependantFromTimeScale);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        swarmerData = entityData as DataSwarmer;
    }

    void FixedUpdate()
    {

        if (isAirbone)
        {
            fElapsedTime += Time.deltaTime;

            if (fElapsedTime >= fTimePropel)
            {
                ReactGravity.DoSpin(this);

                //Check si touche le sol
                fElapsedTime = 0;
                if (Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 1f))
                {
                    isAirbone = false;
                }
            }
        }

    }

}
