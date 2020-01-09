using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityOrb : MonoBehaviour
{
    [SerializeField]
    private DataGravityOrb hGOrb = null;

    float fTimeHeld = 0f;
    bool hasSticked = false;

    [SerializeField]
    public bool bActivedViaScene = false;

    [SerializeField]
    bool bZeroActivateAutomaticaly = true;

    GameObject parentIfSticky = null;
    Camera MainCam = null;

    private void Start()
    {
        /*
        if (bActivedViaScene)
           Invoke("SpawnViaScene", 1);
           */
    }

    public bool OnSpawning(Vector2 mousePosition)
    {

        MainCam = Camera.main;
        Ray rRayGravity = MainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        //hGOrb = weaponIndex == 0 ? hGOrbOne : hGOrbSecond;

        if (Physics.Raycast(rRayGravity, out hit, Mathf.Infinity, hGOrb.layerMask))
        {
            this.transform.position = hit.point;

            GameObject hitObj = hit.collider.gameObject;
            Debug.Log("Raycast has hit");
            IGravityAffect gAffect = hitObj.GetComponent<IGravityAffect>();


            if (hGOrb.bIsSticky && hitObj != null && gAffect != null)
            {
                gAffect.OnGravityDirectHit();

                this.transform.SetParent(hitObj.transform);
                parentIfSticky = hitObj;
                parentIfSticky.GetComponent<Rigidbody>().isKinematic = true;
                hasSticked = true;
            }

            
            this.OnAttractionStart();

            FxManager.Instance.PlayFx("VFX_GravityOrb", hit.point, Quaternion.identity);

            StartCoroutine("OnHoldAttraction");

            //CustomSoundManager.Instance.PlaySound(MainCam.gameObject, "Sound_Orb_Boosted", false, 0.5f);
            return true;
            
        }


        return false;

    }
    
    public void SpawnViaScene()
    {
        FxManager.Instance.PlayFx("VFX_GravityOrb", hit.point, Quaternion.identity);
        this.OnAttractionStart();
        StartCoroutine("OnHoldAttraction");
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Sound_Orb_Boosted", false, 0.3f);
    }

    void OnAttractionStart()
    {
        //Debug.Log("Attraction");
        Collider[] tHits = Physics.OverlapSphere(this.transform.position, hGOrb.fGravityBullet_AttractionRange);

        foreach (Collider hVictim in tHits)
        {
            /*
            if (hVictim.GetComponent<C_ShooterBullet>())
                hVictim.GetComponent<C_ShooterBullet>().OnGravityPull();
            */

            IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();

            if (gAffect != null && hVictim.gameObject != parentIfSticky)
                gAffect.OnPull(this.transform.position, hGOrb.fPullForce);
            
        }

    }

    public void StopHolding()
    {
        int nbEnemiesHitByFloatExplo = 0;
        StopCoroutine("OnHoldAttraction");

        if (hasSticked)
            ReactGravity.DoUnfreeze(parentIfSticky.GetComponent<Entity>());

        if (hGOrb.bIsExplosive)
        {
            //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Sounf_Orb_NoGrav_Boosted", false, 0.3f);
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, hGOrb.fGravityBullet_AttractionRange);

            foreach (Collider hVictim in tHits)
            {
                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();


                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnPull(this.transform.position + hGOrb.v3OffsetExplosion, -hGOrb.fExplosionForce);
                    gAffect.OnRelease();

                    if (hGOrb.bIsFloatExplosion)
                    {
                        gAffect.OnFloatingActivation(hGOrb.bUpwardsForceOnFloat, hGOrb.tTimeBeforeFloatActivate, hGOrb.bIsSlowedDownOnFloat, hGOrb.tFloatTime, hGOrb.bZeroGIndependantTimeScale);

                        /*
                        if (hVictim.GetComponent<C_Enemy>() != null)
                        {
                            nbEnemiesHitByFloatExplo++;
                        }
                        */
                    }

                }
                
            }
            if (hGOrb.bIsExplosive)
            {
                 FxManager.Instance.PlayFx("VFX_GravityOrb", transform.position, Quaternion.identity);
                float newDuration = hGOrb.fSlowMoDuration;

                newDuration *= (nbEnemiesHitByFloatExplo == 0 ? 0 : 1 + (nbEnemiesHitByFloatExplo * .03f));

                //GameObject.FindObjectOfType<C_TimeScale>().AddSlowMo(hGOrb.fSlowMoPower, newDuration, hGOrb.tTimeBeforeFloatActivate, hGOrb.fSlowMoProbability);
            }
        }


        Destroy(this.gameObject);
    }

    IEnumerator OnHoldAttraction()
    {
        new WaitForSecondsRealtime(hGOrb.fTimeBeforeHold);

        fTimeHeld = 0;

        while (true)
        {
            //Debug.Log("Attraction");
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, hGOrb.fHoldRange);

            foreach (Collider hVictim in tHits)
            {
                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();

                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnPull(this.transform.position, hGOrb.fPullForce);
                    gAffect.OnHold();
                }
                    

            }

            fTimeHeld += hGOrb.fWaitingTimeBetweenAttractions;


            if (fTimeHeld >= hGOrb.fLockTime)
            {
                StopHolding();
            }

            yield return new WaitForSecondsRealtime(hGOrb.fWaitingTimeBetweenAttractions);
        }
        

    }
    
}
