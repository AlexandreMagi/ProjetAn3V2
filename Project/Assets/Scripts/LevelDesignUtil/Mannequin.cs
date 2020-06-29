using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mannequin : Entity<DataProp>, IGravityAffect, IBulletAffect
{
    [SerializeField]
    bool mustBeKilledInZeroG = false;


    bool isZeroG = false;
    [HideInInspector] public bool isAffectedByGravity = false;

    float floatTimeLeft = 0;

    bool willDie = false;

    [SerializeField] Renderer[] renderers;

    [SerializeField,ColorUsage(true, true)]
    Color colorBase = Color.black;
    [SerializeField] float contrastBase = 10;
    [SerializeField, ColorUsage(true, true)]
    Color colorWhenZeroG = Color.white;
    [SerializeField] float contrastZeroG = 50;

    Rigidbody rb = null;

    [SerializeField] Collider[] mannequinCollider = null;
    AudioSource sparkAudioSource = null;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        foreach (Renderer _renderer in renderers)
        {
            Material newMat = Instantiate(_renderer.materials[1]);

            _renderer.materials[1] = newMat;
            _renderer.materials[1].SetColor("_Reveallightcolor", colorBase);
            _renderer.materials[1].SetFloat("_Contrast", contrastBase);
        }
        sparkAudioSource = CustomSoundManager.Instance.PlaySound("SE_Mannequin_Sparks", "Effect", null, 1, true, 0.3f, 0.2f, 0);
        if (sparkAudioSource != null)
        {
            sparkAudioSource.spatialBlend = 1;
            sparkAudioSource.minDistance = 8;
            sparkAudioSource.transform.position = transform.position;
        }

    }

    public override void TakeDamage(float value)
    {
        if(((mustBeKilledInZeroG && isZeroG) || !mustBeKilledInZeroG ) && !willDie)
        {
            willDie = true;
            Die();
        }
    }

    public override void ForceKill()
    {
        if (((mustBeKilledInZeroG && isZeroG) || !mustBeKilledInZeroG) && !willDie)
        {

            willDie = true;
            Die();
        }
    }

    protected override void Die()
    {
        if (sparkAudioSource != null) sparkAudioSource.Stop();
        FxManager.Instance.PlayFx(entityData.fxPlayedOnDestroy, transform.position, Quaternion.identity);
        if (DeadBodyPartManager.Instance != null) DeadBodyPartManager.Instance.RequestPop(entityData.fractureType, transform.position, transform.up * .5f);
        MannequinManager parentManger = GetComponentInParent<MannequinManager>();
        if (mannequinCollider != null)
        {
            for (int i = 0; i < mannequinCollider.Length; i++)
            {
                if (mannequinCollider[i] != null) mannequinCollider[i].enabled = false;
            }
        }

        if (parentManger)
        {
            GetComponentInParent<MannequinManager>().ChildDied();





            base.Die();
        }
       
    }

    #region Gravity
    public void OnGravityDirectHit()
    {
        ReactGravity<DataProp>.DoFreeze(rb);
        isAffectedByGravity = true;
    }

    public void OnZeroGRelease()
    {
        foreach (Renderer _renderer in renderers)
        {
            Debug.Log(_renderer.materials[1]);
            _renderer.materials[1].SetColor("_Reveallightcolor", colorBase);
            _renderer.materials[1].SetFloat("_Contrast", contrastBase);
        }
        isZeroG = false;
    }

    public void OnHold()
    {
        //Nothing happens on hold
    }

    public void OnPull(Vector3 origin, float force, bool isReppelForce = false, Vector3? normalReppel = null)
    {
        ReactGravity<DataProp>.DoPull(rb, origin, force, isZeroG, isReppelForce, normalReppel);
        isAffectedByGravity = true;
    }

    public void OnRelease()
    {
        ReactGravity<DataProp>.DoUnfreeze(rb);

        /*foreach (Renderer _renderer in renderers)
        {
            _renderer.materials[0].SetColor("_Reveallightcolor", colorBase);
        }*/
    }

    public void OnZeroG()
    {
        ReactGravity<DataProp>.DoSpin(rb);
        isZeroG = true;
        foreach (Renderer _renderer in renderers)
        {
            _renderer.materials[1].SetColor("_Reveallightcolor", colorWhenZeroG);
            _renderer.materials[1].SetFloat("_Contrast", contrastZeroG);
        }

    }

    public void SetTimerToRelease(float timeSent) { Invoke("CompleteRelease", timeSent + 2.5f); }
    void CompleteRelease() { isAffectedByGravity = false; isZeroG = false; }


    public void OnFloatingActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float floatTime, bool bIndependantFromTimeScale)
    {
        //Die();
        
        //ReactGravity<DataProp>.DoPull(rb, Vector3.up.normalized + this.transform.position, fGForce, false);

        ReactGravity<DataProp>.DoFloat(rb, timeBeforeActivation, isSlowedDownOnFloat, floatTime, bIndependantFromTimeScale);

        floatTimeLeft = floatTime;

    }
    #endregion

    #region Bullets
    public void OnHit(DataWeaponMod mod, Vector3 position, float dammage, Ray shotRay)
    {
        if (mustBeKilledInZeroG && isZeroG)
        {
            rb.AddExplosionForce(250, position, 1);
            TakeDamage(mod.bullet.damage);
        }
           
    }

    public void OnHitShotGun(DataWeaponMod mod)
    {
    }

    public void OnHitSingleShot(DataWeaponMod mod)
    {
    }

    public void OnBulletClose()
    {
    }
    #endregion

    void Update()
    {
        if(floatTimeLeft > 0)
        {
            floatTimeLeft -= Time.deltaTime;

            if(floatTimeLeft <= 0)
            {
                floatTimeLeft = 0;
                isZeroG = false;
                foreach (Renderer _renderer in renderers)
                {
                    _renderer.materials[1].SetColor("_Reveallightcolor", colorBase);
                    _renderer.materials[1].SetFloat("_Contrast", contrastBase);
                }
            }
        }
        if (sparkAudioSource != null)
        {
            sparkAudioSource.transform.position = transform.position;
        }

        //Debug.Log($"Velo update : {rb.velocity}");
    }


}
