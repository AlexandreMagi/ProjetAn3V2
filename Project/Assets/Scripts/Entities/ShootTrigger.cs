using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

public class ShootTrigger : Entity<DataEntity>, IBulletAffect
{
    ShootTriggerManager parentManager = null;

    [SerializeField]
    public bool isCollectible = false;

    bool isTriggered = false;

    [SerializeField]
    float armorGiven = 0;

    //[SerializeField]
    //bool keepsCombo = true;

    [SerializeField]
    string[] soundPlayed = null;
    [SerializeField]
    float[] soundVolume = null;
    [SerializeField]
    float[] soundDelay = null;

    [SerializeField]
    Light lightToDisable = null;

    [SerializeField]
    bool useFracturedProp = false;

    [SerializeField, ShowIf("useFracturedProp")]
    GameObject fracturedProp = null;

    [SerializeField, ShowIf("useFracturedProp")]
    GameObject pivotFracturedExplosion = null;

    [SerializeField, ShowIf("useFracturedProp")]
    float fracturedForceOnDie = 100;


    Collider thisCollider = null;

    [SerializeField]
    bool useMeshRenderer = true;

    [SerializeField, ShowIf("useMeshRenderer")]
    Renderer[] mshrenderer = null;

    [SerializeField]
    bool useParticles = false;

    [SerializeField, ShowIf("useParticles")]
    ParticleSystem[] particles = null;

    CollectiblesSpritesAutoChange col;

    float currentHp = 0;

    protected override void Start()
    {
        parentManager = this.transform.GetComponentInParent<ShootTriggerManager>();

        thisCollider = GetComponent<Collider>();

        col = GetComponent<CollectiblesSpritesAutoChange>();

        currentHp = entityData.startHealth;
    }
    void InstantiateExplosion()
    {
        if (fracturedProp != null)
        {
            GameObject fract;
            fract = Instantiate(fracturedProp, pivotFracturedExplosion.transform);
            fract.transform.parent = null;

            Rigidbody[] rb = fract.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rbs in rb)
            {
                rbs.AddExplosionForce(fracturedForceOnDie * 10, rbs.transform.position, 10);
            }
        }
    }


    //Stimulus reactions
    #region StimulusBullet
    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray rayShot)
    {
        currentHp -= mod.bullet.damage;

        if (!isTriggered && currentHp <= 0)
        {
            //MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);

            isTriggered = true;
            //if (keepsCombo) C_ComboManager.Instance.MaintainCombo();

            if (parentManager != null)
                parentManager.OnEventSent();

            if (useMeshRenderer)
            {
                for (int i = 0; i < mshrenderer.Length; i++)
                {
                    mshrenderer[i].enabled = false;
                }
            }

            if (useParticles)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].gameObject.SetActive(false);
                }
            }

            if (lightToDisable != null)
                lightToDisable.gameObject.SetActive(false);

            thisCollider.enabled = false;

            if (isCollectible)
                col.HideMesh();

            if (useFracturedProp)
                InstantiateExplosion();

            if (gameObject.transform.tag == "EnvironnementTrigger")
            {
                FxManager.Instance.PlayFx("VFX_EnvironnementTrigger", this.transform.position, Quaternion.identity);
            }
            else
            {
                FxManager.Instance.PlayFx("VFX_CollectiblesShoot", this.transform.position, Quaternion.identity);
                //GetComponentInChildren<ParticleSystem>().Stop();
                //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, soundPlayed, false, soundVolume);
                for (int i = 0; i < soundPlayed.Length; i++)
                {
                    StartCoroutine(PlaySound(soundPlayed[i], soundDelay[i], soundVolume[i]));
                }
                //CustomSoundManager.Instance.PlaySound(soundPlayed, "Effect", soundVolume);
            }


            if (GetComponent<DeleteAfterJPOKillsParticles>() != null)
                GetComponent<DeleteAfterJPOKillsParticles>().StopParticles();

            if (armorGiven != 0)
                Player.Instance.GainArmor(armorGiven);

            if (isCollectible)
            {
                PublicManager.Instance.OnPlayerAction(PublicManager.ActionType.Collectible, transform.position);
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ArmorPadDestroyed);
            }


            Destroy(this);
        }
    }
    
    IEnumerator PlaySound(string _soundName, float _soundDelay, float _soundVolume)
    {
        //if (_soundDelay > 0) yield return new WaitForSeconds(_soundDelay);
        if (CustomSoundManager.Instance != null)
            CustomSoundManager.Instance.PlaySound(_soundName, "Effect", _soundVolume);
        yield break;
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
        Weapon.Instance.OnShotGunHitTarget();
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
        if (!Weapon.Instance.CheckIfModIsMinigun(mod)) 
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShootHit);
    }

    public void OnBulletClose()
    {
       
    }

    public void OnCursorClose()
    {
        
    }

    #endregion
}
