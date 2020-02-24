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

    public int GetInitialViewers()
    {
        return this.publicData.startViewers;
    }

    public int GetGrowthValue()
    {
        return this.publicData.baseViewerGrowth;
    }

    public void OnPlayerAction(ActionType action, Vector3 _position, IEntity cause = null, float bonus = 0)
    {
        switch (action)
        {
            case ActionType.EnvironmentKill:
                //AddViewers(4, true, action, "Environment Kill", _position);
                AddViewers(4, true, action, "Élimination environnemental", _position);
                break;
            case ActionType.RefuseBonus:
                //AddViewers(5, true, action, "Big balls", _position);
                AddViewers(5, true, action, "Arrogance", _position);
                break;
            case ActionType.PerfectProjectile:
                //AddViewers(3, true, action, "So close ouuuh", _position);
                AddViewers(3, true, action, "Si près ouuuh", _position);
                break;
            case ActionType.BackToSender:
                //AddViewers(3, true, action, "Back to mama !", _position);
                AddViewers(3, true, action, "Retour à l'envoyeur !", _position);
                break;
            case ActionType.Kill:
                //Un peu spécial
                multiKillCounter++;
                if (timeLeftForMultiKill > 0)
                {
                    if(multiKillCounter > 5)
                    {
                        //AddViewers(3, false, ActionType.Kill, "Mu-mu-multi kill", _position);
                        AddViewers(3, false, ActionType.Kill, "Mu-mu-multi éliminations", _position);
                    }
                    else
                    {
                        //AddViewers(2, false, ActionType.Kill, "Combo kill", _position);
                        AddViewers(2, false, ActionType.Kill, "Combo !", _position);
                    }
                   
                }
                timeLeftForMultiKill = 0.8f; 
                break;
            case ActionType.PerfectReload:
                //AddViewers(2, true, action, "Perfect Reload", _position);
                AddViewers(2, true, action, "Rechargement parfait", _position);
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
                    //AddViewers(3, true, action, "Revenge", _position);
                    AddViewers(3, true, action, "Revanche", _position);
                }
                break;
            case ActionType.SuperLowHp:
                hpMultiplier = 1.2f;
                break;
            case ActionType.LowHp:
                hpMultiplier = 1.5f;
                break;
            case ActionType.DamageOnArmor:
                //AddViewers(2, true, action, "Damage", _position);
                AddViewers(2, true, action, "Aïe", _position);
                break;
            case ActionType.DamageOnLifeBar:
                //LoseViewers(3, "So weak", _position);
                LoseViewers(3, "Si faible", _position);
                break;
            case ActionType.Repeat:
                //LoseViewers(3, "Stop doing that !", _position);
                LoseViewers(3, "Répétition", _position);
                break;
            case ActionType.MissGravityOrb:
                //LoseViewers(7, "How can you miss this ?!", _position);
                LoseViewers(7, "Inratable, et pourtant...", _position);
                break;
            case ActionType.MissShotGun:
                //LoseViewers(2, "You killed the wall", _position);
                LoseViewers(2, "Tir raté", _position);
                break;
            case ActionType.DeathAndRespawn:
                //LoseViewers(10, "Somes don't like beggars", _position);
                LoseViewers(10, "Dernière chance", _position);
                break;
            case ActionType.BonusOnRespawn:
                AddRawViewers((int)bonus, false, action);
                break;
            default:
                break;
        }
    }

    private void AddViewers(int viewerLevel, bool isAffectedByBuffer, ActionType action, string textToDisplay, Vector3 pos)
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

        int difViewer = Mathf.FloorToInt((publicData.baseViewerGrowth + Random.Range(0, publicData.randomViewerGrowth)) * viewerLevel * bufferMultiplier * hpMultiplier);

        if (difViewer != 0) 
        {
            if (pos != Vector3.zero)
            {
                UiScoreBonusDisplay.Instance.AddScoreBonus(textToDisplay + " : + " + difViewer, true, pos, 1);
            }
            else
            {
                UiScoreBonusDisplay.Instance.AddScoreBonus(textToDisplay + " : + " + difViewer, true);
            }
        }

        nbViewers += difViewer;

        if(nbViewers <= 0)
        {
            nbViewers = 1;
        }

        RecalculateMultiplier();
    }

    private void AddRawViewers(int number, bool isAffectedByBuffer, ActionType action)
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

                    if (capCount >= publicData.antiFarmCap)
                    {
                        bufferMultiplier = 0;
                        break;
                    }
                }
            }
        }
        if (bufferMultiplier > 0 && isAffectedByBuffer)
        {
            AddToBuffer(action);
        }

        nbViewers += number * publicData.baseViewerGrowth;

        if (nbViewers <= 0)
        {
            nbViewers = 1;
        }

        RecalculateMultiplier();
    }

    private void LoseViewers(int viewerLevel, string textToDisplay, Vector3 pos)
    {
        int difViewer = Mathf.FloorToInt((publicData.baseViewerLoss + Random.Range(0, publicData.randomViewerLoss)) * viewerLevel);
        if (difViewer != 0)
        {
            if (pos != Vector3.zero)
            {
                UiScoreBonusDisplay.Instance.AddScoreBonus(textToDisplay + " : - " + difViewer, false, pos, 1);
            }
            else
            {
                UiScoreBonusDisplay.Instance.AddScoreBonus(textToDisplay + " : - " + difViewer, false);
            }
        }
        nbViewers -= difViewer;

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
        VendettaPrepare = 14,
        BonusOnRespawn = 15,
        DamageOnArmor = 16
    }
}
