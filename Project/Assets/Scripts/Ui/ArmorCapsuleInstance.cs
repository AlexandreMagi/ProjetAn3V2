using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorCapsuleInstance
{

    public float sizeMult = 1;
    public Vector2 size = Vector2.one;
    public float outlineSize = 5;
    public Color color = Color.red;
    public float stockArmor;
    public float currentArmor;

    DataArmorBarUi data;
    float timeRemainingAnimatedHit = 0;


    bool desactivated = false;

    public ArmorCapsuleInstance (DataArmorBarUi _data)
    {
        data = _data;
        color = data.baseColor;
        size = data.baseSize;
        outlineSize = data.outlineSize;
    }

    public void UpdateValues()
    {
        if (timeRemainingAnimatedHit > 0) timeRemainingAnimatedHit -= Time.unscaledDeltaTime;
        if (timeRemainingAnimatedHit < 0) timeRemainingAnimatedHit = 0;

        //Debug.Log(data.takeDamageScaleAnim.Evaluate(1 - timeRemainingAnimatedHit / data.scaleAnimTime) * data.scaleAnimValue);

        if (desactivated)
        {
            sizeMult = Mathf.Lerp(sizeMult, data.deadSize, Time.unscaledDeltaTime * data.speedDieAnim);
            color = Color.Lerp(color, data.deadColor, Time.unscaledDeltaTime * data.speedDieAnim);
            size = data.baseSize * sizeMult;
        }
        else
        {
            float sizeModifier = sizeMult + data.takeDamageScaleAnim.Evaluate(1 - timeRemainingAnimatedHit / data.scaleAnimTime) * data.scaleAnimValue + Mathf.Sin(Time.unscaledTime * data.idleSpeed) * data.idleMagnitude;
            size = data.baseSize * sizeModifier;
            outlineSize = data.outlineSize * sizeModifier * (currentArmor / stockArmor);

            color = Color.Lerp(Color.Lerp(data.lowLifeColor, data.baseColor, currentArmor / stockArmor), data.hitedColor, data.takeDamageScaleAnim.Evaluate(1 - timeRemainingAnimatedHit / data.scaleAnimTime));
        }
    }

    public void TakeDammage(float valuePurcentage)
    {
        timeRemainingAnimatedHit = data.scaleAnimTime;
        //Debug.Log("Armor Bar take damage");
    }

    public void desactivate()
    {
        if (!desactivated)
        {
            desactivated = true;
            color = data.hitedColor;
        }
    }

}
