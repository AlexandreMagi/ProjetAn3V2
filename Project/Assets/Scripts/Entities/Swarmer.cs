using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarmer : Enemy, IGravityAffect, IBulletAffect
{
    private DataSwarmer swarmerData;

    private bool isAirbone = false;
    private float fTimePropel = .5f;
    private float fElapsedTime = 0;

    //Stimulus
    public void OnGravityDirectHit()
    {
        Debug.Log("Direct hit swarmer");

        ReactGravity.DoFreeze(this);
    }

    public void OnHold()
    {
        throw new System.NotImplementedException();
    }

    public void OnPull(Vector3 origin, float force)
    {
        ReactGravity.DoPull(this, origin, force, isAirbone);
    }

    public void OnRelease()
    {
        throw new System.NotImplementedException();
    }

    public void OnZeroG()
    {
        throw new System.NotImplementedException();
    }

    public void OnFloatingActivation()
    {
        throw new System.NotImplementedException();
    }

    //  ################################################### //
    //  ################ BULLET AFFECTED ################## //
    //  ################################################### //

    public void Hit(DataWeaponMod bulletType)
    {
        TakeDamage(bulletType.bullet.damage);
    }

    public void HitClose()
    {
        throw new System.NotImplementedException();
    }

    public void CursorNear()
    {
        throw new System.NotImplementedException();
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
                //Spin();

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
