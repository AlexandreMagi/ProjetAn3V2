using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairInstance
{
    public float size = 100;
    public Color color = Color.white;
    public Color outlineColor = Color.white;
    public float rotation = 0;
    public Vector2 offset;

    public DataCrossHair data;
    public float hitValue = 0;
    public float reculValue = 0;
    public float currentRotation = 0;

    public float pastChargeValue = 0;
    public bool inChargedFb = false;
    public float chargedFbValue = 0;

    public bool haveOrb = false;
    public float orbModifierPurcentage = 0;
    public float purcentageReductionNoBullet = 0;

    public RectTransform rect = null;
    public Image img = null;
    public Outline outline = null;

    public bool unlocked = false;
    public float unlockedPurcentage = 0;

    public CrosshairInstance (DataCrossHair _data, RectTransform _rect, Image _img, Outline _outline)
    {
        data = _data;
        size = data.baseSize;
        currentRotation = Random.Range(data.randomRotateAtStart.x, data.randomRotateAtStart.y);
        rect = _rect;
        img = _img;
        outline = _outline;
    }

    public void UpdateValues()
    {
        float dt = data.affectedByTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
        float time = data.affectedByTimeScale ? Time.time : Time.unscaledTime;
        float chargeValue = UpdateChargeValue();
        GetChargeFeedbackValue(chargeValue, dt);

        Vector2Int bulletAmount =Weapon.Instance != null? Weapon.Instance.GetBulletAmmount() : Vector2Int.one * 15;
        bool triggerNoBullet = (bulletAmount.x == 0 || (Main.Instance != null ? !Main.Instance.PlayerCanShoot : false));
        if (triggerNoBullet) purcentageReductionNoBullet += dt / data.noBulletAnimTime;


        else purcentageReductionNoBullet = dt;
        purcentageReductionNoBullet = Mathf.Clamp(purcentageReductionNoBullet, 0f, 1f);

        float sizeIdle                      = Mathf.Sin((time + data.offsetIdle) * (triggerNoBullet ? Mathf.Lerp(data.speedIdle, data.noBulletIdleSpeed, purcentageReductionNoBullet) : Mathf.Lerp(data.speedIdle, data.chargingSpeed, chargeValue)));
        float sizeIdleMultiplier            = (triggerNoBullet ? Mathf.Lerp(data.amplitudeIdle, data.noBulletAmplitudeIdle, purcentageReductionNoBullet) : Mathf.Lerp(data.amplitudeIdle, data.chargingAmplitudeIdle, chargeValue));
        float sizeAffectedByBulletNumber    = triggerNoBullet ? Mathf.Lerp(data.baseSize, data.noBulletSize, purcentageReductionNoBullet) : 0;
        float sizeAffectedByCurrentCharge   = triggerNoBullet ? 0 : Mathf.Lerp(data.baseSize, data.chargingSize, chargeValue);
        float sizeAffectedByAnim            = data.sizeAnim.Evaluate(chargedFbValue) * data.sizeMultiplier;
        float sizeAffectedByRecoil          = (reculValue / data.reculMax) * data.reculSizeMultiplier;
        float sizeAffectedByHit             = (hitValue / data.hitMax) * data.hitSizeMultiplier;

        size =  sizeIdle                        // Mathf sin pour l'idle
                * sizeIdleMultiplier            // Multiplier du mathf sin
                + sizeAffectedByBulletNumber    // Taille ajouté par le nombre de balle restant
                + sizeAffectedByCurrentCharge   // Taille ajouté par la charge d'arme actuelle
                + sizeAffectedByAnim            // Taille ajouté pra l'anim de charge
                + sizeAffectedByRecoil          // Taille ajouté par recul
                + sizeAffectedByHit;            // Taille ajouté par les Hit


        color = triggerNoBullet ? Color.Lerp(data.baseColor, data.noBulletColor, purcentageReductionNoBullet) : chargeValue == 1 ? data.chargedColor : Color.Lerp(Color.Lerp(data.baseColor, data.hitMaxColor, hitValue / data.hitMax), data.chargingColor, chargeValue);                                       // Changement de couleur
        outlineColor = triggerNoBullet ? Color.Lerp(data.outlineBaseColor, data.noBulletOutlineColor, purcentageReductionNoBullet) : chargeValue == 1 ? data.outlineChargedColor : Color.Lerp(Color.Lerp(data.outlineBaseColor, data.outlineHitMaxColor, hitValue / data.hitMax), data.outlineChargingColor, chargeValue);    // Changement de couleur

        currentRotation += data.rotateDir * dt * (chargeValue == 1 ? data.chargedRotateSpeed : Mathf.Lerp(data.rotateSpeed, data.chargingRotateSpeed, chargeValue));
        if (Mathf.Abs(currentRotation) > 360) currentRotation += 360 * Mathf.Sign(currentRotation);
        rotation = Mathf.Lerp(data.startRotation, data.chargingRotation, chargeValue) + currentRotation;

        offset = Vector2.Lerp(data.offset, data.chargingOffset, chargeValue);

        if (hitValue > 0) hitValue -= data.hitRecoverRate * dt;
        else hitValue = 0;
        if (reculValue > 0) reculValue -= data.reculRecoverRate * dt;
        else reculValue = 0;



        if (data.reactToOrb)
        {
            if (haveOrb)
                orbModifierPurcentage += dt / data.orbGrowTime;
            else 
                orbModifierPurcentage -= dt / data.orbShrinkTime;
            orbModifierPurcentage = Mathf.Clamp(orbModifierPurcentage, 0f, 1f);

            size += Mathf.Lerp(0, data.orbBonusSize, orbModifierPurcentage) + Mathf.Sin(time * (data.orbIdleSpeed * orbModifierPurcentage)) * data.orbIdleAmplitude;
            color = Color.Lerp(color, data.orbChargedColor, orbModifierPurcentage);
            outlineColor = Color.Lerp(outlineColor, data.outlineOrbChargedColor, orbModifierPurcentage);
        }

        if (!unlocked || unlockedPurcentage < 1)
        {
            if (unlocked) unlockedPurcentage += Time.unscaledDeltaTime / data.popTime;
            size = Mathf.Lerp(0, size, unlockedPurcentage);
            color = Color.Lerp(new Color (color.r, color.g, color.b, 0), color, unlockedPurcentage);
            outlineColor = Color.Lerp(new Color(outlineColor.r, outlineColor.g, outlineColor.b, 0), outlineColor, unlockedPurcentage);
            rotation = Mathf.Lerp(0, rotation, unlockedPurcentage);
            offset = Vector2.Lerp(Vector2.zero, offset, unlockedPurcentage);
        }

    }

    public void UpdateTransformsAndColors()
    {
        rect.sizeDelta = Vector2.one * size;
        rect.localRotation = Quaternion.identity;
        rect.Rotate(Vector3.back * rotation, Space.Self);
        img.color = color;
        outline.effectColor = outlineColor;
    }

    float UpdateChargeValue()
    {
        float currentChargevalue = (Weapon.Instance != null? Weapon.Instance.GetChargeValue() : MenuMain.Instance.GetChargeValue());
        float chargevalue = 0;
        if (currentChargevalue > data.transitionStartAt)
            chargevalue = (currentChargevalue - data.transitionStartAt) / (1 - data.transitionStartAt);
        return chargevalue;
    }

    public float GetChargeFeedbackValue(float newValue, float dt)
    {
        if (newValue == 1 && pastChargeValue != 1)
            inChargedFb = true;

        if (newValue != 1 && pastChargeValue == 1)
        {
            inChargedFb = false;
            chargedFbValue = 0;
        }

        if (inChargedFb) chargedFbValue += dt / data.timeToDoChargedFeedback;
        if (chargedFbValue > 1) chargedFbValue = 1;

        pastChargeValue = newValue;

        return chargedFbValue;
    }

    public void PlayerShot(float value)
    {
        reculValue += data.reculAddedPerShot * value;
        if (reculValue > data.reculMax) reculValue = data.reculMax;

        if (data.resetRotationOnRelease) currentRotation = 0;
    }

    public void PlayerHitSomething(float value)
    {
        hitValue += data.hitAddedPerShot * value;
        if (hitValue > data.hitMax) hitValue = data.hitMax;
    }

}
