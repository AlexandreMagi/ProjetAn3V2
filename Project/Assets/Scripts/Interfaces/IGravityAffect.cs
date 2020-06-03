using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGravityAffect
{

    void OnGravityDirectHit();

    void OnPull(Vector3 position, float force, bool fleesPlayer = false);

    void OnRelease();

    void OnHold();

    void OnZeroG();

    void OnZeroGRelease();

    void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale);
}
