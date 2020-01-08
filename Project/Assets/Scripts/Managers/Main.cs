using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    private static Main _instance;


    public static Main Instance{
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Weapon.Instance.GravityOrbInput();
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Weapon.Instance.InputHold();
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            Weapon.Instance.InputUp(Input.mousePosition);
        }
    }
}
