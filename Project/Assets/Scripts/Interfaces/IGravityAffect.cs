using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGravityAffect
{
    void OnDirectHit();

    void OnPull();

    void OnRelease();

    void OnHold();

    void OnZeroG();
}
