using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletAffect
{
    void OnHitShotGun();

    void OnHitSingleShot();

    void OnBulletClose();

    void OnCursorClose();
}
