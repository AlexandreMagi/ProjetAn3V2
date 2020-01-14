using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private static Weapon _instance;
    public static Weapon Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField]
    DataWeapon weapon = null;
    int bulletRemaining = 0;
    float currentChargePurcentage = 0;
    [SerializeField]
    private GameObject orbPrefab = null;

    float timeRemainingBeforeOrb = 0;

    void Awake ()
    {
        _instance = this;
        bulletRemaining = weapon.bulletMax;
        timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
    }

    private void Update()
    {
        timeRemainingBeforeOrb -= (weapon.grabityOrbCooldownRelativeToTime ? Time.deltaTime : Time.unscaledDeltaTime);
    }

    public float GetChargeValue()
    {
        return currentChargePurcentage;
    }

    public void GravityOrbInput()
    {
        if (timeRemainingBeforeOrb < 0)
        {
            GameObject orb = Instantiate(orbPrefab);
            orb.GetComponent<GravityOrb>().OnSpawning(Input.mousePosition);
            timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
        }
    }

    public void InputHold()
    {
        if (currentChargePurcentage < 1)
        {
            currentChargePurcentage += (weapon.chargeSpeedIndependantFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / weapon.chargeTime;
            if (currentChargePurcentage > 1)
            {
                currentChargePurcentage = 1;
            }
        }
    }

    public void InputUp(Vector2 mousePosition)
    {
        DataWeaponMod currentWeaponMod = null;
        if (currentChargePurcentage == 1)
        {
            currentWeaponMod = weapon.chargedShot;
        }
        else
        {
            currentWeaponMod = weapon.baseShot;
        }
        currentChargePurcentage = 0;
        OnShoot(mousePosition, currentWeaponMod);
    }

    private void OnShoot(Vector2 mousePosition, DataWeaponMod weaponMod)
    {
        if (bulletRemaining > 0)
        {
            for (int i = 0; i < weaponMod.bulletPerShoot; i++)
            {
                GameObject mainCam = CameraHandler.Instance.RenderingCam;
                Vector3 imprecision = new Vector3(  UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision),
                                                    UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision),
                                                    UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision));
                Ray rayBullet = mainCam.GetComponent<Camera>().ScreenPointToRay(mousePosition);
                rayBullet.direction += imprecision;

                //Shoot raycast
                RaycastHit hit;
                if (Physics.Raycast(rayBullet, out hit, Mathf.Infinity, weapon.layerMaskHit))
                {
                    FxImpactDependingOnSurface(hit.transform.gameObject, hit.point, weaponMod);
                    CheckIfMustSlowMo(hit.transform.gameObject, weaponMod);
                    IBulletAffect bAffect = hit.transform.GetComponent<IBulletAffect>();
                    if (bAffect != null)
                    {
                        bAffect.OnHit(weaponMod, hit.point);
                        UiCrossHair.Instance.PlayerHitSomething(weaponMod.hitValueUiRecoil);
                    }
                }
            }
            UiCrossHair.Instance.PlayerShot(weaponMod.shootValueUiRecoil);
        }
        bulletRemaining -= weaponMod.bulletCost;
        if (bulletRemaining < 0) bulletRemaining = 0;
    }

    private void CheckIfMustSlowMo(GameObject hit, DataWeaponMod weaponMod)
    {
        if (hit.GetComponent<Enemy>() != null && weaponMod.bullet.activateSlowMoAtImpact)
            TimeScaleManager.Instance.AddSlowMo(weaponMod.bullet.slowMoPower, weaponMod.bullet.slowMoDuration, 0, weaponMod.bullet.slowMoProbability);
        if (hit.GetComponent<Enemy>() != null && weaponMod.bullet.activateStopTimeAtImpact)
            TimeScaleManager.Instance.AddStopTime(weaponMod.bullet.timeStopAtImpact, 0, weaponMod.bullet.timeStopProbability);
    }

    private void FxImpactDependingOnSurface(GameObject hit, Vector3 hitPoint, DataWeaponMod weaponMod)
    {
        for (int i = 0; i < weaponMod.bullet.bulletFxs.allFxReaction.Length; i++)
        {
            if (weaponMod.bullet.bulletFxs.allFxReaction[i].mask == (weaponMod.bullet.bulletFxs.allFxReaction[i].mask | (1 << hit.layer)))
            {
                FxManager.Instance.PlayFx(weaponMod.bullet.bulletFxs.allFxReaction[i].fxName, hitPoint, Quaternion.identity);
            }
        }
    }
}
