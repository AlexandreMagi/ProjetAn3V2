using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swarmer : Enemy, IGravityAffect
{
    private DataSwarmer swarmerData; 

    public void OnDirectHit()
    {
        throw new System.NotImplementedException();
    }

    public void OnHold()
    {
        throw new System.NotImplementedException();
    }

    public void OnPull()
    {
        throw new System.NotImplementedException();
    }

    public void OnRelease()
    {
        throw new System.NotImplementedException();
    }

    public void OnZeroG()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        swarmerData = entityData as DataSwarmer;
    }

   
}
