using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInGameManager : MonoBehaviour
{
    private static UiInGameManager _instance;
    public static UiInGameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
