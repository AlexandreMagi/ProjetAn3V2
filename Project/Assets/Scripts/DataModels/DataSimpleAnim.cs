using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataSimpleAnim", menuName = "ScriptableObjects/DataSimpleAnim")]
public class DataSimpleAnim : ScriptableObject
{
    public AnimationCurve Curve = AnimationCurve.Linear(0,0,1,1);
    public float MultipyValue = 1;
    public float Time = 1;

    public float ValueAt (float purcentage) { return Curve.Evaluate(purcentage) * MultipyValue; }

    public bool AddPurcentage(float purcentage, float dt, out float currPurcentage)
    {
        if (purcentage < 1)
        {
            purcentage += dt / Time;
            if (purcentage > 1)
            {
                purcentage = 1;
                currPurcentage = purcentage;
                return true;
            }
        }
        currPurcentage = purcentage;
        return false;
    }

}
