﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBulletAffect
{
    void OnHit(DataWeaponMod mod, Vector3 position);

    void OnHitShotGun(DataWeaponMod mod);

    void OnHitSingleShot(DataWeaponMod mod);

    void OnBulletClose();
}
