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
                Ray rRayBullet = mainCam.GetComponent<Camera>().ScreenPointToRay(mousePosition);

                //Shoot raycast
                RaycastHit hit;
                if (Physics.Raycast(rRayBullet, out hit, Mathf.Infinity, weapon.layerMaskHit))
                {
                    Debug.Log("CheckDeRecepteursBullet");
                }
            }
        }
        bulletRemaining -= weaponMod.bulletCost;
        if (bulletRemaining < 0) bulletRemaining = 0;
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
