using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannequinManager : MonoBehaviour
{
    List<Mannequin> mans;

    int numberOfMannequinsDied = 0;

    [SerializeField]
    bool animOnMansDead = false;

    [SerializeField, ShowIf("animOnMansDead")]
    Animator[] anims = null;

    [SerializeField]
    bool nextSequenceOnMansDead = true;

    [SerializeField]
    float timeBeforeStart = 0;

    // Start is called before the first frame update
    void Start()
    {
        mans = new List<Mannequin>(GetComponentsInChildren<Mannequin>());
    }


    public void ChildDied()
    {
        numberOfMannequinsDied++;

        if(numberOfMannequinsDied == mans.Count)
        {
            if (nextSequenceOnMansDead)
                TriggerUtil.TriggerSequence(timeBeforeStart);


            if (animOnMansDead)
                TriggerUtil.TriggerAnimators(timeBeforeStart, anims, false);
        }
    }
}
