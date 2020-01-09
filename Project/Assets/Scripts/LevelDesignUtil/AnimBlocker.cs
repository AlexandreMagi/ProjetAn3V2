using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBlocker : MonoBehaviour
{
    public bool isBlocked = true;

    bool played = false;
    [SerializeField]
    bool startsNextSequenceOnUnlock = false;

    [SerializeField]
    List<IGravityAffect> blockers;

    void Start()
    {
        blockers = new List<IGravityAffect>();

        isBlocked = CheckBlock();
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer != 12)
        {
            IGravityAffect affect = other.GetComponent<IGravityAffect>();

            if (!blockers.Contains(affect))
            {
                blockers.Add(affect);
            }

            isBlocked = CheckBlock();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != 12)
        {
            IGravityAffect affect = other.GetComponent<IGravityAffect>();

            if (blockers.Contains(affect))
            {
                blockers.Remove(affect);
            }

            isBlocked = CheckBlock();

            if (startsNextSequenceOnUnlock && !played && !isBlocked)
            {
                SequenceHandler.Instance.NextSequence();
                played = true;
            }

        }

    }

    bool CheckBlock()
    {
        return (blockers.Count != 0);
    }
}
