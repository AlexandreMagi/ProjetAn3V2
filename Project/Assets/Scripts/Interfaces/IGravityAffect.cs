using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGravityAffect
{

    void OnGravityDirectHit();

    void OnPull(Vector3 position, float force);

    void OnRelease();

    void OnHold();

    void OnZeroG();

    void OnZeroGRelease();

    void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale);
}
