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
        if (waitAnmatr != null && waitRoot.activeSelf)
            waitAnmatr.SetTrigger("pop");
        //Weapon.Instance.rotateLocked = true;
        
    }

    public void RemoveWait()
    {
        if (waitAnmatr != null && waitRoot.activeSelf)
            waitAnmatr.SetTrigger("depop");
        Invoke("RemoveWaitTrue", 0.39f);
        //Weapon.Instance.rotateLocked = false;
    }


    public void RemoveWaitTrue()
    {
        foreach (var obj in objectToDisableOnWait)
        {
            obj.SetActive(true);
        }
        waitRoot.SetActive(false);
    }

}
