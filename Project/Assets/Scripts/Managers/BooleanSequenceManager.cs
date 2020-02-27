using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BooleanSequenceManager : MonoBehaviour
{
    [InlineEditor(InlineEditorModes.SmallPreview)]
    public static BooleanSequenceManager Instance { get; private set; }

    [SerializeField, ListDrawerSettings(NumberOfItemsPerPage = 10)]
    List<DataBooleanSequence> sequenceBooleansData = new List<DataBooleanSequence>();

    List<BooleanSequence> sequenceBooleans = new List<BooleanSequence>();

    public void Start()
    {
        Instance = this;

        foreach (DataBooleanSequence bSeq in sequenceBooleansData)
        {
            sequenceBooleans.Add(bSeq.OnInit());
        }
    }
    
    public void SetStateOfBoolSequence(string _name, bool _state)
    {
        foreach(BooleanSequence bSeq in sequenceBooleans)
        {
            if(bSeq.boolName == _name)
            {
                bSeq.runtimeState = _state;
                break;
            }
        }
    }

    public bool GetStateOfBoolSequence(string _name)
    {
        foreach (BooleanSequence bSeq in sequenceBooleans)
        {
            if (bSeq.boolName == _name)
            {
                return bSeq.runtimeState;
            }
        }

        return false;
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Sequences/DataBooleanSequence")]
public class DataBooleanSequence : ScriptableObject
{
    [SerializeField]
    string boolName = "";

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