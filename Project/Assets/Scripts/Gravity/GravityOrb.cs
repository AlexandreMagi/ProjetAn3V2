using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrb : MonoBehaviour
{
    [SerializeField]
    private DataGravityOrb orbData = null;

    float fTimeHeld = 0f;
    bool hasSticked = false;
    bool hasHitSomething = false;

    [SerializeField]
    public bool bActivedViaScene = false;

    //[SerializeField]
    //bool bZeroActivateAutomaticaly = true;

    GameObject parentIfSticky = null;
    Camera MainCam = null;

    [HideInInspector]
    public bool hasExploded = false;

    private void Start()
    {

        if (bActivedViaScene)
           Invoke("SpawnViaScene", 1);

    }

    public bool OnSpawning(Vector2 mousePosition)
    {
        hasHitSomething = false;
        MainCam = CameraHandler.Instance.renderingCam;
        Ray rRayGravity = MainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        //orbData = weaponIndex == 0 ? orbDataOne : orbDataSecond;

        if (Physics.Raycast(rRayGravity, out hit, Mathf.Infinity, orbData.layerMask))
        {
            UiDamageHandler.Instance.GravityFlash(orbData.flashScreen);
            this.transform.position = hit.point;

            GameObject hitObj = hit.collider.gameObject;
            IGravityAffect gAffect = hitObj.GetComponent<IGravityAffect>();


            if (orbData.isSticky && hitObj != null && gAffect != null)
            {
                gAffect.OnGravityDirectHit();

                this.transform.SetParent(hitObj.transform);
                parentIfSticky = hitObj;
                hasSticked = true;
            }



            this.OnAttractionStart();

            //FxManager.Instance.PlayFx(orbData.fxName, hit.point, Quaternion.identity);//, orbData.gravityBullet_AttractionRange, orbData.fxSizeMultiplier);
            FxManager.Instance.PlayFx(orbData.fxName, hit.point, hit, true);//, orbData.gravityBullet_AttractionRange, orbData.fxSizeMultiplier);

            StartCoroutine("OnHoldAttraction");

            CustomSoundManager.Instance.PlaySound(MainCam.gameObject, "Sound_Orb_Boosted", false, 0.5f);
            return true;
            
        }

        PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.MissGravityOrb, Vector3.zero);
        return false;

    }
    
    public void SpawnViaScene()
    {
        //GameObject.FindObjectOfType<C_Fx>().GravityOrbFx(transform.position);

        //FxManager.Instance.PlayFx(orbData.fxName, transform.position, Quaternion.identity, orbData.gravityBullet_AttractionRange, orbData.fxSizeMultiplier);

        this.OnAttractionStart();
        StartCoroutine("OnHoldAttraction");
        CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Sound_Orb_Boosted", false, 0.3f);
    }

    void OnAttractionStart()
    {
        //Debug.Log("Attraction");
        Collider[] tHits = Physics.OverlapSphere(this.transform.position, orbData.gravityBullet_AttractionRange);

        foreach (Collider hVictim in tHits)
        {
            /*
            if (hVictim.GetComponent<C_ShooterBullet>())
                hVictim.GetComponent<C_ShooterBullet>().OnGravityPull();
            */

            IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();

            if (gAffect != null && hVictim.gameObject != parentIfSticky)
            {
                hasHitSomething = true;
                gAffect.OnPull(this.transform.position, orbData.pullForce);
            }
        }
    }

    public DataGravityOrb getOrbData () { return orbData; }

    public void StopHolding(bool loosePointIfMissed = true)
    {
        hasExploded = true;
        int nbEnemiesHitByFloatExplo = 0;

        PostprocessManager.Instance.setChroma(true);
        PostprocessManager.Instance.doDistortion();

        StopCoroutine("OnHoldAttraction");


        if (!hasHitSomething && loosePointIfMissed)
        {
            PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.MissGravityOrb, transform.position);
        }

        if (hasSticked)
            ReactGravity<DataEntity>.DoUnfreeze(parentIfSticky.GetComponent<Rigidbody>());

        if (this.parentIfSticky != null)
        {
            IGravityAffect parentGravityAffect = parentIfSticky.GetComponent<IGravityAffect>();

            if (parentGravityAffect != null)
                parentGravityAffect.OnRelease();
        }

        if (orbData.isExplosive)
        {

            CameraHandler.Instance.AddShake(orbData.zeroGCamShake, orbData.zeroGCamShakeTime);
            CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "Sounf_Orb_NoGrav_Boosted", false, 0.3f);
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, orbData.gravityBullet_AttractionRange);

            if (tHits.Length > 0)
            {
                Invoke("OnZeroGRelease", orbData.floatTime);
            }

            foreach (Collider hVictim in tHits)
            {
                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();


                if (hVictim.GetComponent<Prop>()) hVictim.GetComponent<Prop>().SetTimerToRelease(orbData.floatTime);
                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnPull(this.transform.position + orbData.offsetExplosion, -orbData.explosionForce);
                    gAffect.OnRelease();

                    if (orbData.isFloatExplosion)
                    {
                        gAffect.OnFloatingActivation(orbData.upwardsForceOnFloat, orbData.timeBeforeFloatActivate, orbData.isSlowedDownOnFloat, orbData.floatTime, orbData.zeroGIndependantTimeScale);
                        /*
                        if (hVictim.GetComponent<C_Enemy>() != null)
                        {
                            nbEnemiesHitByFloatExplo++;
                        }
                        */
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
    }

    void OnZeroGRelease()
    {
        PostprocessManager.Instance.setChroma(false);
        Destroy(this.gameObject);
    }

    IEnumerator OnHoldAttraction()
    {
        new WaitForSecondsRealtime(orbData.timeBeforeHold);

        fTimeHeld = 0;

        while (true)
        {
            //Debug.Log("Attraction");
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, orbData.holdRange);

            foreach (Collider hVictim in tHits)
            {
                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();

                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnPull(this.transform.position, orbData.pullForce);
                    gAffect.OnHold();
                    hasHitSomething = true;
                }
                    

            }

            if (!bActivedViaScene)
            {
                fTimeHeld += orbData.waitingTimeBetweenAttractions;

                if (fTimeHeld >= orbData.lockTime)
                {
                    StopHolding();
                }
            }


            yield return new WaitForSecondsRealtime(orbData.waitingTimeBetweenAttractions);
        }
        

    }
    
}
