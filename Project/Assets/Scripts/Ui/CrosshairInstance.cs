using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public CrosshairInstance (DataCrossHair _data)
    {
        data = _data;
        size = data.baseSize;
        currentRotation = Random.Range(data.randomRotateAtStart.x, data.randomRotateAtStart.y);
    }

    public void UpdateValues()
    {
        float dt = data.affectedByTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
        float time = data.affectedByTimeScale ? Time.time : Time.unscaledTime;
        float chargeValue = UpdateChargeValue();
        GetChargeFeedbackValue(chargeValue, dt);

        size = Mathf.Sin((time + data.offsetIdle) * Mathf.Lerp(data.speedIdle, data.chargingSpeed, chargeValue)) * Mathf.Lerp(data.amplitudeIdle, data.chargingAmplitudeIdle, chargeValue)                      // Sin Idle
                + Mathf.Lerp(data.baseSize, data.chargingSize, chargeValue) + (data.sizeAnim.Evaluate(chargedFbValue) * data.sizeMultiplier)                                                                    // Base Size + FB charged size
                + (reculValue / data.reculMax) * data.reculSizeMultiplier                                                                                                                                       // Taille ajouté par recul
                + (hitValue / data.hitMax) * data.hitSizeMultiplier;                                                                                                                                            // Taille ajouté par les Hit

        color = chargeValue == 1 ? data.chargedColor : Color.Lerp(Color.Lerp(data.baseColor, data.hitMaxColor, hitValue / data.hitMax), data.chargingColor, chargeValue);                                       // Changement de couleur
        outlineColor = chargeValue == 1 ? data.outlineChargedColor : Color.Lerp(Color.Lerp(data.outlineBaseColor, data.outlineHitMaxColor, hitValue / data.hitMax), data.outlineChargingColor, chargeValue);    // Changement de couleur

        currentRotation += data.rotateDir * dt * (chargeValue == 1 ? data.chargedRotateSpeed : Mathf.Lerp(data.rotateSpeed, data.chargingRotateSpeed, chargeValue));
        if (Mathf.Abs(currentRotation) > 360) currentRotation += 360 * Mathf.Sign(currentRotation);
        rotation = Mathf.Lerp(data.startRotation, data.chargingRotation, chargeValue) + currentRotation;

        offset = Vector2.Lerp(data.offset, data.chargingOffset, chargeValue); /// new Vector2 (Screen.width, Screen.height) * 1000;

        if (hitValue > 0) hitValue -= data.hitRecoverRate * dt;
        else hitValue = 0;
        if (reculValue > 0) reculValue -= data.reculRecoverRate * dt;
        else reculValue = 0;

    }

    float UpdateChargeValue()
    {
        float currentChargevalue = Weapon.Instance.GetChargeValue();
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
