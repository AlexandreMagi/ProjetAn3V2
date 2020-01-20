using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour
{
    uint nbViewers = 0;

    List<ActionType> stallBuffer;

    float currentMultiplier = 1;

    void Awake()
    {
        Instance = this;
        stallBuffer = new List<ActionType>();
    }

    public static PublicManager Instance { get; private set; }

    public uint GetNbViewers()
    {
        return nbViewers;
    }

    public void OnEpicAction(string actionName)
    {
        switch (actionName)
        {

        }
    }

    public void OnLameAction(string actionName)
    {

    }

    public enum ActionType
    {
        EnvironmentKill = 0,
        RefuseBonus = 1,
        PerfectProjectile = 2,
        BackToSender = 3,
        Kill = 4,
        PerfectReload = 5,
        Vendetta = 6,
        SuperLowHp = 7,
        LowHp = 8,
    }
}
