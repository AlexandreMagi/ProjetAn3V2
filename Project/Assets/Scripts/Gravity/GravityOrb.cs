using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrb : MonoBehaviour
{
    [SerializeField]
    private DataGravityOrb orbData = null;
    public DataGravityOrb OrbData { get { return orbData;  } }

    List<Collider> collidersToAttract = null;

    float fTimeHeld = 0f;
    float fTimeHeldThreshold = 0f;
    float fTimeThresholdResetTargets = 0.4f;
    float stepTimeHold = 0f;
    bool hasSticked = false;
    bool hasAttractionStarted = false;

    bool hasDoneFirstImpulsion = false;
    //bool hasHitSomething = false;

    [SerializeField]
    public bool bActivedViaScene = false;

    //[SerializeField]
    //bool bZeroActivateAutomaticaly = true;

    GameObject parentIfSticky = null;
    Camera MainCam = null;

    [HideInInspector]
    public bool hasExploded = false;

    ParticleSystem ps = null;

    private void Start()
    {
        if (bActivedViaScene)
           Invoke("SpawnViaScene", 1);

        //Debug.Log("start");
    }

    public bool OnSpawning(Vector2 mousePosition)
    {
        collidersToAttract = new List<Collider>();
        hasAttractionStarted = false;
        hasDoneFirstImpulsion = false;
        stepTimeHold = 0f;

        //hasHitSomething = false;
        MainCam = CameraHandler.Instance.renderingCam;
        Ray rRayGravity = MainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(rRayGravity, out hit, Mathf.Infinity, orbData.layerMask))
        {
            GameObject hitObj = hit.collider.gameObject;
            IGravityAffect gAffect = hitObj.GetComponent<IGravityAffect>();
            if (gAffect != null)
                rRayGravity.origin += rRayGravity.direction * orbData.zoneMorteDeRayCast;
        }


        //orbData = weaponIndex == 0 ? orbDataOne : orbDataSecond;

        if (Physics.Raycast(rRayGravity, out hit, Mathf.Infinity, orbData.layerMask))
        {
            TutorialCheckpoint.Instance.PlayerUsedOrb();
            UiDamageHandler.Instance.GravityFlash(orbData.flashScreen);
            this.transform.position = hit.point;
            if (PostprocessManager.Instance!=null)PostprocessManager.Instance.doDistortion(transform);

            GameObject hitObj = hit.collider.gameObject;
            IGravityAffect gAffect = hitObj.GetComponent<IGravityAffect>();


            if (orbData.isSticky && hitObj != null && gAffect != null)
            {
                gAffect.OnGravityDirectHit();

                this.transform.SetParent(hitObj.transform);
                parentIfSticky = hitObj;
                hasSticked = true;
            }

            //FxManager.Instance.PlayFx(orbData.fxName, hit.point, Quaternion.identity);//, orbData.gravityBullet_AttractionRange, orbData.fxSizeMultiplier);
            ps = FxManager.Instance.PlayFx(orbData.fxName, hit.point, hit, true);//, orbData.gravityBullet_AttractionRange, orbData.fxSizeMultiplier);


            hasAttractionStarted = true;

            CustomSoundManager.Instance.PlaySound("Sound_Orb_Boosted", "Effect", .5f);
            return true;
            
        }

        //PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.MissGravityOrb, Vector3.zero);
        return false;

    }
    
    public void SpawnViaScene()
    {
        //GameObject.FindObjectOfType<C_Fx>().GravityOrbFx(transform.position);

        //FxManager.Instance.PlayFx(orbData.fxName, transform.position, Quaternion.identity, orbData.gravityBullet_AttractionRange, orbData.fxSizeMultiplier);

        hasAttractionStarted = true;
        CustomSoundManager.Instance.PlaySound("Sound_Orb_Boosted", "Effect", .3f);
    }

    public DataGravityOrb getOrbData () { return orbData; }

    public void StopHolding(bool loosePointIfMissed = true)
    {
        hasExploded = true;
        hasAttractionStarted = false;
        int nbEnemiesHitByFloatExplo = 0;

        if (PostprocessManager.Instance != null)
        {
            PostprocessManager.Instance.setChroma(true);
            PostprocessManager.Instance.doDistortion(transform);
        }

        //StopCoroutine("OnHoldAttraction");


        //if (!hasHitSomething && loosePointIfMissed)
        //{
        //    PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.MissGravityOrb, transform.position);
        //}

        if (hasSticked && parentIfSticky != null)
            ReactGravity<DataEntity>.DoUnfreeze(parentIfSticky.GetComponent<Rigidbody>());

        if (this.parentIfSticky != null)
        {
            IGravityAffect parentGravityAffect = parentIfSticky.GetComponent<IGravityAffect>();

            if (parentGravityAffect != null)
                parentGravityAffect.OnRelease();
        }

        foreach(Collider col in collidersToAttract)
        {
            if(col.GetComponent<Shooter>() != null)
            {
                col.GetComponent<Shooter>().OnRelease();
            }
        }

        if (orbData.isExplosive && (Main.Instance == null || Main.Instance.playerCanZeroG))
        {
            if (ps != null && ps.isEmitting)
                ps.Stop();
            CameraHandler.Instance.AddShake(orbData.zeroGCamShake, orbData.zeroGCamShakeTime);
            CustomSoundManager.Instance.PlaySound("Sounf_Orb_NoGrav_Boosted", "Effect", .3f);
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, orbData.gravityBullet_AttractionRange*3);

            if (tHits.Length > 0)
            {
                Invoke("OnZeroGRelease", orbData.floatTime);
            }

            foreach (Collider hVictim in tHits)
            {
                if (hVictim == null) continue;

                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();


                if (hVictim.GetComponent<Prop>()) hVictim.GetComponent<Prop>().SetTimerToRelease(orbData.floatTime);

                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnRelease();

                    if (Vector3.Distance(this.transform.position, hVictim.transform.position) <= orbData.gravityBullet_AttractionRange)
                    {
                        gAffect.OnPull(this.transform.position + orbData.offsetExplosion, -orbData.explosionForce, true);

                        if (orbData.isFloatExplosion)
                        {
                            gAffect.OnFloatingActivation(orbData.upwardsForceOnFloat, orbData.timeBeforeFloatActivate, orbData.isSlowedDownOnFloat, orbData.floatTime, orbData.zeroGIndependantTimeScale);
                            /*
                            if (hVictim.GetComponent<C_Enemy>() != null)
                            {
                                nbEnemiesHitByFloatExplo++;
                            }
                            */


                            gAffect.OnZeroG();
                        }
                    }
                   
                }

            }
            if (!bActivedViaScene)
                FxManager.Instance.PlayFx(orbData.fxExplosionName, transform.position, Quaternion.identity, orbData.gravityBullet_AttractionRange, orbData.fxSizeMultiplier);
            float newDuration = orbData.slowMoDuration;

            newDuration *= (nbEnemiesHitByFloatExplo == 0 ? 0 : 1 + (nbEnemiesHitByFloatExplo * .03f));

            if(SequenceHandler.Instance != null && !SequenceHandler.Instance.isWaitingTimer)
                TimeScaleManager.Instance.AddSlowMo(orbData.slowMoPower, newDuration, orbData.timeBeforeFloatActivate, orbData.slowMoProbability);

        }
        else
        {
            OnZeroGRelease();
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, orbData.gravityBullet_AttractionRange*2);
            foreach (Collider hVictim in tHits)
            {
                if (hVictim == null) continue;

                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();
                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnRelease();

                }

            }
        }
    }

    void OnZeroGRelease()
    {
        foreach(Collider col in collidersToAttract)
        {
            if (col == null) continue;

            IGravityAffect gAffect = col.GetComponent<IGravityAffect>();
            if (gAffect != null)
            {
                gAffect.OnZeroGRelease();

            }
        }

        if (PostprocessManager.Instance != null) PostprocessManager.Instance.setChroma(false);
        Destroy(this.gameObject);
    }

    void Attract(float force)
    {
        try
        {
            if (collidersToAttract != null)
            {
                foreach (Collider hVictim in collidersToAttract)
                {
                    if (hVictim == null)
                    {
                        collidersToAttract.Remove(hVictim);
                        continue;
                    }
                    else
                    {
                        IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();
                        if (gAffect != null && hVictim.gameObject != parentIfSticky && hVictim.gameObject.activeSelf)
                        {
                            //Debug.Log("pull");
                            gAffect.OnPull(this.transform.position, force, false);
                            gAffect.OnHold();
                            //hasHitSomething = true;
                        }
                    }
                }
            }
        }
        catch (Exception e) { //Debug.LogError(e);
        }

        
    }

    public void FixedUpdate()
    {
        //Debug.Break();
       // Debug.Log("Truc");

        //yield return new WaitForSecondsRealtime(orbData.timeBeforeHold);
        if (hasAttractionStarted)
        {

            if (!hasDoneFirstImpulsion)
            {
                fTimeHeld = 0;
                hasDoneFirstImpulsion = true;

                Collider[] tHits = Physics.OverlapSphere(this.transform.position, orbData.holdRange);
                if (collidersToAttract != null ) collidersToAttract.AddRange(tHits);

                Attract(orbData.pullForce);
            }
            else
            {
                if (!bActivedViaScene)
                {
                    fTimeHeld += Time.fixedDeltaTime;
                    fTimeHeldThreshold += Time.fixedDeltaTime;
                    stepTimeHold += Time.fixedDeltaTime;

                    if (fTimeHeld >= orbData.lockTime)
                    {
                        StopHolding();
                    }

                    if(stepTimeHold >= orbData.waitingTimeBetweenAttractions)
                    {
                        stepTimeHold -= orbData.waitingTimeBetweenAttractions;

                        Attract(orbData.holdForce);
                    }

                    /*
                    if(fTimeHeldThreshold >= fTimeThresholdResetTargets)
                    {
                        fTimeHeldThreshold -= fTimeThresholdResetTargets;
                        
                        Collider[] tHits = Physics.OverlapSphere(this.transform.position, orbData.holdRange);
                        if (collidersToAttract != null)
                        {
                            collidersToAttract.Clear();
                            collidersToAttract.AddRange(tHits);
                        }
                    }
                    */
                }

            }

        }

    }
    
}
