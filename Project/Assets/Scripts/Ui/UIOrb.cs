using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOrb : MonoBehaviour
{
    private static UIOrb _instance;
    public static UIOrb Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }

    [SerializeField] ScriptIdleASuprimerPostJPO[] orbs = new ScriptIdleASuprimerPostJPO[0];
    [SerializeField] AnimationCurve animWhenFull = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] float animMultiplier = 0.3f;
    [SerializeField] float animTime = 0.8f;
    float animPurcentage = 0;


    [SerializeField] GameObject orbObtainedMesh = null;
    [SerializeField] AnimationCurve animWhenObtained = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float animMultiplierObtained = 0.3f;
    [SerializeField] float animTimeObtained = 0.8f;
    float animObtained = 1;
    bool firstTime = true;


    [SerializeField] ScriptIdleASuprimerPostJPO orbContainer = null;
    [SerializeField] AnimationCurve animWhenOnCooldown = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float animMultiplierCooldown = 0.3f;
    [SerializeField] float animTimeCooldown = 0.8f;
    float animCooldown = 1;

    [SerializeField] UIParticuleSystem unlockedFx = null;

    [SerializeField] Image[] shaderimages = null;
    [SerializeField] Color[] shaderLockColor = null;
    [SerializeField] Color[] shaderUnlockColor = null;

    [SerializeField] UIParticuleSystem particleEffectOnCursor = null;
    [SerializeField] Canvas cvs = null;

    // Update is called once per frame
    void Update()
    {
        float currVal = Weapon.Instance.GetOrbValue();

        if (currVal > 1)
        {
            if (animPurcentage < 1)
                animPurcentage += Time.unscaledDeltaTime / animTime;
            if (animPurcentage > 1)
                animPurcentage = 1;
        }
        else
        {
            animPurcentage = 0;

        }
        
        if (animObtained < 1)
        {
            //orbObtainedMesh.transform.localScale = animWhenObtained.Evaluate(animObtained) * animMultiplierObtained * Vector3.one;
            animObtained += Time.unscaledDeltaTime / animTimeObtained;
        }
        if (animObtained > 1)
        {
            orbObtainedMesh.transform.localScale = Vector3.zero;
            animObtained = 1;
        }
        
        if (animCooldown < 1)
        {
            orbContainer.refScale = animWhenOnCooldown.Evaluate(animCooldown) * animMultiplierCooldown + 1;
            animCooldown += Time.unscaledDeltaTime / animTimeCooldown;
        }
        if (animCooldown > 1)
        {
            orbContainer.refScale = 1;
            animCooldown = 1;
        }
        
        foreach (var orb in orbs)
        {
            orb.refScale = currVal > 1 ? 1 + animWhenFull.Evaluate(animPurcentage) * animMultiplier : currVal;
            orb.scaleIdle = currVal > 1;
        }

        for (int i = 0; i < shaderimages.Length; i++)
        {
            shaderimages[i].material.SetColor("_Color", currVal < 1 ? shaderLockColor[i] : shaderUnlockColor[i]);
        }

        if (particleEffectOnCursor != null && cvs != null)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Main.Instance.GetCursorPos(), cvs.worldCamera, out pos);
            particleEffectOnCursor.transform.position = transform.TransformPoint(pos);
        }
    }

    public void OrbCooldownUp()
    {
        if (unlockedFx != null) unlockedFx.Play();
        if (particleEffectOnCursor != null) particleEffectOnCursor.Play();
    }

    public void ActivateOrb()
    {
        if (firstTime)
        {
            animObtained = 0;
            firstTime = false;
            if (unlockedFx != null)
            {
                unlockedFx.Play();
                particleEffectOnCursor.Play();
            }
        }
    }

    public void cantOrb()
    {
        if (animCooldown == 1)
        {
            animCooldown = 0;
            //CustomSoundManager.Instance.PlaySound(CameraHandler.Instance.renderingCam.gameObject, "NoAmmoEnergetic", false, 1, 0.2f);
            CustomSoundManager.Instance.PlaySound("NoAmmoEnergetic", "UI", null, 1.5f, false,1, 0.2f);
        }
    }

}
