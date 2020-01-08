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
    private GameObject orbPrefab;

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
                GameObject mainCam = Camera.main.gameObject;
                Vector3 imprecision = new Vector3(  UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision),
                                                    UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision),
                                                    UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision));
                Ray rayBullet = mainCam.GetComponent<Camera>().ScreenPointToRay(mousePosition);
                rayBullet.direction += imprecision;

                //Shoot raycast
                RaycastHit hit;
                if (Physics.Raycast(rayBullet, out hit, Mathf.Infinity, weapon.layerMaskHit))
                {
                    FxImpactDependingOnSurface(hit.transform.gameObject, hit.point);
                    CheckIfMustSlowMo(hit.transform.gameObject, weaponMod);
                    IBulletAffect bAffect = hit.transform.GetComponent<IBulletAffect>();
                    if (bAffect != null)
                        bAffect.Hit(weaponMod);
                }
            }
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

    private void FxImpactDependingOnSurface(GameObject hit, Vector3 hitPoint)
    {
        if (hit.GetComponent<Enemy>() != null)
            FxManager.Instance.PlayFx("VFX_ImpactBlood", hitPoint, Quaternion.identity);
        else 
            FxManager.Instance.PlayFx("VFX_ImpactWalls", hitPoint, Quaternion.identity);
    }

    // Permet d'obtenir la valeur de charge pour les feedbacks -> renvoit le pourcentage de charge avec une marge de sécurité
    public float GetChargeValue()
    {
        float ValueSafe = 0.25f;
        float Chargevalue = 0;
        if (currentChargePurcentage > ValueSafe)
            Chargevalue = (currentChargePurcentage - ValueSafe) / (1 - ValueSafe);
        return Chargevalue;
    }
}
