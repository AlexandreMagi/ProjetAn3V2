using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCRIPTRECOLTEORBEASUPPRIMERQUANDNOUVEAULD : MonoBehaviour, IBulletAffect
{
    bool bPlayerCanDammage = true;
    [SerializeField] float fCurrentScale = 1;
    [SerializeField] float fScaleBoostBeforeExplosion = .5f;

    float DammageDone = 0;
    float DammageDoneSaved = 0;
    [SerializeField] float DammageBeforeExplosion = 8;

    bool bItemDestroyed = false;
    bool bItemDestroyedCompletly = false;

    [HideInInspector]
    public bool bInTuto = false;

    [SerializeField]
    private float distanceAllowedToPlayer = 5;
    [SerializeField]
    private float timeBetweenDeathAndNextSequence = 7f;
    [SerializeField]
    private float shakeTime = 1;
    [SerializeField]
    private float shakeForce = 30;
    [SerializeField]
    private float safeShotgunValue = 0.4f;
    float timerSafeShotgun = 0;

    private float timerSafeFx = 0.2f;
    private float currentTimer = 0;

    [SerializeField]
    GameObject player = null;

    [SerializeField]
    ParticleSystem pr;

    bool pouletCoco = true;

    bool canPlay = true;

    float multiplierBoom = 1f;

    void Update()
    {
        timerSafeShotgun -= Time.unscaledDeltaTime;

        if ((transform.position.magnitude - player.transform.position.magnitude <= 10) && canPlay)
        {
            pr.Play();
            GetComponent<GravityOrb>().enabled = true;
            canPlay = false;
        }

        if (Player.Instance == null)
        {
            Destroy(this);
            bPlayerCanDammage = false;
        }
        else bPlayerCanDammage = Vector3.Distance(Player.Instance.transform.position, transform.position) < distanceAllowedToPlayer;


        if (DammageDone < DammageBeforeExplosion)
        {
            //fCurrentScale = Mathf.Lerp(fCurrentScale, 1 + DammageDone * fScaleBoostBeforeExplosion / DammageBeforeExplosion, Time.deltaTime * 5);
            //transform.localScale = Vector3.one * fCurrentScale;
        }
        else if (!bItemDestroyed)
        {
            bPlayerCanDammage = false;
            GetComponent<GravityOrb>().StopHolding();
            Invoke("OrbPreDestroyed", 2.6f);
            Invoke("OrbDestroyed", 0.1f);
            Invoke("GoToTuto", timeBetweenDeathAndNextSequence);
            bItemDestroyed = true;
        }
        if (bItemDestroyed && !bItemDestroyedCompletly && pouletCoco)
        {
            GetComponent<SphereCollider>().enabled = false;

            GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);

            CameraHandler.Instance.AddShake(10 * Time.deltaTime);

            pouletCoco = false;
        }

        if (currentTimer < Time.deltaTime) currentTimer = 0;
        else currentTimer -= Time.deltaTime;

    }

    void OrbPreDestroyed()
    {
        transform.localScale = Vector3.zero;
        CameraHandler.Instance.RemoveShake();
    }

    void OrbDestroyed()
    {
        FxManager.Instance.PlayFx("VFX_OrbGatherExplosion", transform.position,transform.rotation);
        CameraHandler.Instance.AddShake(shakeForce, 0.5f);
        Invoke("SecondShake", 0.9f);
    }

    void SecondShake()
    {

        CameraHandler.Instance.AddShake(shakeForce, shakeTime);

        bItemDestroyedCompletly = true;
        GetComponent<SCRIPTRECOLTEORBEASUPPRIMERQUANDNOUVEAULD>().enabled = false;
    }

    void GoToTuto()
    {
        SequenceHandler.Instance.NextSequence();
    }

    public void PlayerShootOnObjet(float Dmg, bool playFx)
    {
        if (bPlayerCanDammage && !bItemDestroyed)
        {
            currentTimer = timerSafeFx;
            if (currentTimer != 0 && playFx)
            {
                FxManager.Instance.PlayFx("VFX_DistortionBoom", transform.position, transform.rotation, multiplierBoom * 2.5f);
            }

            DammageDone += Dmg / 35;

            for (int i = Mathf.CeilToInt(DammageDoneSaved); i < DammageDone; i++)
            {
                CameraHandler.Instance.AddShake(i * 1.5f, 0.1f);
            }

            multiplierBoom += 1.5f;

            DammageDoneSaved = DammageDone;
        }
    }

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
        bool playFx = false;
        if (timerSafeShotgun < 0)
        {
            playFx = true;
            timerSafeShotgun = safeShotgunValue;
        }
        PlayerShootOnObjet(mod.bullet.damage, playFx);
        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        PlayerShootOnObjet(mod.bullet.damage, true);
    }

    public void OnBulletClose()
    {
    }
}