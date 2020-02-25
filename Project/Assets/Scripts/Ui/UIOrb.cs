using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOrb : MonoBehaviour
{

    [SerializeField] ScriptIdleASuprimerPostJPO[] orbs = new ScriptIdleASuprimerPostJPO[0];
    [SerializeField] AnimationCurve animWhenFull = AnimationCurve.Linear(0,0,1,1);
    [SerializeField] float animMultiplier = 0.3f;
    [SerializeField] float animTime = 0.8f;
    float animPurcentage = 0;

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

        foreach (var orb in orbs)
        {
            orb.refScale = currVal > 1 ? 1 + animWhenFull.Evaluate(animPurcentage) * animMultiplier : currVal;
            orb.scaleIdle = currVal > 1;
        }
    }
}
