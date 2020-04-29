using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [SerializeField]
    TextMeshPro display = null;

    // Start is called before the first frame update
    void Start()
    {
        mans = new List<Mannequin>(GetComponentsInChildren<Mannequin>());
        UpdateText();

    }


    public void ChildDied()
    {
        numberOfMannequinsDied++;
        UpdateText();

        if (numberOfMannequinsDied == mans.Count)
        {
            if (nextSequenceOnMansDead)
                TriggerUtil.TriggerSequence(timeBeforeStart);


            if (animOnMansDead)
                TriggerUtil.TriggerAnimators(timeBeforeStart, anims, false);
        }
    }

    void UpdateText()
    {
        if (display != null)
            display.text = numberOfMannequinsDied.ToString() + "/" + mans.Count.ToString();
    }

}
