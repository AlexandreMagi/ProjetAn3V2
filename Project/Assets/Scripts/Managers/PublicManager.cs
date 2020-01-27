using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour
{
    [SerializeField]
    DataPublic publicData;

    IEntity enemyForVendetta = null;

    int nbViewers = 0;

    float timeLeftForMultiKill = 0;
    float multiKillCounter = 0;


    float currentMultiplier = 1;

    float hpMultiplier = 1;

    List<ActionType> stallBuffer;

    void Awake()
    {
        Instance = this;
        stallBuffer = new List<ActionType>();
        nbViewers = publicData.startViewers;
    }

    public void Update()
    {
        if(timeLeftForMultiKill > 0)
        {
            timeLeftForMultiKill -= Time.deltaTime;
            if(timeLeftForMultiKill <= 0)
            {
                timeLeftForMultiKill = 0;
            } 
        }

        
    }

    public static PublicManager Instance { get; private set; }

    public int GetNbViewers()
    {
        return nbViewers;
    }

    public void OnPlayerAction(ActionType action, IEntity cause = null)
    {
        switch (action)
        {
            case ActionType.EnvironmentKill:
                AddViewers(4, true, action);
                break;
            case ActionType.RefuseBonus:
                AddViewers(5, true, action);
                break;
            case ActionType.PerfectProjectile:
                AddViewers(3, true, action);
                break;
            case ActionType.BackToSender:
                AddViewers(3, true, action);
                break;
            case ActionType.Kill:
                //Un peu spécial
                multiKillCounter++;
                if (timeLeftForMultiKill > 0)
                {
                    if(multiKillCounter > 5)
                    {
                        AddViewers(3, false, ActionType.Kill);
                    }
                    else
                    {
                        AddViewers(2, false, ActionType.Kill);
                    }
                   
                }
                timeLeftForMultiKill = 0.8f; 
                break;
            case ActionType.PerfectReload:
                AddViewers(2, true, action);
                break;
            case ActionType.VendettaPrepare:
                //Special
                if(cause != null)
                {
                    enemyForVendetta = cause;
                }
                break;
            case ActionType.Vendetta:
                if(cause == enemyForVendetta)
                {
                    AddViewers(3, true, action);
                }
                break;
            case ActionType.SuperLowHp:
                hpMultiplier = 1.2f;
                break;
            case ActionType.LowHp:
                hpMultiplier = 1.5f;
                break;
            case ActionType.DamageOnLifeBar:
                LoseViewers(3);
                break;
            case ActionType.Repeat:
                LoseViewers(3);
                break;
            case ActionType.MissGravityOrb:
                LoseViewers(7);
                break;
            case ActionType.MissShotGun:
                LoseViewers(2);
                break;
            case ActionType.DeathAndRespawn:
                LoseViewers(10);
                break;
            default:
                break;
        }
    }

    private void AddViewers(int viewerLevel, bool isAffectedByBuffer, ActionType action)
    {
        float bufferMultiplier = 1;
        int capCount = 0;
        if (isAffectedByBuffer)
        {
            foreach (ActionType actionInTab in stallBuffer)
            {
                if (actionInTab == action)
                {
                    bufferMultiplier *= publicData.bufferStallAffect;
                    capCount++;

                    if(capCount >= publicData.antiFarmCap)
                    {
                        bufferMultiplier = 0;
                        break;
                    }
                }
            }
        }
        if(bufferMultiplier > 0 && isAffectedByBuffer)
        {
            AddToBuffer(action);
        }

        nbViewers += Mathf.FloorToInt((publicData.baseViewerGrowth + Random.Range(0, publicData.randomViewerGrowth)) * viewerLevel * bufferMultiplier * hpMultiplier);

        RecalculateMultiplier();
    }

    private void LoseViewers(int viewerLevel)
    {
        nbViewers -= Mathf.FloorToInt((publicData.baseViewerLoss + Random.Range(0, publicData.randomViewerLoss)) * viewerLevel);

        if (nbViewers < 0) nbViewers = 0;
        //Kill player ?

        RecalculateMultiplier();
    }

    private void AddToBuffer(ActionType action)
    {
        stallBuffer.Add(action);
        if (stallBuffer.Count > publicData.bufferSize) stallBuffer.RemoveAt(0);
    }

    public void RecalculateMultiplier()
    {
        currentMultiplier = nbViewers / publicData.startViewers;
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
        VendettaPrepare = 14
    }
}
