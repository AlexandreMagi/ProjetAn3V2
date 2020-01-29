using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : Enemy<DataHover>, IGravityAffect
{
    #region Stimulus
    #region Gravity
    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        throw new System.NotImplementedException();
    }

    public void OnGravityDirectHit()
    {
        throw new System.NotImplementedException();
    }

    public void OnHold()
    {
        throw new System.NotImplementedException();
    }

    public void OnPull(Vector3 position, float force)
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
    #endregion //Gravity
    #endregion //Stimulus
}
