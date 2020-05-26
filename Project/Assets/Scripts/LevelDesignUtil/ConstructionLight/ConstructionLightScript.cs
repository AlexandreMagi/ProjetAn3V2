using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionLightScript : MonoBehaviour
{
    [SerializeField] Light mySpotlight = null;
    [SerializeField] LightHandler mySpotlightHandler = null;
    float baseLightIntensity = 0;
    int remainingLightBulb = 2;
    int nbMaxLightBulb = 2;


    [SerializeField] Light bulbExplosionLight = null;
    [SerializeField] float bulbExplosionLightTime = .1f;

    void Start()
    {
        if (mySpotlight != null) baseLightIntensity = mySpotlight.intensity;
    }

    public void LightBulbDestroyed()
    {
        remainingLightBulb--;
        if (mySpotlight != null) mySpotlight.intensity = baseLightIntensity * ((float)remainingLightBulb / (float)nbMaxLightBulb);
        if (mySpotlightHandler != null)
        {
            mySpotlightHandler.onRandomTime /= 2;
            mySpotlightHandler.offRandomTime *= 2;
        }
        StartCoroutine(BulbExplosionLight());
    }

    IEnumerator BulbExplosionLight()
    {
        if (bulbExplosionLight != null) bulbExplosionLight.enabled = true;
        yield return new WaitForSeconds(bulbExplosionLightTime);
        if (bulbExplosionLight != null) bulbExplosionLight.enabled = false;
    }

}
