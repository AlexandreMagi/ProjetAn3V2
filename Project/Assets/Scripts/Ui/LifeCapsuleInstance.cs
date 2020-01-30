using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeCapsuleInstance
{

    public float sizeMult = 1;
    public Vector2 size = Vector2.one;
    public float outlineSize = 5;
    public Color color = Color.red;

    public float sizeDesactivated = 1;

    DataLifeBarUi data;
    float timeRemainingAnimatedHit = 0;
    float timeRemainingAnimatedHola = 0;

    float timerBeforeAnimHit = 0;
    float timerBeforeAnimHola = 0;

    int index = 0;

    bool desactivated = false;

    public LifeCapsuleInstance (DataLifeBarUi _data, int i)
    {
        data = _data;
        color = data.lifeCapsuleColor;
        size = data.baseSize;
        outlineSize = data.outlineSize;
        index = i;
    }

    public void UpdateValues()
    {
        if (timerBeforeAnimHit < 0)
        {
            if (timeRemainingAnimatedHit > 0) timeRemainingAnimatedHit -= Time.unscaledDeltaTime;
            if (timeRemainingAnimatedHit < 0) timeRemainingAnimatedHit = 0;
        }
        else timerBeforeAnimHit -= Time.unscaledDeltaTime;
        if (timerBeforeAnimHola < 0)
        {
            if (timeRemainingAnimatedHola > 0) timeRemainingAnimatedHola -= Time.unscaledDeltaTime;
            if (timeRemainingAnimatedHola < 0) timeRemainingAnimatedHola = 0;
        }
        else timerBeforeAnimHola -= Time.unscaledDeltaTime;

        sizeDesactivated = Mathf.Lerp(sizeDesactivated, desactivated ? data.sizeDesac : 1, Time.unscaledDeltaTime * data.speedDesactivate);

        float sizeModifier = (sizeMult + data.holaEffectOnBullet.Evaluate(1 - timeRemainingAnimatedHola / data.holaFeedbackTime) * data.holaScaleMultiplier + data.takeDamageScaleAnim.Evaluate(1 - timeRemainingAnimatedHit / data.scaleAnimTime) * data.scaleAnimValue + Mathf.Sin(Time.unscaledTime * data.idleSpeed + data.idleDecal * index) * data.idleMagnitude) * sizeDesactivated;
        size = data.baseSize * sizeModifier;
        outlineSize = data.outlineSize * sizeModifier;

        color = Color.Lerp(desactivated ? data.lifeEmptyCapsuleColor : data.lifeCapsuleColor, data.hitedColor, data.takeDamageScaleAnim.Evaluate(1 - timeRemainingAnimatedHit / data.scaleAnimTime));
        

    }

    public void TakeDammage(float timerBeforeActivate)
    {
        timerBeforeAnimHit = timerBeforeActivate;
        timeRemainingAnimatedHit = data.scaleAnimTime;
    }

    public void Beat(float timerBeforeActivate)
    {
        timerBeforeAnimHola = timerBeforeActivate;
        timeRemainingAnimatedHola = data.holaFeedbackTime;
    }

    public void desactivate()
    {
        desactivated = true;
        sizeDesactivated = data.sizeWhenJustHited;
    }

    public void activate()
    {
        desactivated = false;
    }

}
