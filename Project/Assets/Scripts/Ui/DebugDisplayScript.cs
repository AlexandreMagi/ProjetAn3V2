using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugDisplayScript : MonoBehaviour
{

    public static DebugDisplayScript Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] GameObject debugText = null;

    public void SetDebugVisible(bool activated)
    {
        if (debugText != null)
            debugText.SetActive(activated);
    }
}
