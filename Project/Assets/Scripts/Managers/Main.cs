using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    private static Main _instance;

    [SerializeField]
    private GameObject orbPrefab;

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
            GameObject orb = Instantiate(orbPrefab);
            orb.GetComponent<GravityOrb>().OnSpawning(Input.mousePosition);
        }
    }
}
