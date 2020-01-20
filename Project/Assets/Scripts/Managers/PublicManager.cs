using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour
{
    int nbViewers = 0;

    List<ActionType> stallBuffer;

    [SerializeField]
    int bufferSize = 8;

    int baseViewerGrowth = 50;
    int randomViewerGrowth = 8;
    float bufferStallAffect = .8f;

    float currentMultiplier = 1;

    void Awake()
    {
        Instance = this;
        stallBuffer = new List<ActionType>();
    }

    public static PublicManager Instance { get; private set; }

    public int GetNbViewers()
    {
        return nbViewers;
    }

    public void OnPlayerAction(ActionType action)
    {
        switch (action)
        {
            case ActionType.EnvironmentKill:
                AddViewers(4, true, action);
                AddToBuffer(action);
                break;
            case ActionType.RefuseBonus:
                break;
            case ActionType.PerfectProjectile:
                break;
            case ActionType.BackToSender:
                break;
            case ActionType.Kill:
                break;
            case ActionType.PerfectReload:
                break;
            case ActionType.Vendetta:
                break;
            case ActionType.SuperLowHp:
                break;
            case ActionType.LowHp:
                break;
            case ActionType.DamageOnLifeBar:
                break;
            case ActionType.Repeat:
                break;
            case ActionType.MissGravityOrb:
                break;
            case ActionType.MissShotGun:
                break;
            case ActionType.DeathAndRespawn:
                break;
            default:
                break;
        }
    }

    private void AddViewers(int viewerLevel, bool isAffectedByBuffer, ActionType action)
    {
        float bufferMultiplier = 1;
        if (isAffectedByBuffer)
        {
            foreach (ActionType actionInTab in stallBuffer)
            {
                if (actionInTab == action) bufferMultiplier *= bufferStallAffect;
            }
        }
       

        nbViewers += Mathf.FloorToInt((baseViewerGrowth + Random.Range(0, randomViewerGrowth)) * viewerLevel * bufferStallAffect);
    }

    private void AddToBuffer(ActionType action)
    {
        stallBuffer.Add(action);
        if (stallBuffer.Count > bufferSize) stallBuffer.RemoveAt(0);
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
        DamageOnLifeBar = 9,
        Repeat = 10,
        MissGravityOrb = 11,
        MissShotGun = 12,
        DeathAndRespawn = 13,
    }
}
