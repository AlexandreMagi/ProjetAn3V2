using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceBranch : ScriptableObject
{
    [SerializeField]
    List<DataSequence> sequencesInBranch;

    public void OnCreate()
    {
        sequencesInBranch = new List<DataSequence>();
    }

    public DataSequence GetDataSequenceAt(int index)
    {
        if (index >= 0 && index < sequencesInBranch.Count)
        {
            return sequencesInBranch[index];
        }

        Debug.LogError($"No data sequence in index {index}");
        return null;
    }

    public int GetNumberOfSequences()
    {
        return sequencesInBranch.Count;
    }

    public void AddSequence(DataSequence seq)
    {
        sequencesInBranch.Add(seq);
    }

    public List<DataSequence> GetAllSequences()
    {
        return sequencesInBranch;
    }
}
