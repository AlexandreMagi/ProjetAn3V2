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

    private float timerSafeFx = 0.2f;
    private float currentTimer = 0;

    [SerializeField]
    GameObject player = null;

    [SerializeField]
    ParticleSystem pr;

    bool canPlay = true;

    /*[SerializeField]
    GameObject GravityOrb = null;
    [SerializeField]
    GameObject WiiMoteSprite = null;*/

    // Update is called once per frame
    void Update()
    {
        if ((transform.position.magnitude - player.transform.position.magnitude <= 10) && canPlay)
        {
            pr.Play();
            GetComponent<GravityOrb>().enabled = true;
            canPlay = false;
        }

        bPlayerCanDammage = Vector3.Distance(Player.Instance.transform.position, transform.position) < distanceAllowedToPlayer;

        if (DammageDone < DammageBeforeExplosion)
        {
            fCurrentScale = Mathf.Lerp(fCurrentScale, 1 + DammageDone * fScaleBoostBeforeExplosion / DammageBeforeExplosion, Time.deltaTime * 5);
            transform.localScale = Vector3.one * fCurrentScale;
        }
        else if (!bItemDestroyed)
        {
            bPlayerCanDammage = false;
            //GameObject.FindObjectOfType<C_Fx>().OrbGatherableExplosionFinal(transform.position + Vector3.up * 0.9542458f * fCurrentScale);
            FxManager.Instance.PlayFx("VFX_OrbGatherExplosion", transform.position /*+ Vector3.up * 0.9542458f * fCurrentScale*/,transform.rotation);
            FxManager.Instance.PlayFx("VFX_DistortionBoom", transform.position, transform.rotation);
            GetComponent<GravityOrb>().StopHolding();
            Invoke("OrbPreDestroyed", 2.6f);
            Invoke("OrbDestroyed", 0.5f);
            Invoke("GoToTuto", timeBetweenDeathAndNextSequence);
            bItemDestroyed = true;
            //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "GravityOrbOvercharge_Boosted", false, 1f);
        }
        if (bItemDestroyed && !bItemDestroyedCompletly)
        {
            GetComponentInChildren<ParticleSystem>().Stop();
            CameraHandler.Instance.AddShake(10 * Time.deltaTime);
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
        CameraHandler.Instance.AddShake(shakeForce, shakeTime);
        bItemDestroyedCompletly = true;
        GetComponent<SCRIPTRECOLTEORBEASUPPRIMERQUANDNOUVEAULD>().enabled = false;
        // GravityOrb.GetComponent<C_GravityOrb>().StopHolding();
        //GameObject.FindObjectOfType<C_Fx>().GatherOrb(transform.position + Vector3.up * 0.9542458f * fCurrentScale);
        //CustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "EquipOrb_Boosted", false, 1f);
    }

    void GoToTuto()
    {
        //GameObject.FindObjectOfType<C_Main>().AllowPlayerOrb();
        //bInTuto = true;
        //WiiMoteSprite.SetActive(true);
        //GameObject.FindObjectOfType<C_GravOrbReady>().PlayFeedback();
        //GameObject.FindObjectOfType<MainFuncTest>().bActivation = true;
        //GameObject.FindObjectOfType<MainFuncTest>().ChangeText("RIGHT CLICK TO ACTIVATE GRAVITY ORB");
        SequenceHandler.Instance.NextSequence();
    }

    public void TutoFinished()
    {
        //WiiMoteSprite.SetActive(false);
    }

    public void PlayerShootOnObjet(float Dmg)
    {
        if (bPlayerCanDammage && !bItemDestroyed)
        {
            currentTimer = timerSafeFx;
            if (currentTimer == 0) FxManager.Instance.PlayFx("VFX_DistortionBoom", transform.position, transform.rotation);
            DammageDone += Dmg / 35;
            for (int i = Mathf.CeilToInt(DammageDoneSaved); i < DammageDone; i++)
            {
                //GameObject.FindObjectOfType<C_Fx>().OrbGatherableExplosion(transform.position + Vector3.up * 0.9542458f * fCurrentScale);
                CameraHandler.Instance.AddShake(i * 0.3f,0.1f);
                //ustomSoundManager.Instance.PlaySound(Camera.main.gameObject, "ImpactOrbeSequence_Boosted", false, 1f);
            }
            DammageDoneSaved = DammageDone;
        }
    }

    public void OnHit(DataWeaponMod mod, Vector3 position)
    {
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
        PlayerShootOnObjet(mod.bullet.damage);
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        PlayerShootOnObjet(mod.bullet.damage);
    }

    public void OnBulletClose()
    {
    }


    /*private void OnTriggerEnter(Collider other)
    {
        bPlayerCanDammage = true;
    }*/
}