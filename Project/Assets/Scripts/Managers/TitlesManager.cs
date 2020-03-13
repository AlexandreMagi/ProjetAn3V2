using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlesManager : MonoBehaviour
{
    public static TitlesManager Instance { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

}
