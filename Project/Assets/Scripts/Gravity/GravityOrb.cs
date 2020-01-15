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

    //[SerializeField]
    //bool bZeroActivateAutomaticaly = true;

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

        MainCam = CameraHandler.Instance.RenderingCam.GetComponent<Camera>();
        Ray rRayGravity = MainCam.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        //hGOrb = weaponIndex == 0 ? hGOrbOne : hGOrbSecond;

        if (Physics.Raycast(rRayGravity, out hit, Mathf.Infinity, hGOrb.layerMask))
        {
            this.transform.position = hit.point;

            GameObject hitObj = hit.collider.gameObject;
            Debug.Log("Raycast has hit");
            IGravityAffect gAffect = hitObj.GetComponent<IGravityAffect>();


            if (hGOrb.isSticky && hitObj != null && gAffect != null)
            {
                gAffect.OnGravityDirectHit();

                this.transform.SetParent(hitObj.transform);
                parentIfSticky = hitObj;
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
        //GameObject.FindObjectOfType<C_Fx>().GravityOrbFx(transform.position);

        FxManager.Instance.PlayFx("VFX_GravityOrb", transform.position, Quaternion.identity);

        this.OnAttractionStart();
        StartCoroutine("OnHoldAttraction");
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Sound_Orb_Boosted", false, 0.3f);
    }

    void OnAttractionStart()
    {
        //Debug.Log("Attraction");
        Collider[] tHits = Physics.OverlapSphere(this.transform.position, hGOrb.gravityBullet_AttractionRange);

        foreach (Collider hVictim in tHits)
        {
            /*
            if (hVictim.GetComponent<C_ShooterBullet>())
                hVictim.GetComponent<C_ShooterBullet>().OnGravityPull();
            */

            IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();

            if (gAffect != null && hVictim.gameObject != parentIfSticky)
                gAffect.OnPull(this.transform.position, hGOrb.pullForce);
            
        }

    }

    public void StopHolding()
    {
        int nbEnemiesHitByFloatExplo = 0;
        StopCoroutine("OnHoldAttraction");

        if (hasSticked)
            ReactGravity<DataEntity>.DoUnfreeze(parentIfSticky.GetComponent<Rigidbody>());

        if (hGOrb.isExplosive)
        {
            //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "Sounf_Orb_NoGrav_Boosted", false, 0.3f);
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, hGOrb.gravityBullet_AttractionRange);

            foreach (Collider hVictim in tHits)
            {
                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();


                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnPull(this.transform.position + hGOrb.offsetExplosion, -hGOrb.explosionForce);
                    gAffect.OnRelease();

                    if (hGOrb.isFloatExplosion)
                    {
                        gAffect.OnFloatingActivation(hGOrb.upwardsForceOnFloat, hGOrb.timeBeforeFloatActivate, hGOrb.isSlowedDownOnFloat, hGOrb.floatTime, hGOrb.zeroGIndependantTimeScale);

                        /*
                        if (hVictim.GetComponent<C_Enemy>() != null)
                        {
                            nbEnemiesHitByFloatExplo++;
                        }
                        */
                    }

                }
                
            }
            if (hGOrb.isExplosive)
            {
                 FxManager.Instance.PlayFx("VFX_GravityOrb", transform.position, Quaternion.identity);
                float newDuration = hGOrb.slowMoDuration;

                newDuration *= (nbEnemiesHitByFloatExplo == 0 ? 0 : 1 + (nbEnemiesHitByFloatExplo * .03f));

                //GameObject.FindObjectOfType<C_TimeScale>().AddSlowMo(hGOrb.slowMoPower, newDuration, hGOrb.timeBeforeFloatActivate, hGOrb.slowMoProbability);
            }
        }


        Destroy(this.gameObject);
    }

    IEnumerator OnHoldAttraction()
    {
        new WaitForSecondsRealtime(hGOrb.timeBeforeHold);

        fTimeHeld = 0;

        while (true)
        {
            //Debug.Log("Attraction");
            Collider[] tHits = Physics.OverlapSphere(this.transform.position, hGOrb.holdRange);

            foreach (Collider hVictim in tHits)
            {
                IGravityAffect gAffect = hVictim.GetComponent<IGravityAffect>();

                if (gAffect != null && hVictim.gameObject != parentIfSticky)
                {
                    gAffect.OnPull(this.transform.position, hGOrb.pullForce);
                    gAffect.OnHold();
                }
                    

            }

            fTimeHeld += hGOrb.waitingTimeBetweenAttractions;


            if (fTimeHeld >= hGOrb.lockTime)
            {
                StopHolding();
            }

            yield return new WaitForSecondsRealtime(hGOrb.waitingTimeBetweenAttractions);
        }
        

    }
    
}
