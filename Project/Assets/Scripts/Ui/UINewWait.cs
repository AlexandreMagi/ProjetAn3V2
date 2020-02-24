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
    [SerializeField] GameObject waitRoot = null;
    Animator waitAnmatr = null;

    private void Start()
    {
        waitAnmatr = waitRoot.GetComponent<Animator>();
    }

    public void TriggerWait()
    {
        foreach (var obj in objectToDisableOnWait)
        {
            obj.SetActive(false);
        }
        waitRoot.SetActive(true);
        waitAnmatr.SetTrigger("pop");
        
    }

    public void RemoveWait()
    {
        foreach (var obj in objectToDisableOnWait)
        {
            obj.SetActive(true);
        }
        waitRoot.SetActive(false);
        waitAnmatr.SetTrigger("depop");
    }

}
