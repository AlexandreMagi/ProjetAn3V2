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

    float newPerfectPlacement = 0;
    bool haveTriedPerfet = false;
    bool reloading = false;
    float reloadingPurcentage = 0;


    [SerializeField]
    GameObject muzzleFlash = null;
    [SerializeField]
    GameObject weaponLight = null;

    float timerMuzzleFlash = 0;
    float timeMuzzleAdded = 0.05f;

    void Awake ()
    {
        _instance = this;
        bulletRemaining = weapon.bulletMax;
        timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
    }
    private void Start()
    {
        CameraHandler.Instance.SetWeapon(weapon);
    }

    private void Update()
    {
        if (timerMuzzleFlash >= 0) timerMuzzleFlash -= Time.unscaledDeltaTime;
        timerMuzzleFlash = Mathf.Clamp(timerMuzzleFlash, 0, 1);
        muzzleFlash.SetActive(timerMuzzleFlash > 0);

        timeRemainingBeforeOrb -= (weapon.grabityOrbCooldownRelativeToTime ? Time.deltaTime : Time.unscaledDeltaTime);

        UiCrossHair.Instance.PlayerHasOrb(timeRemainingBeforeOrb < 0);
        UiReload.Instance.UpdateGraphics(Mathf.Clamp(reloadingPurcentage, 0, 1), newPerfectPlacement, weapon.perfectRange, haveTriedPerfet);

        if (reloading)
        {
            reloadingPurcentage += Time.unscaledDeltaTime / weapon.reloadingTime;
            if (reloadingPurcentage > 1)
            {
                EndReload(false);
            }
        }
        if (reloadingPurcentage > (newPerfectPlacement + weapon.perfectRange)) ReloadValidate();

        var v3 = Input.mousePosition + Vector3.forward * 10; 
        v3 = CameraHandler.Instance.RenderingCam.GetComponent<Camera>().ScreenToWorldPoint(v3);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, v3 - CameraHandler.Instance.RenderingCam.transform.position, 360, 0.0f);
        weaponLight.transform.rotation = Quaternion.LookRotation(newDirection);

    }

    public Vector2 GetBulletAmmount()
    {
        return new Vector2(bulletRemaining, weapon.bulletMax);
    }
    public bool GetIfReloading()
    {
        return reloading;
    }

    public void ReloadingInput()
    {
        if (!reloading && bulletRemaining < weapon.bulletMax && currentChargePurcentage ==0 )
        {
            newPerfectPlacement = Mathf.Clamp(weapon.perfectPlacement + UnityEngine.Random.Range(-weapon.perfectRandom, weapon.perfectRandom), 0f, 1);
            CameraHandler.Instance.AddShake(weapon.reloadingStartShake);
            reloading = true;
            haveTriedPerfet = false;
            UiReload.Instance.DisplayGraphics();
            reloadingPurcentage = 0;
        }
    }

    public void ReloadValidate()
    {
        if (reloading && !haveTriedPerfet)
        {
            haveTriedPerfet = true;
            if (reloadingPurcentage > (newPerfectPlacement - weapon.perfectRange) && reloadingPurcentage < (newPerfectPlacement + weapon.perfectRange))
                EndReload(true);
        }
    }

    public void EndReload(bool perfect)
    {
        reloading = false;
        bulletRemaining = perfect ? weapon.bulletMax + weapon.bulletAddedIfPerfect : weapon.bulletMax;
        UiReload.Instance.HideGraphics(perfect);
        CameraHandler.Instance.AddShake(perfect ? weapon.reloadingPerfectShake : weapon.reloadingShake);
        if (perfect)
        {
            TimeScaleManager.Instance.AddSlowMo(weapon.reloadingPerfectSlowmo, weapon.reloadingPerfectSlowmoDur);
            CameraHandler.Instance.AddRecoil(weapon.reloadingPerfectRecoil);
            CameraHandler.Instance.AddFovRecoil(weapon.reloadingPerfectRecoil);
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.PerfectReload);
        }
    }

    public float GetChargeValue()
    {
        return currentChargePurcentage;
    }

    public void GravityOrbInput()
    {
        if (!reloading)
        {
            if (timeRemainingBeforeOrb < 0)
            {
                GameObject orb = Instantiate(orbPrefab);
                orb.GetComponent<GravityOrb>().OnSpawning(Input.mousePosition);
                timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
            }
        }
    }

    public void InputHold()
    {
        if (!reloading && bulletRemaining >= weapon.chargedShot.bulletCost)
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
    }

    public void InputUp(Vector2 mousePosition)
    {
        if (!reloading)
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


                Vector3 newDirection = Vector3.RotateTowards(transform.forward, rayBullet.direction, 360, 0.0f);
                muzzleFlash.transform.rotation = Quaternion.LookRotation(newDirection);

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
                        if (weaponMod == weapon.baseShot)
                            bAffect.OnHitSingleShot();
                        if (weaponMod == weapon.chargedShot)
                            bAffect.OnHitShotGun();
                        UiCrossHair.Instance.PlayerHitSomething(weaponMod.hitValueUiRecoil);
                    }
                }
            }
            UiCrossHair.Instance.PlayerShot(weaponMod.shootValueUiRecoil);
            CameraHandler.Instance.AddRecoil(weaponMod.recoilPerShot);
            CameraHandler.Instance.AddFovRecoil(weaponMod.recoilPerShot);
            CameraHandler.Instance.AddShake(weaponMod.shakePerShot);
            timerMuzzleFlash += timeMuzzleAdded;
        }
        bulletRemaining -= weaponMod.bulletCost;
        if (bulletRemaining < 0) bulletRemaining = 0;
    }

    private void CheckIfMustSlowMo(GameObject hit, DataWeaponMod weaponMod)
    {
        if (hit.GetComponent<Enemy<DataEnemy>>() != null && weaponMod.bullet.activateSlowMoAtImpact)
            TimeScaleManager.Instance.AddSlowMo(weaponMod.bullet.slowMoPower, weaponMod.bullet.slowMoDuration, 0, weaponMod.bullet.slowMoProbability);
        if (hit.GetComponent<Enemy<DataEnemy>>() != null && weaponMod.bullet.activateStopTimeAtImpact)
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
