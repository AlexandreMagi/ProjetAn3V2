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
    float savedReloadingPurcentage = 0;

    bool shotGunHasHit = false;
    float reloadCoolDown = 0;

    [SerializeField]
    bool ignoreBulletLimitForCharge = false;
    [SerializeField]
    bool shotgunBounces = false;
    [SerializeField, ShowIf("shotgunBounces")]
    float bounceLag = 0.05f;

    float timeRemainingBeforeCanValidateReload = 0;

    // --- Lumières sur le gun

    [SerializeField]
    GameObject muzzleFlash = null;
    [SerializeField]
    bool rotateWithCursor = true;
    [HideInInspector]
    public bool rotateLocked = false;
    [SerializeField]
    Light weaponLight = null;
    float timerMuzzleFlash = 0;
    float timeMuzzleAdded = 0.05f;

    float rangeMultipler = 1;
    float intensityMultiplier = 1;
    float lastLightDistance = 15;
    int lastFrameLightRayCast = 0;
    int shootLightRayCastEvery = 5;

    Main mainContainer = null;

    // --- Display gravity orb previsualisation
    private DataGravityOrb orbData = null;
    float timeRemainingBeforeCanReactivateOrb = 0;
    [SerializeField]
    private GameObject orbPrevisu = null;
    [SerializeField]
    private ParticleSystem orbPrevisuFx = null;
    [HideInInspector]
    public bool displayOrb = true;
    bool tpOrb = false;
    float pauseOrbFx = 0;
    float pauseOrbFxTimer = 0.5f;
    private Transform gravityOrbRangeDisplay = null;
    private float sizeAimedOrbVisu = 0;
    private float timeToMaxSize = 0.2f;
    private float timeToMinSize = 0.2f;
    private float timerOrb = 0;

    [SerializeField] public bool crosshairOnEnemy = false;
    int lastFrameCrosshairRayCast = 0;
    int shootCrosshairRayCastEvery = 5;


    // --- Minigun
    bool isMinigun = false;
    public bool IsMinigun { get { return isMinigun; } }
    [SerializeField] DataWeaponMod minigunMod = null;
    float minigunCooldownTime = 0;
    Vector3 cursorImprecisionRandomSaved = Vector3.zero;
    Vector3 cursorImprecisionRandomGoTo = Vector3.zero;
    Vector2 cursorImprecision = Vector2.zero;
    public Vector2 CursorImprecision { get { return cursorImprecision; } }
    float currentCursorImprecisionPurcentage = 0;
    float customTimeForCursorNoise = 0;
    float currMinigunRateOfFirePurcentage = 0;
    public float CurrMinigunRateOfFirePurcentage { get { return currMinigunRateOfFirePurcentage; } }

    [SerializeField]
    float maxSpeedMinigunPitch = 0.55f;
    AudioSource minigunAudioSource = null;

    Vector3 lastHitOrbPosition = Vector3.zero;
    Vector3 lastHitOrbNormal = Vector3.zero;
    bool firstRay = true;
    bool orbHitSomething = false;
    void Awake ()
    {
        _instance = this;
        bulletRemaining = weapon.bulletMax;
        orbData = orbPrefab.GetComponent<GravityOrb>().OrbData;
        //timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
    }
    private void Start()
    {
        CameraHandler.Instance.SetWeapon(weapon);
        mainContainer = GetComponent<Main>();
        minigunAudioSource = CustomSoundManager.Instance.PlaySound("Se_Minigun_Engine", "Player", CameraHandler.Instance.renderingCam.transform, 2f, true, 0);
    }

    private void Update()
    {
        if (reloadCoolDown > 0) reloadCoolDown -= Time.unscaledDeltaTime;
        if (reloadCoolDown < 0) reloadCoolDown = 0;

        if (timerMuzzleFlash >= 0) timerMuzzleFlash -= Time.unscaledDeltaTime;
        timerMuzzleFlash = Mathf.Clamp(timerMuzzleFlash, 0, 1);
        muzzleFlash.SetActive(timerMuzzleFlash > 0);

        if (timeRemainingBeforeOrb >= 0)
        {
            timeRemainingBeforeOrb -= (weapon.grabityOrbCooldownRelativeToTime ? Time.deltaTime : Time.unscaledDeltaTime);
            if (timeRemainingBeforeOrb < 0 && mainContainer.playerCanOrb) UIOrb.Instance.OrbCooldownUp();
        }

        UiCrossHair.Instance.PlayerHasOrb(timeRemainingBeforeOrb < 0 && mainContainer.playerCanOrb);
        UiReload.Instance.UpdateGraphics(Mathf.Clamp(reloadingPurcentage, 0, 1), newPerfectPlacement, weapon.perfectRange, haveTriedPerfet);

        if (reloading)
        {
            savedReloadingPurcentage = reloadingPurcentage;
            reloadingPurcentage += Time.unscaledDeltaTime / weapon.reloadingTime;
            if (reloadingPurcentage > 1)
            {
                EndReload(false);
            }
        }
        if (savedReloadingPurcentage > (newPerfectPlacement + weapon.perfectRange)) ReloadValidate();


        // --- Gestion de la lumière sur le gun

        Vector3 v3 = Main.Instance.GetCursorPos() + Vector3.forward * 10; 
        v3 = CameraHandler.Instance.renderingCam.ScreenToWorldPoint(v3);
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, v3 - CameraHandler.Instance.renderingCam.transform.position, 360, 0.0f);
        if (!rotateLocked)
            weaponLight.transform.rotation = Quaternion.LookRotation(newDirection);
        else
            weaponLight.transform.rotation = Quaternion.LookRotation(CameraHandler.Instance.renderingCam.transform.forward, Vector3.up);


        Ray rayFromMouse = CameraHandler.Instance.renderingCam.ScreenPointToRay(Main.Instance.GetCursorPos());

        if (lastFrameLightRayCast < Time.frameCount - shootLightRayCastEvery)
        {
            RaycastHit hitLight;
            if (Physics.Raycast(rayFromMouse, out hitLight, Mathf.Infinity, weapon.maskCheckDistanceForLight))
            {
                lastLightDistance = Mathf.Clamp(hitLight.distance, 0, weapon.distanceMax);
            }
            lastFrameLightRayCast = Time.frameCount;
        }
        if (lastFrameCrosshairRayCast < Time.frameCount - shootCrosshairRayCastEvery)
        {
            RaycastHit hitCrosshair;
            if (Physics.Raycast(rayFromMouse, out hitCrosshair, Mathf.Infinity, weapon.layerMaskHit))
            {
                IBulletAffect bAffect = hitCrosshair.transform.GetComponent<IBulletAffect>();
                crosshairOnEnemy = bAffect != null;
            }
            else 
                crosshairOnEnemy = false;
            lastFrameCrosshairRayCast = Time.frameCount;
        }

        // --- Changement de la light en fonction de la distance
        intensityMultiplier = Mathf.Lerp(intensityMultiplier, lastLightDistance * weapon.distanceIntensityMultiplier, Time.unscaledDeltaTime * weapon.distanceMultiplierLerpSpeed);
        rangeMultipler = Mathf.Lerp(rangeMultipler, lastLightDistance * weapon.distanceRangeMultiplier, Time.unscaledDeltaTime * weapon.distanceMultiplierLerpSpeed);
        weaponLight.spotAngle = Mathf.Lerp(weapon.baseAngle, weapon.chargedAngle, currentChargePurcentage);
        weaponLight.range = Mathf.Lerp(weapon.baseRange, weapon.chargedRange, currentChargePurcentage); // Calcul de la range en fonction de la charge actuelle
        weaponLight.range = weaponLight.range * (1 - weapon.distanceImpactPurcentageOnValueRange) + weaponLight.range * weapon.distanceImpactPurcentageOnValueRange * rangeMultipler; // Calcul de la range en prenant compte la distance avec l'endroit visé
        weaponLight.intensity = Mathf.Lerp(weapon.baseIntensity, weapon.chargedIntensity, currentChargePurcentage); // Calcul de l'intensité en fonction de la charge actuelle
        weaponLight.intensity = weaponLight.intensity * (1 - weapon.distanceImpactPurcentageOnValueIntensity) + weaponLight.intensity * weapon.distanceImpactPurcentageOnValueIntensity * intensityMultiplier; // Calcul de l'intensité en prenant compte la distance avec l'endroit visé

        // Reload sécurité
        if (timeRemainingBeforeCanValidateReload > 0) timeRemainingBeforeCanValidateReload -= Time.unscaledDeltaTime;

        // --- Previsu orbe
        if (displayOrb && timeRemainingBeforeOrb < 0 && orbPrevisu != null)
        {
            Ray rRayGravity = CameraHandler.Instance.renderingCam.ScreenPointToRay(Main.Instance.GetCursorPos());
            //Shoot raycast
            RaycastHit hit;
            if (Time.frameCount % Mathf.CeilToInt(1 / (Time.deltaTime != 0 ? Time.deltaTime : 0.01f) / 15) == 0 || firstRay) 
            {
                if (Physics.Raycast(rRayGravity, out hit, Mathf.Infinity, orbData.layerMask))
                {
                    GameObject hitObj = hit.collider.gameObject;
                    IGravityAffect gAffect = hitObj.GetComponent<IGravityAffect>();
                    if (gAffect != null)
                        rRayGravity.origin += rRayGravity.direction * orbData.zoneMorteDeRayCast;
                }

                if (Physics.Raycast(rRayGravity, out hit, Mathf.Infinity, orbData.layerMask))
                {
                    // Si touche, scale l'orbe pour afficher sa portée et lerp vers la position
                    orbHitSomething = true;
                    orbPrevisu.SetActive(true);
                    lastHitOrbPosition = hit.point;
                    lastHitOrbNormal = hit.normal;
                    firstRay = false;
                }
                else
                {
                    orbHitSomething = false;
                    firstRay = true;
                }
            }
            if (tpOrb)
            {
                tpOrb = false;
                pauseOrbFx = pauseOrbFxTimer;
                orbPrevisu.transform.position = lastHitOrbPosition;
                orbPrevisuFx.Play();
            }
            else if (pauseOrbFx < 0)
            {
                pauseOrbFx = 0;
                orbPrevisuFx.Pause();
            }
            orbPrevisu.transform.position = Vector3.Lerp(orbPrevisu.transform.position, lastHitOrbPosition + lastHitOrbNormal * 1.3f, Time.unscaledDeltaTime * 8);
            orbPrevisu.transform.localScale = Vector3.Lerp(orbPrevisu.transform.localScale, orbHitSomething ? Vector3.one * orbData.holdRange : Vector3.zero, Time.unscaledDeltaTime * 5);
            if (pauseOrbFx > 0) pauseOrbFx -= Time.deltaTime;


        }
        else if (orbPrevisu != null)
        {
            // Si la touche n'est pas appuyé, fait disparaitre
            orbPrevisu.transform.localScale = Vector3.zero;
            orbPrevisu.SetActive(false);
            orbPrevisuFx.Stop();
            tpOrb = true;
            firstRay = true;
            pauseOrbFx = 0;
        }
        //}


        if (gravityOrbRangeDisplay != null)
        {
            if (timerOrb > 0) timerOrb -= Time.deltaTime;

            if (timerOrb < timeToMinSize)
            {
                gravityOrbRangeDisplay.localScale = Vector3.one * sizeAimedOrbVisu * (timerOrb / timeToMinSize);
            }
            else
            {
                gravityOrbRangeDisplay.localScale = Vector3.MoveTowards(gravityOrbRangeDisplay.localScale, Vector3.one * sizeAimedOrbVisu, Time.deltaTime * orbData.gravityBullet_AttractionRange / timeToMaxSize);
            }

        }

        if (isMinigun && minigunCooldownTime > 0)
            minigunCooldownTime -= Time.unscaledDeltaTime;

        customTimeForCursorNoise += Mathf.Lerp(weapon.minImprecisionFrequency, weapon.maxImprecisionFrequency, currentCursorImprecisionPurcentage) * Time.unscaledDeltaTime;

        cursorImprecisionRandomSaved = Vector3.Lerp(cursorImprecisionRandomSaved, cursorImprecisionRandomGoTo, Time.unscaledDeltaTime * weapon.imprecisionCursorLerpSpeed);
        Vector3 currImprecision = cursorImprecisionRandomSaved * Mathf.Lerp(weapon.minImprecision, weapon.maxImprecision, currentCursorImprecisionPurcentage);
        cursorImprecision = currImprecision * Screen.width;
        if (minigunAudioSource != null) minigunAudioSource.pitch = currMinigunRateOfFirePurcentage * maxSpeedMinigunPitch;
        else minigunAudioSource = CustomSoundManager.Instance.PlaySound("Se_Minigun_Engine", "Player", CameraHandler.Instance.renderingCam.transform, 2f, true, 0);

        if (timeRemainingBeforeCanReactivateOrb > 0) timeRemainingBeforeCanReactivateOrb -= Time.unscaledDeltaTime;

    }

    public void SetupOrbRangeDisplay (Transform orbVisu)
    {
        sizeAimedOrbVisu = 0;
        gravityOrbRangeDisplay = orbVisu;
        gravityOrbRangeDisplay.localScale = Vector3.zero;
        sizeAimedOrbVisu = orbData.gravityBullet_AttractionRange;
        timerOrb = orbData.lockTime + orbData.timeBeforeHold;
    }

    public void MakeOrbDisplayDiseapear ()
    {
        timerOrb = 0;
        //gravityOrbRangeDisplay = orbVisu;
    }

    void HitMarkerSoundFunc()
    {
        //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "HitMarker_Boosted", false, 0.5f, 0, 3f, false);
        CustomSoundManager.Instance.PlaySound("HitMarker_Boosted", "PlayerUnpitched", null, 0.5f, false, 1,0f, -.3f);
    }
    public float GetOrbValue()
    {
        return mainContainer.playerCanOrb ? (1 - (timeRemainingBeforeOrb / weapon.gravityOrbCooldown)) : 0;
    }

    public void SetBulletAmmount(int nbBullet, bool doIfReloading)
    {
        if (doIfReloading || !reloading)
        {
            bulletRemaining = nbBullet;
            if (bulletRemaining > weapon.bulletMax) bulletRemaining = weapon.bulletMax;
        }
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
        if (!reloading && (bulletRemaining < weapon.bulletMax || weapon.canReloadAnytime) && currentChargePurcentage ==0 && reloadCoolDown == 0 && !isMinigun)
        {
            //Metrics
            if (Main.Instance.playerCanPerfectReload) MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ReloadWithPerfectActivated);
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.Reload);

            newPerfectPlacement = Mathf.Clamp(weapon.perfectPlacement + UnityEngine.Random.Range(-weapon.perfectRandom, weapon.perfectRandom), 0f, 1);
            CameraHandler.Instance.AddShake(weapon.reloadingStartShake);
            reloading = true;
            haveTriedPerfet = false;
            UiReload.Instance.DisplayGraphics();
            reloadingPurcentage = 0;
            bulletRemaining = 0;
            reloadCoolDown = weapon.reloadCooldown;

            //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "ReloadStart", false, 1f);
            CustomSoundManager.Instance.PlaySound("ReloadStart", "PlayerUnpitched", 1f);

            timeRemainingBeforeCanValidateReload = weapon.reloadSafeCooldownValidate;
        }
    }

    public bool ReloadValidate()
    {
        if (reloading && !haveTriedPerfet && Main.Instance.playerCanPerfectReload)
        {
            if (timeRemainingBeforeCanValidateReload > 0) return false;
            haveTriedPerfet = true;
            if ((reloadingPurcentage > (newPerfectPlacement - weapon.perfectRange) && reloadingPurcentage < (newPerfectPlacement + weapon.perfectRange)) || (savedReloadingPurcentage > (newPerfectPlacement - weapon.perfectRange) && savedReloadingPurcentage < (newPerfectPlacement + weapon.perfectRange)))
            {
                EndReload(true);
            }
            else
            {
                CameraHandler.Instance.AddShake(weapon.reloadingMissTryShake);
                CameraHandler.Instance.AddRecoil(false, weapon.reloadingMissTryRecoil);
                //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Reload_FinishMiss", false, 1f);
                CustomSoundManager.Instance.PlaySound("Reload_FinishMiss", "PlayerUnpitched", 1f);
            }
            return false;
        }
        return true;
    }

    public void EndReload(bool perfect, bool unforced = true)
    {

        if (unforced) TutorialCheckpoint.Instance.PlayerReloaded(perfect);

        reloading = false;
        UiReload.Instance.HideGraphics(perfect, bulletRemaining);
        bulletRemaining = perfect ? weapon.bulletMax + weapon.bulletAddedIfPerfect : weapon.bulletMax;

        if (!isMinigun)
            CameraHandler.Instance.AddShake(perfect ? weapon.reloadingPerfectShake : weapon.reloadingShake);

        if (perfect)
        {
            //Le slow motion est lourd et redondant si on est pas dans l'action. Il est désactivé si on est pas sur une sequence ennemis

            if(SequenceHandler.Instance != null && SequenceHandler.Instance.IsCurrentSequenceOnAction())
            {
                if (!isMinigun)
                {
                    TimeScaleManager.Instance.AddSlowMo(weapon.reloadingPerfectSlowmo, weapon.reloadingPerfectSlowmoDur);
                    CameraHandler.Instance.AddRecoil(false, weapon.reloadingPerfectRecoil, true);
                }
            }
            
            if (unforced)
            {
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.PerfectReload, transform.position);
                CustomSoundManager.Instance.PlaySound("Reload_FinishPerfect", "PlayerUnpitched", 1f);
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.PerfectReload);
            }
            //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Reload_FinishPerfect", false, 1f);
        }
        else
        {
            //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Reload_Finish", false, 1f);
            if (unforced) CustomSoundManager.Instance.PlaySound("Reload_Finish", "PlayerUnpitched", 1f);
        }
    }

    public float GetChargeValue()
    {
        return currentChargePurcentage;
    }

    public bool GravityOrbInput()
    {
        //if (!reloading)
        //{
        if (timeRemainingBeforeOrb < 0)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedGravity);

            currentOrb = Instantiate(orbPrefab);
            currentOrb.GetComponent<GravityOrb>().OnSpawning(Main.Instance.GetCursorPos());
            timeRemainingBeforeOrb = weapon.gravityOrbCooldown;
            timeRemainingBeforeCanReactivateOrb = orbData.timeSafeInputBeforeCanReactivate;
            return true;
        }
        else if (currentOrb != null && weapon.gravityOrbCanBeReactivated && !currentOrb.GetComponent<GravityOrb>().hasExploded)
        {
            if (timeRemainingBeforeCanReactivateOrb <= 0)
            {
                TutorialCheckpoint.Instance.PlayerUsedZeroG();
                currentOrb.GetComponent<GravityOrb>().StopHolding();
            }
            return true;
        }
        //}
        return false;
    }

    public void InputHold(Vector2 mousePosition)
    {
        if (!reloading && bulletRemaining >= (ignoreBulletLimitForCharge ? 1 : weapon.chargedShot.bulletCost) && !isMinigun)
        {
            if (currentChargePurcentage < 1)
            {
                currentChargePurcentage += (weapon.chargeSpeedIndependantFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / weapon.chargeTime;
                if (currentChargePurcentage > 1)
                {
                    UiCrossHair.Instance.JustFinishedCharging();
                    //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Charged_Shotgun", false, 1f, 0.1f);
                    CustomSoundManager.Instance.PlaySound("Charged_Shotgun", "Player", null, 1f,false,1,0.1f);
                    currentChargePurcentage = 1;
                }
            }
        }
        if (isMinigun)
        {
            currentCursorImprecisionPurcentage = Mathf.MoveTowards(currentCursorImprecisionPurcentage, 1, Time.unscaledDeltaTime / weapon.timeToMaxImprecision);
            currMinigunRateOfFirePurcentage = Mathf.MoveTowards(currMinigunRateOfFirePurcentage, 1, Time.unscaledDeltaTime / weapon.minigunRoFTimeToGoUp);
            if (minigunCooldownTime <= 0)
            {
                float currMinigunCooldown = minigunCooldownTime;
                for (currMinigunCooldown = minigunCooldownTime; currMinigunCooldown <= 0; currMinigunCooldown += 1 / Mathf.Lerp(weapon.minigunMinRateOfFire, weapon.minigunMaxRateOfFire, currMinigunRateOfFirePurcentage))
                {
                    //cursorImprecisionRandomGoTo = GetPerlinVectorThree();
                    //cursorImprecisionRandomGoTo = new Vector3((UnityEngine.Random.Range(0f, 1f) - 0.5f) * 2, (UnityEngine.Random.Range(0f, 1f) - 0.5f) * 2, (UnityEngine.Random.Range(0f, 1f) - 0.5f) * 2);

                    cursorImprecisionRandomGoTo = GetPerlinVectorThree() * weapon.purcentageNoiseOnImprecision + new Vector3((UnityEngine.Random.Range(0f, 1f) - 0.5f) * 2, (UnityEngine.Random.Range(0f, 1f) - 0.5f) * 2, (UnityEngine.Random.Range(0f, 1f) - 0.5f) * 2).normalized * (1 - weapon.purcentageNoiseOnImprecision);
                    OnShoot(mousePosition, minigunMod);
                }
                minigunCooldownTime = currMinigunCooldown;
            }
        }
    }

    public void InputUnHold()
    {
        currentCursorImprecisionPurcentage = Mathf.MoveTowards(currentCursorImprecisionPurcentage, 0, Time.unscaledDeltaTime / weapon.timeToMinImprecision);
        currMinigunRateOfFirePurcentage = Mathf.MoveTowards(currMinigunRateOfFirePurcentage, 0, Time.unscaledDeltaTime / weapon.minigunRoFTimeToGoDown);
    }

    public void InputUp(Vector2 mousePosition)
    {
        if (!reloading && !isMinigun)
        {
            DataWeaponMod currentWeaponMod = null;
            if (currentChargePurcentage == 1)
            {
                currentWeaponMod = weapon.chargedShot;
                TutorialCheckpoint.Instance.PlayerUsedShotGun();
            }
            else currentWeaponMod = weapon.baseShot;


            currentChargePurcentage = 0;
            OnShoot(mousePosition, currentWeaponMod);
        }
        else if (reloading && !isMinigun && CustomSoundManager.Instance != null) CustomSoundManager.Instance.PlaySound("SE_NoBullet", "PlayerUnpitched", 2);
    }

    public void CanNotShoot()
    {
        if (currentChargePurcentage > 0)
        {
            currentChargePurcentage -= (weapon.chargeSpeedIndependantFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / weapon.chargeTime;
            if (currentChargePurcentage < 0) currentChargePurcentage = 0;
        }
    }


    public bool CheckIfModIsMinigun(DataWeaponMod weaponMod) { return weaponMod == minigunMod; }

    private void OnShoot(Vector2 mousePosition, DataWeaponMod weaponMod, bool cantHit = false, bool canPlaySound = true)
    {
        if (bulletRemaining > 0)
        {
            if (weaponMod != minigunMod)
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.Shoot);

            List<Ray> bounceCalculations = new List<Ray>();
            if(weaponMod == weapon.chargedShot)
            {
                shotGunHasHit = false;
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.UsedShotgun);
                Invoke("CheckIfShotGunHasHit", .1f);
            }
            if (UiDamageHandler.Instance != null)
                UiDamageHandler.Instance.MuzzleFlashFunc();

            bool hasPlayedHitMarkerSound = false;
            bool hasPlayedHitSound = false;
            int nbShotGunTouched = 0;
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
                if (Physics.Raycast(rayBullet, out hit, Mathf.Infinity, weapon.layerMaskHit) && !cantHit)
                {
                    FxImpactDependingOnSurface(hit.transform.gameObject, hit.point, weaponMod, 0.2f, rayBullet, hit, hasPlayedHitSound);
                    hasPlayedHitSound = true;
                    CheckIfMustSlowMo(hit.transform.gameObject, weaponMod);
                    if (TrailManager.Instance != null ) TrailManager.Instance.RequestBulletTrail(rayBullet.origin, hit.point);
                    else Debug.Log("No Trail Manager");

                    //Debug.Log("Hit Someting " + hit.transform.gameObject.name);


                    IBulletAffect bAffect = hit.transform.GetComponent<IBulletAffect>();
                    if (bAffect != null)
                    {
                        bAffect.OnHit(weaponMod, hit.point, i==0 ? weaponMod.bullet.damage * weaponMod.firstBulletDamageMultiplier : weaponMod.bullet.damage, rayBullet);
                        if (weaponMod == weapon.baseShot || weaponMod == minigunMod) 
                        {
                            bAffect.OnHitSingleShot(weaponMod);
                        }
                        if (weaponMod == weapon.chargedShot)
                        {
                            bAffect.OnHitShotGun(weaponMod); 
                            shotGunHasHit = true;
                        }

                        if (weaponMod == weapon.chargedShot)
                        {
                            nbShotGunTouched++;
                        }

                            if (!hasPlayedHitMarkerSound && canPlaySound)
                            Invoke("HitMarkerSoundFunc", 0.05f * Time.timeScale);
                        hasPlayedHitMarkerSound = true;

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
            bulletRemaining -= Main.Instance.playerInLeaderboard ? 0 : weaponMod.bulletCost;

            if (bulletRemaining <= 1 && isMinigun)
                EndReload(true, false);

            if (bulletRemaining < 0) bulletRemaining = 0;
            //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, weaponMod == weapon.chargedShot ? weapon.chargedShot.soundPlayed : weapon.baseShot.soundPlayed, false, weaponMod == weapon.chargedShot ?  0.8f : 0.4f, 0.2f);
            if (canPlaySound)
            {
                if (weaponMod == minigunMod)
                    CustomSoundManager.Instance.PlaySound(minigunMod.soundPlayed, "Player", null, 0.6f, false, 0.8f, .2f);
                else
                    CustomSoundManager.Instance.PlaySound(weaponMod == weapon.chargedShot ? weapon.chargedShot.soundPlayed : weapon.baseShot.soundPlayed, "Player", null, weaponMod == weapon.chargedShot ? 0.8f : 0.4f, false, 1, .2f);
            }

        }
        else
        {
            //CameraHandler.Instance.AddShake(weapon.shakeIfNoBullet,0.1f);
            //CameraHandler.Instance.AddRecoil(false, weapon.recoilIfNoBullet);
        }
    }

    private void CheckIfMustSlowMo(GameObject hit, DataWeaponMod weaponMod)
    {
        if (hit.GetComponent<Enemy<DataEnemy>>() != null && weaponMod.bullet.activateSlowMoAtImpact)
            TimeScaleManager.Instance.AddSlowMo(weaponMod.bullet.slowMoPower, weaponMod.bullet.slowMoDuration, 0, weaponMod.bullet.slowMoProbability);
        if (hit.GetComponent<Enemy<DataEnemy>>() != null && weaponMod.bullet.activateStopTimeAtImpact)
            TimeScaleManager.Instance.AddStopTime(weaponMod.bullet.timeStopAtImpact, 0, weaponMod.bullet.timeStopProbability);
    }

    private void FxImpactDependingOnSurface(GameObject hit, Vector3 hitPoint, DataWeaponMod weaponMod, float castradius, Ray raybase, RaycastHit hitBase, bool hasAlreadyPlayedSound)
    {
        bool found = false;
        for (int i = 0; i < weaponMod.bullet.bulletFxs.allFxReaction.Length; i++)
        {
            if (hit.tag == weaponMod.bullet.bulletFxs.allFxReaction[i].tag)
            {
                PlayFxsDecalsAndSound(weaponMod.bullet.bulletFxs.allFxReaction[i], hitBase, raybase, hasAlreadyPlayedSound);
                found = true;
                break;
            }
        }
        if (!found)
        {
            for (int i = 0; i < weaponMod.bullet.bulletFxs.allFxReaction.Length; i++)
            {
                if (weaponMod.bullet.bulletFxs.allFxReaction[i].mask == (weaponMod.bullet.bulletFxs.allFxReaction[i].mask | (1 << hit.layer)))
                {
                    PlayFxsDecalsAndSound(weaponMod.bullet.bulletFxs.allFxReaction[i], hitBase, raybase, hasAlreadyPlayedSound);
                    break;
                }
            }
        }
    }

    void PlayFxsDecalsAndSound(DataBulletFx data, RaycastHit hitBase, Ray raybase, bool hasAlreadyPlayedSound)
    {
        FxManager.Instance.PlayFx(data.fxName, hitBase, raybase);
        DecalManager.Instance.ProjectDecal(hitBase, data.decalName, data.decalSizeMultiplier);
        if (!hasAlreadyPlayedSound) StartCoroutine(BulletSoundCoroutine(data));
    }

    IEnumerator BulletSoundCoroutine(DataBulletFx data)
    {
        yield return new WaitForSecondsRealtime(data.delayBeforeSound);
        CustomSoundManager.Instance.PlaySound(data.soundPlayed, data.mixerPlayed, CameraHandler.Instance.renderingCam.transform, data.soundVolume, false, 1, data.soundPitchRandom);
        yield break;
    }

    private void CheckIfShotGunHasHit()
    {
        if (!shotGunHasHit)
        {
            //PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.MissShotGun, transform.position);
        }
        else
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);
        }
    }

    public void SetMinigun (bool enabled)
    {
        isMinigun = enabled;
        if (enabled)
        {
            EndReload(true, false);
            currentChargePurcentage = 0;
        }
    }

    public void OnShotGunHitTarget()
    {
        //shotGunHasHit = true;
    }

    //public void JustDestroyedBodyPart(Vector3 pos)
    //{
    //    Debug.Log("ReShoot");
    //    OnShoot(CameraHandler.Instance.renderingCam.WorldToScreenPoint(pos), weapon.baseShot, false, false);
    //}

    public void EnableDisableRevealLight(bool activation)
    {
        weaponLight.gameObject.SetActive(activation);
    }
    float GetFloat(float seed) { return (Mathf.PerlinNoise(seed, customTimeForCursorNoise) - 0.5f) * 2f; }
    Vector3 GetPerlinVectorThree() { return new Vector3(GetFloat(16), GetFloat(34), GetFloat(85)); }

    IEnumerator BounceBullets(List<Ray> bounces, float bounceLag)
    {
        yield return new WaitForSeconds(bounceLag);

        //On divise par 2 lol
        DataWeaponMod bounceMod = Instantiate(weapon.chargedShot);
        bounceMod.bullet.damage /= 2;

        bool hasPlayedHitSound = false;
        foreach (Ray bounceBullet in bounces)
        {
            RaycastHit hit;
            if (Physics.Raycast(bounceBullet, out hit, Mathf.Infinity, weapon.layerMaskHit))
            {
                FxImpactDependingOnSurface(hit.transform.gameObject, hit.point, bounceMod, 0.2f, bounceBullet, hit, hasPlayedHitSound);
                hasPlayedHitSound = true;
                CheckIfMustSlowMo(hit.transform.gameObject, bounceMod);
                if (TrailManager.Instance != null) TrailManager.Instance.RequestBulletTrail(bounceBullet.origin, hit.point);
                else Debug.Log("No Trail Manager");

                IBulletAffect bAffect = hit.transform.GetComponent<IBulletAffect>();
                if (bAffect != null)
                {
                    bAffect.OnHit(bounceMod, hit.point, bounceMod.bullet.damage, bounceBullet);

                    bAffect.OnHitShotGun(bounceMod);


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
