using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINewWait : MonoBehaviour
{

    public static UINewWait Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }


    [SerializeField] GameObject[] objectToDisableOnWait = new GameObject[0];
    [SerializeField] GameObject[] objectToEnableOnWait = new GameObject[0];

    public void TriggerWait()
    {
        foreach (var obj in objectToDisableOnWait)
        {
            obj.SetActive(false);
        }
        foreach (var obj in objectToEnableOnWait)
        {
            obj.SetActive(true);
        }
    }

    public void RemoveWait()
    {
        foreach (var obj in objectToDisableOnWait)
        {
            obj.SetActive(true);
        }
        foreach (var obj in objectToEnableOnWait)
        {
            obj.SetActive(false);
        }
    }

}
