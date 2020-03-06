using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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

    private GameObject currentOrb;

    float timeRemainingBeforeOrb = 0;

    float newPerfectPlacement = 0;
    bool haveTriedPerfet = false;
    bool reloading = false;
    float reloadingPurcentage = 0;

    bool shotGunHasHit = false;
    float reloadCoolDown = 0;


    [SerializeField]
    GameObject muzzleFlash = null;
    [SerializeField]
    bool rotateWithCursor = true;
    [HideInInspector]
    public bool rotateLocked = false;
    [SerializeField]
    Light weaponLight = null;
    [SerializeField]
    bool ignoreBulletLimitForCharge = false;
    [SerializeField]
    bool shotgunBounces = false;
    [SerializeField, ShowIf("shotgunBounces")]
    float bounceLag = 0.05f;


    float timerMuzzleFlash = 0;
    float timeMuzzleAdded = 0.05f;

    Main mainContainer = null;

    void Awake ()
    {
        _instance = this;
        bulletRemaining = weapon.bulletMax;
        //timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
    }
    private void Start()
    {
        CameraHandler.Instance.SetWeapon(weapon);
        mainContainer = GetComponent<Main>();
    }

    private void Update()
    {
        if (reloadCoolDown > 0) reloadCoolDown -= Time.unscaledDeltaTime;
        if (reloadCoolDown < 0) reloadCoolDown = 0;

        if (timerMuzzleFlash >= 0) timerMuzzleFlash -= Time.unscaledDeltaTime;
        timerMuzzleFlash = Mathf.Clamp(timerMuzzleFlash, 0, 1);
        muzzleFlash.SetActive(timerMuzzleFlash > 0);

        timeRemainingBeforeOrb -= (weapon.grabityOrbCooldownRelativeToTime ? Time.deltaTime : Time.unscaledDeltaTime);

        UiCrossHair.Instance.PlayerHasOrb(timeRemainingBeforeOrb < 0 && mainContainer.PlayerCanOrb);
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

        Vector3 v3 = Main.Instance.GetCursorPos() + Vector3.forward * 10; 
        v3 = CameraHandler.Instance.renderingCam.ScreenToWorldPoint(v3);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, v3 - CameraHandler.Instance.renderingCam.transform.position, 360, 0.0f);
        if (!rotateLocked)
            weaponLight.transform.rotation = Quaternion.LookRotation(newDirection);
        else
            weaponLight.transform.rotation = Quaternion.LookRotation(CameraHandler.Instance.renderingCam.transform.forward, Vector3.up);

        weaponLight.spotAngle = Mathf.Lerp(weapon.baseAngle, weapon.chargedAngle, currentChargePurcentage);
        weaponLight.range = Mathf.Lerp(weapon.baseRange, weapon.chargedRange, currentChargePurcentage);
        weaponLight.intensity = Mathf.Lerp(weapon.baseIntensity, weapon.chargedIntensity, currentChargePurcentage);

        if (displayOrb)
        {
            Ray rayBullet = CameraHandler.Instance.renderingCam.ScreenPointToRay(Main.Instance.GetCursorPos());
            //Shoot raycast
            RaycastHit hit;
            if (Physics.Raycast(rayBullet, out hit, Mathf.Infinity, orbData.layerMask))
            {
                orb.transform.position = hit.point;
                orb.transform.localScale = Vector3.Lerp(orb.transform.localScale, Vector3.one * orbData.gravityBullet_AttractionRange, Time.unscaledDeltaTime * 5);
            }
        }
        else  orb.transform.localScale = Vector3.zero;


    }
    [SerializeField]
    private DataGravityOrb orbData = null;
    [SerializeField]
    private GameObject orb = null;
    [HideInInspector]
    public bool displayOrb = true;

    void HitMarkerSoundFunc()
    {
        CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "HitMarker_Boosted", false, 0.5f, 0, 3f, false);
    }
    public float GetOrbValue()
    {
        return mainContainer.PlayerCanOrb ? (1 - (timeRemainingBeforeOrb / weapon.gravityOrbCooldown)) : 0;
    }

    public Vector2Int GetBulletAmmount()
    {
        return new Vector2Int(bulletRemaining, weapon.bulletMax);
    }
    public bool GetIfReloading()
    {
        return reloading;
    }
    public int GetChargedWeaponBulletCost()
    {
        return weapon.chargedShot.bulletCost;
    }
    public int GetSuplementaryBullet()
    {
        return weapon.bulletAddedIfPerfect;
    }

    public void ReloadingInput()
    {
        if (!reloading && (bulletRemaining < weapon.bulletMax || weapon.canReloadAnytime) && currentChargePurcentage ==0 && reloadCoolDown == 0)
        {
            newPerfectPlacement = Mathf.Clamp(weapon.perfectPlacement + UnityEngine.Random.Range(-weapon.perfectRandom, weapon.perfectRandom), 0f, 1);
            CameraHandler.Instance.AddShake(weapon.reloadingStartShake);
            reloading = true;
            haveTriedPerfet = false;
            UiReload.Instance.DisplayGraphics();
            reloadingPurcentage = 0;
            bulletRemaining = 0;
            reloadCoolDown = weapon.reloadCooldown;

            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "ReloadStart", false, 1f);
        }
    }

    public bool ReloadValidate()
    {
        if (reloading && !haveTriedPerfet)
        {
            haveTriedPerfet = true;
            if (reloadingPurcentage > (newPerfectPlacement - weapon.perfectRange) && reloadingPurcentage < (newPerfectPlacement + weapon.perfectRange))
                EndReload(true);
            else
            {
                CameraHandler.Instance.AddShake(weapon.reloadingMissTryShake);
                CameraHandler.Instance.AddRecoil(false, weapon.reloadingMissTryRecoil);
                CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Reload_FinishMiss", false, 1f);
            }
            return false;
        }
        return true;
    }

    public void EndReload(bool perfect)
    {
        reloading = false;
        UiReload.Instance.HideGraphics(perfect, bulletRemaining);
        bulletRemaining = perfect ? weapon.bulletMax + weapon.bulletAddedIfPerfect : weapon.bulletMax;
        CameraHandler.Instance.AddShake(perfect ? weapon.reloadingPerfectShake : weapon.reloadingShake);
        if (perfect)
        {
            //Le slow motion est lourd et redondant si on est pas dans l'action. Il est désactivé si on est pas sur une sequence ennemis

            if(SequenceHandler.Instance != null && SequenceHandler.Instance.IsCurrentSequenceOnAction())
            {
                TimeScaleManager.Instance.AddSlowMo(weapon.reloadingPerfectSlowmo, weapon.reloadingPerfectSlowmoDur);
                CameraHandler.Instance.AddRecoil(false,weapon.reloadingPerfectRecoil, true);
            }
            
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.PerfectReload, transform.position);
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Reload_FinishPerfect", false, 1f);
        }
        else
        {
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Reload_Finish", false, 1f);
        }
    }

    public float GetChargeValue()
    {
        return currentChargePurcentage;
    }

    public bool GravityOrbInput()
    {
        if (!reloading)
        {
            if (timeRemainingBeforeOrb < 0)
            {
                currentOrb = Instantiate(orbPrefab);
                currentOrb.GetComponent<GravityOrb>().OnSpawning(Main.Instance.GetCursorPos());
                timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
                return true;
            }

            else if (currentOrb != null && weapon.gravityOrbCanBeReactivated && !currentOrb.GetComponent<GravityOrb>().hasExploded)
            {
                currentOrb.GetComponent<GravityOrb>().StopHolding();
                return true;
            }
        }
        return false;
    }

    public void InputHold()
    {
        if (!reloading && bulletRemaining >= (ignoreBulletLimitForCharge ? 1 : weapon.chargedShot.bulletCost))
        {
            if (currentChargePurcentage < 1)
            {
                currentChargePurcentage += (weapon.chargeSpeedIndependantFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / weapon.chargeTime;
                if (currentChargePurcentage > 1)
                {
                    UiCrossHair.Instance.JustFinishedCharging();
                    CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Charged_Shotgun", false, 1f, 0.1f);
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
            if (currentChargePurcentage == 1) currentWeaponMod = weapon.chargedShot;
            else currentWeaponMod = weapon.baseShot;


            currentChargePurcentage = 0;
            OnShoot(mousePosition, currentWeaponMod);
        }
    }

    private void OnShoot(Vector2 mousePosition, DataWeaponMod weaponMod)
    {
        if (bulletRemaining > 0)
        {
            List<Ray> bounceCalculations = new List<Ray>();
            if(weaponMod == weapon.chargedShot)
            {
                shotGunHasHit = false;
                Invoke("CheckIfShotGunHasHit", .1f);
            }
            if (UiDamageHandler.Instance != null)
                UiDamageHandler.Instance.MuzzleFlashFunc();
            for (int i = 0; i < weaponMod.bulletPerShoot; i++)
            {
                Camera mainCam = CameraHandler.Instance.renderingCam;

                Vector3 imprecision = new Vector3(UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision),
                                                    UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision),
                                                    UnityEngine.Random.Range(-weaponMod.bulletImprecision, weaponMod.bulletImprecision));
                if (i == 0 && weaponMod.firstBulletAlwaysPrecise)
                    imprecision = Vector3.zero;

                Ray rayBullet = mainCam.ScreenPointToRay(mousePosition);


                if (rotateWithCursor)
                {
                    Vector3 newDirection = Vector3.RotateTowards(transform.forward, rayBullet.direction, 360, 0.0f);
                    muzzleFlash.transform.rotation = Quaternion.LookRotation(newDirection);
                }
                else
                {
                    muzzleFlash.transform.rotation = Quaternion.LookRotation(CameraHandler.Instance.renderingCam.transform.forward, Vector3.up);
                }

                rayBullet.direction += imprecision;

                //Shoot raycast
                RaycastHit hit;
                if (Physics.Raycast(rayBullet, out hit, Mathf.Infinity, weapon.layerMaskHit))
                {
                    FxImpactDependingOnSurface(hit.transform.gameObject, hit.point, weaponMod, 0.2f, rayBullet, hit);
                    CheckIfMustSlowMo(hit.transform.gameObject, weaponMod);


                    IBulletAffect bAffect = hit.transform.GetComponent<IBulletAffect>();
                    if (bAffect != null)
                    {
                        bAffect.OnHit(weaponMod, hit.point, i==0 ? weaponMod.bullet.damage * weaponMod.firstBulletDamageMultiplier : weaponMod.bullet.damage);
                        if (weaponMod == weapon.baseShot)
                            bAffect.OnHitSingleShot(weaponMod);
                        if (weaponMod == weapon.chargedShot)
                            bAffect.OnHitShotGun(weaponMod);

                        Invoke("HitMarkerSoundFunc", 0.05f * Time.timeScale);
                        TimeScaleManager.Instance.AddStopTime(weaponMod.stopTimeAtImpact);

                        UiCrossHair.Instance.PlayerHitSomething(weaponMod.hitValueUiRecoil);

                        //PUBLIC
                        if(hit.collider.GetComponent<ShooterBullet>() != null && hit.distance < 2)
                        {
                            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.PerfectProjectile, transform.position);
                        }
                    }

                    if(shotgunBounces && weaponMod == weapon.chargedShot)
                    {
                        Vector3 directionShot = rayBullet.direction;
                        Vector3 normalFromHit = hit.normal;

                        Vector3 bounceDirection = directionShot - 2 * Vector3.Dot(directionShot, normalFromHit) * normalFromHit;
                        Ray rayBounce = new Ray(hit.point, bounceDirection);

                        bounceCalculations.Add(rayBounce);
                    }
                }
            }
            if (bounceCalculations.Count > 0)
            {
                StartCoroutine(BounceBullets(bounceCalculations, bounceLag));
            }

            UiCrossHair.Instance.PlayerShot(weaponMod.shootValueUiRecoil, weaponMod == weapon.chargedShot);
            UiReload.Instance.PlayerShot();
            CameraHandler.Instance.AddRecoil(false,weaponMod.recoilPerShot, true);
            CameraHandler.Instance.AddShake(weaponMod.shakePerShot, weaponMod.shakeTimePerShot);
            timerMuzzleFlash += timeMuzzleAdded;
            bulletRemaining -= weaponMod.bulletCost;
            if (bulletRemaining < 0) bulletRemaining = 0;
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, weaponMod == weapon.chargedShot ? weapon.chargedShot.soundPlayed : weapon.baseShot.soundPlayed, false, weaponMod == weapon.chargedShot ?  0.8f : 0.4f, 0.2f);

        }
        else
        {
            CameraHandler.Instance.AddShake(weapon.shakeIfNoBullet);
            CameraHandler.Instance.AddRecoil(false, weapon.recoilIfNoBullet);
        }
    }

    private void CheckIfMustSlowMo(GameObject hit, DataWeaponMod weaponMod)
    {
        if (hit.GetComponent<Enemy<DataEnemy>>() != null && weaponMod.bullet.activateSlowMoAtImpact)
            TimeScaleManager.Instance.AddSlowMo(weaponMod.bullet.slowMoPower, weaponMod.bullet.slowMoDuration, 0, weaponMod.bullet.slowMoProbability);
        if (hit.GetComponent<Enemy<DataEnemy>>() != null && weaponMod.bullet.activateStopTimeAtImpact)
            TimeScaleManager.Instance.AddStopTime(weaponMod.bullet.timeStopAtImpact, 0, weaponMod.bullet.timeStopProbability);
    }

    private void FxImpactDependingOnSurface(GameObject hit, Vector3 hitPoint, DataWeaponMod weaponMod, float castradius, Ray raybase, RaycastHit hitBase)
    {
        for (int i = 0; i < weaponMod.bullet.bulletFxs.allFxReaction.Length; i++)
        {
            if (weaponMod.bullet.bulletFxs.allFxReaction[i].mask == (weaponMod.bullet.bulletFxs.allFxReaction[i].mask | (1 << hit.layer)))
            {
                FxManager.Instance.PlayFx(weaponMod.bullet.bulletFxs.allFxReaction[i].fxName, hitBase, raybase);
                DecalManager.Instance.ProjectDecal(hitBase);
            }
        }
    }

    private void CheckIfShotGunHasHit()
    {
        if (!shotGunHasHit)
        {
            //PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.MissShotGun, transform.position);
        }
    }

    public void OnShotGunHitTarget()
    {
        shotGunHasHit = true;
    }

    IEnumerator BounceBullets(List<Ray> bounces, float bounceLag)
    {
        yield return new WaitForSeconds(bounceLag);

        //On divise par 2 lol
        DataWeaponMod bounceMod = Instantiate(weapon.chargedShot);
        bounceMod.bullet.damage /= 2;

        foreach(Ray bounceBullet in bounces)
        {
            RaycastHit hit;
            if (Physics.Raycast(bounceBullet, out hit, Mathf.Infinity, weapon.layerMaskHit))
            {

                FxImpactDependingOnSurface(hit.transform.gameObject, hit.point, bounceMod, 0.2f, bounceBullet, hit);
                CheckIfMustSlowMo(hit.transform.gameObject, bounceMod);

                IBulletAffect bAffect = hit.transform.GetComponent<IBulletAffect>();
                if (bAffect != null)
                {
                    bAffect.OnHit(bounceMod, hit.point, bounceMod.bullet.damage);

                    bAffect.OnHitShotGun(bounceMod);

                    TimeScaleManager.Instance.AddStopTime(bounceMod.stopTimeAtImpact);

                    UiCrossHair.Instance.PlayerHitSomething(bounceMod.hitValueUiRecoil);

                    //PUBLIC
                    if (hit.collider.GetComponent<ShooterBullet>() != null && hit.distance < 2)
                    {
                        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.PerfectProjectile, transform.position);
                    }
                }
            }
        }

        Destroy(bounceMod);

        yield break;
    }
}
