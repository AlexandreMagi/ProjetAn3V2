using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletAffect
{
    void Hit(DataWeaponMod bulletType);

    void HitClose();

    void CursorNear();

}
