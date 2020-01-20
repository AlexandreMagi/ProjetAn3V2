using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour
{
    uint nbViewers = 0;

    float currentMultiplier = 1;

    void Awake()
    {
        Instance = this;
    }

    public static PublicManager Instance { get; private set; }

    public uint GetNbViewers()
    {
        return nbViewers;
    }
}
