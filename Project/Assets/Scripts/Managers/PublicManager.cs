using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour
{
    [SerializeField]
    DataPublic publicData = null;

    IEntity enemyForVendetta = null;

    int nbViewers = 0;

    float timeLeftForMultiKill = 0;
    float multiKillCounter = 0;


    float currentMultiplier = 1;

    float hpMultiplier = 1;

    float randomBalancedUp = 0;
    float randomBalancedDown = 0;

    List<ActionType> stallBuffer;

    // Multiplicateur de score
    public const float scoreMultiplier = 183.492761f * 1.5f;

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
            timeLeftForMultiKill -= Time.unscaledDeltaTime;
            if(timeLeftForMultiKill <= 0)
            {
                timeLeftForMultiKill = 0;
                multiKillCounter = 0;
            } 
        }
        if (nbViewers < 0) nbViewers = 0;


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
                AddViewers(4, true, action, "Destruction environnementale", _position);
                break;
            case ActionType.RefuseBonus:
                //AddViewers(5, true, action, "Big balls", _position);
                AddViewers(5, true, action, "Arogance", _position);
                break;
            case ActionType.PerfectProjectile:
                //AddViewers(3, true, action, "So close ouuuh", _position);
                AddViewers(3, true, action, "SI près ouuuh", _position);
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
                    if (multiKillCounter >= 3)
                    {
                        //AddViewers(3, false, ActionType.Kill, "Mu-mu-multi kill", _position);
                        AddViewers(1, false, ActionType.Kill, "Multi-élimination", _position);
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
                if (cause != null)
                {
                    enemyForVendetta = cause;
                }
                break;
            case ActionType.Vendetta:
                if (cause == enemyForVendetta)
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
                AddViewers(2, true, action, "Vive le spectacle !", _position);
                break;
            case ActionType.DamageOnLifeBar:
                //LoseViewers(3, "So weak", _position);
                LoseViewers(3, "Si faible...", _position);
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
                LoseViewers(2, "Raté !", _position);
                break;
            case ActionType.DeathAndRespawn:
                //LoseViewers(10, "Somes don't like beggars", _position);
                LoseViewers(10, "Dernière chance", _position);
                break;
            case ActionType.BonusOnRespawn:
                AddRawViewers((int)bonus, false, action);
                break;
            case ActionType.Collectible:
                AddViewers(1, false, ActionType.Collectible, "Destruction", _position);
                break;
            case ActionType.DamageOnEnemy:
                //AddViewers(0.1f, false, ActionType.DamageOnEnemy, "Dégats", _position);
                break;
            case ActionType.DamageFixedCam:
                AddViewers(1, false, ActionType.DamageFixedCam, "Boom", _position);
                break;
            case ActionType.KillSwarmer:
                AddViewers(0.4f, false, ActionType.KillSwarmer, "Kill Swarmer", _position);
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.SwarmerKill);
                break;
            case ActionType.KillShooter:
                AddViewers(1, false, ActionType.KillShooter, "Kill Shooter", _position);
                MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.ShooterKill);
                break;
            case ActionType.Cheat:
                AddViewers(10, false, ActionType.KillShooter, "Kill Shooter", _position);
                break;
            default:
                break;
        }
    }

    private void AddViewers(float viewerLevel, bool isAffectedByBuffer, ActionType action, string textToDisplay, Vector3 pos)
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

        float _randomBalancedUp = Random.Range(0, publicData.randomViewerGrowth);
        int difViewer = Mathf.FloorToInt((publicData.baseViewerGrowth - randomBalancedUp + _randomBalancedUp) * viewerLevel * bufferMultiplier * hpMultiplier);
        randomBalancedUp = _randomBalancedUp;

        difViewer = Mathf.RoundToInt(difViewer * scoreMultiplier);

        if (difViewer != 0) 
        {
            if (pos != Vector3.zero)
            {
                UiScoreBonusDisplay.Instance.AddScoreBonus(" + " + difViewer.ToString("N0"), true, pos + Vector3.up * 0.5f, 1);
            }
            else
            {
                //UiScoreBonusDisplay.Instance.AddScoreBonus(textToDisplay + " : + " + difViewer, true);
                UiScoreBonusDisplay.Instance.AddScoreBonus(" + " + difViewer.ToString("N0"), true);
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

    public void LoseRawViewer (int number)
    {
        nbViewers -= number;
    }

    private void LoseViewers(int viewerLevel, string textToDisplay, Vector3 pos)
    {
        float _randomBalancedDown = Random.Range(0, publicData.randomViewerLoss);
        int difViewer = Mathf.FloorToInt((publicData.baseViewerLoss - randomBalancedDown + _randomBalancedDown) * viewerLevel);
        randomBalancedDown = _randomBalancedDown;

        difViewer = Mathf.RoundToInt(difViewer * scoreMultiplier);
        if (difViewer != 0)
        {
            if (pos != Vector3.zero)
            {
                //UiScoreBonusDisplay.Instance.AddScoreBonus(textToDisplay + " : - " + difViewer, false, pos, 1);
                UiScoreBonusDisplay.Instance.AddScoreBonus(" - " + difViewer.ToString("N0"), false, pos, 1);
            }
            else
            {
                //UiScoreBonusDisplay.Instance.AddScoreBonus(textToDisplay + " : - " + difViewer, false);
                UiScoreBonusDisplay.Instance.AddScoreBonus(" - " + difViewer.ToString("N0"), false);
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

    public void RemoveFromVendetta(IEntity enemy)
    {
        if(enemyForVendetta == enemy)
        {
            enemyForVendetta = null;
        }
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
        DamageOnArmor = 16,
        Collectible = 17,
        DamageOnEnemy = 18,
        DamageFixedCam = 19,
        KillSwarmer = 20,
        KillShooter = 21,
        Cheat = 22
    }
}
