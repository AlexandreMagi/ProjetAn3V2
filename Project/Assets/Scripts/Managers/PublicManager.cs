using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour
{
    int nbViewers = 0;

    List<ActionType> stallBuffer;

    [SerializeField]
    int bufferSize = 8;

    [SerializeField]
    int baseViewerGrowth = 50;

    [SerializeField]
    int baseViewerLoss = 40;

    [SerializeField]
    int randomViewerLoss = 10;

    [SerializeField]
    int randomViewerGrowth = 8;

    [SerializeField]
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
                AddViewers(5, true, action);
                AddToBuffer(action);
                break;
            case ActionType.PerfectProjectile:
                AddViewers(3, true, action);
                AddToBuffer(action);
                break;
            case ActionType.BackToSender:
                AddViewers(3, true, action);
                AddToBuffer(action);
                break;
            case ActionType.Kill:
                //Un peu spécial
                break;
            case ActionType.PerfectReload:
                AddViewers(2, true, action);
                AddToBuffer(action);
                break;
            case ActionType.Vendetta:
                //Special
                break;
            case ActionType.SuperLowHp:
                AddViewers(4, false, action);
                break;
            case ActionType.LowHp:
                AddViewers(2, false, action);
                break;
            case ActionType.DamageOnLifeBar:
                LoseViewers(3);
                break;
            case ActionType.Repeat:
                LoseViewers(3);
                break;
            case ActionType.MissGravityOrb:
                LoseViewers(5);
                break;
            case ActionType.MissShotGun:
                LoseViewers(2);
                break;
            case ActionType.DeathAndRespawn:
                LoseViewers(6);
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

    private void LoseViewers(int viewerLevel)
    {
        nbViewers -= Mathf.FloorToInt((baseViewerLoss + Random.Range(0, randomViewerLoss)) * viewerLevel);

        if (nbViewers < 0) nbViewers = 0;
        //Kill player ?
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
