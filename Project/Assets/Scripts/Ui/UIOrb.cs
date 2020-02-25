using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOrb : MonoBehaviour
{

    [SerializeField] ScriptIdleASuprimerPostJPO[] orbs = new ScriptIdleASuprimerPostJPO[0];

    // Update is called once per frame
    void Update()
    {
        float currVal = Weapon.Instance.GetOrbValue();
        foreach (var orb in orbs)
        {
            orb.refScale = currVal > 1 ? 1: currVal;
            orb.scaleIdle = currVal > 1;
        }
    }
}
