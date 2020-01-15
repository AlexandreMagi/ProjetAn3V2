using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BooleanSequenceManager : MonoBehaviour
{
    public static BooleanSequenceManager Instance { get; private set; }

    [SerializeField, ListDrawerSettings(NumberOfItemsPerPage = 10)]
    List<DataBooleanSequence> sequenceBooleans = new List<DataBooleanSequence>();

    public void Start()
    {
        Instance = this;

        foreach (DataBooleanSequence bSeq in sequenceBooleans)
        {
            bSeq.OnInit();
        }
    }
    
    public void SetStateOfBoolSequence(string _name, bool _state)
    {
        Debug.Log($"Name : {_name} -- State asked {_state} ");
        foreach(DataBooleanSequence bSeq in sequenceBooleans)
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
        foreach (DataBooleanSequence bSeq in sequenceBooleans)
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
    [HorizontalGroup]
    public string boolName = "";

    [SerializeField]
    bool defaultState = false;


    public bool runtimeState = false;

    public void OnInit()
    {
        runtimeState = defaultState;
    }
}