using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sequences/DataBooleanSequence")]
public class DataBooleanSequence : ScriptableObject
{
    [SerializeField]
    public string boolName = "";

    [SerializeField]
    bool defaultState = false;

    public BooleanSequence OnInit()
    {
        return new BooleanSequence(boolName, defaultState);
    }
}

public class BooleanSequence
{
    public string boolName = "";
    public bool runtimeState = false;

    public BooleanSequence(string name, bool stateInit)
    {
        boolName = name;
        runtimeState = stateInit;
    }
}