using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionLight_LightBulb : MonoBehaviour,IBulletAffect
{

    [SerializeField] ConstructionLightScript manager = null;
    [SerializeField] ParticleSystem fxToPlay = null;

    public void OnBulletClose() { }

    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray shotRay)
    {
        manager.LightBulbDestroyed();
        Debug.Log("Play LightBulbDestuction Sound");
        Debug.Log("Play LightBulbDestuction Fx");
        if (fxToPlay != null) fxToPlay.Play();
        gameObject.SetActive(false);
    }

    public void OnHitShotGun(DataWeaponMod mod) { }

    public void OnHitSingleShot(DataWeaponMod mod) { }
}
