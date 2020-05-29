using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class MetricsGestionnary : MonoBehaviour
{
    [SerializeField]
    public TitlesData dataTitles;

    public static MetricsGestionnary Instance { get; private set; }

    public int countOfCameras = 0;
    public int countOfCollectibles = 0;
    float timeAtLaunch;

    [SerializeField]
    bool recordMetrics = true;

    Metrics currentMetrics = new Metrics();

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        //Detection auto des caméras et des collectibles
        countOfCameras = FindObjectsOfType<FixedCameraScript>().Length;

        ShootTrigger[] tabOfTriggers;
        tabOfTriggers = FindObjectsOfType<ShootTrigger>();

        foreach (ShootTrigger trigger in tabOfTriggers)
        {
            if (trigger.isCollectible)
            {
                countOfCollectibles++;
            }
        }

        timeAtLaunch = Time.time;
    }

    public void EventMetrics(MetricsEventType eventType, float additionnalData = 0)
    {
        if (recordMetrics)
        {
            switch (eventType)
            {
                case MetricsEventType.DamageTaken:
                    TitlesManager.Instance.ChangeTitleState(1, false); //"Immaculate"
                    currentMetrics.totalDamageTaken += additionnalData;

                    if (currentMetrics.totalDamageTaken > dataTitles.damageTakenRequired) TitlesManager.Instance.ChangeTitleState(13, true); //Unshakable

                    break;

                case MetricsEventType.DamageTakenOnHealth:
                    TitlesManager.Instance.ChangeTitleState(2, false); //"Well protected"

                    break;

                case MetricsEventType.Shoot:
                    currentMetrics.numberOfShots++;
                    break;

                case MetricsEventType.ShootHit:
                    currentMetrics.numberOfHits++;
                    break;

                case MetricsEventType.Reload:
                    currentMetrics.numberOfReloads++;
                    break;

                case MetricsEventType.PerfectReload:
                    currentMetrics.numberOfPerfectReloads++;
                    break;

                case MetricsEventType.Death:
                    TitlesManager.Instance.ChangeTitleState(0, false); //"Unkillable"
                    break;

                case MetricsEventType.Resurrection:
                    currentMetrics.playerHasBeenRaised = true;
                    break;

                case MetricsEventType.CameraDestroyed:
                    currentMetrics.camerasHit++;
                    if (currentMetrics.camerasHit == countOfCameras) TitlesManager.Instance.ChangeTitleState(5, true); //"Not today, Big Brother"

                    break;

                case MetricsEventType.ArmorPadDestroyed:
                    currentMetrics.collectiblesHit++;
                    if (currentMetrics.collectiblesHit == countOfCollectibles) TitlesManager.Instance.ChangeTitleState(7, true); //"Living armor"

                    break;

                case MetricsEventType.RevivedByCrowd:
                    TitlesManager.Instance.ChangeTitleState(9, true); //"Chouchou"
                    currentMetrics.playerHasBeenRaised = true;

                    break;

                case MetricsEventType.UsedCheatCode:
                    TitlesManager.Instance.ChangeTitleState(10, true); //"Tech Wizard"
                    break;

                case MetricsEventType.UsedShotgun:
                    if (!currentMetrics.shotgunUsed)
                    {
                        currentMetrics.shotgunUsed = true;
                        TitlesManager.Instance.ChangeTitleState(11, false); //"Who needs a shotgun"
                    }
                    currentMetrics.shotgunUsedTimes++;
                    break;

                case MetricsEventType.UsedGravity:
                    if (!currentMetrics.gravityUsed)
                    {
                        currentMetrics.gravityUsed = true;
                        TitlesManager.Instance.ChangeTitleState(12, false); //"Gravity is for the weaks"
                    }
                    currentMetrics.orbUsedTimes++;
                    break;

                case MetricsEventType.EnvironmentalKill:
                    TitlesManager.Instance.ChangeTitleState(14, true); //"Environmentalist"
                    currentMetrics.numberOfEnvKills++;

                    break;

                case MetricsEventType.InExtremisKill:
                    currentMetrics.numberOfInExtremisSwarmerKills++;

                    break;

                case MetricsEventType.SwarmerKill:
                    currentMetrics.swarmerKills++;

                    break;

                case MetricsEventType.ShooterKill:
                    currentMetrics.shooterKills++;

                    break;

                case MetricsEventType.MissileKill:
                    currentMetrics.numberOfMissilesDestroyed++;

                    break;
            }
        }
        
    }

    public void EndMetrics()
    {
        currentMetrics.timeOfGame = Time.time - timeAtLaunch;
        currentMetrics.aim = currentMetrics.numberOfHits / currentMetrics.numberOfShots * 100;
        if (currentMetrics.aim > dataTitles.aimRequiredForTitle)
        {
            TitlesManager.Instance.ChangeTitleState(3, true); //Sniper
        }

        currentMetrics.aimReload = (currentMetrics.numberOfPerfectReloads != 0) ? currentMetrics.numberOfReloads / currentMetrics.numberOfPerfectReloads * 100 : 0;
        if(currentMetrics.aimReload > dataTitles.percentPerfectReloadForTitle)
        {
            TitlesManager.Instance.ChangeTitleState(16, true); //Perfect reloads
        }

        if (currentMetrics.camerasHit == countOfCameras && currentMetrics.collectiblesHit == countOfCollectibles)
        {
            TitlesManager.Instance.ChangeTitleState(6, true); //"All bonuses"

            if (currentMetrics.totalDamageTaken == 0)
            {
                TitlesManager.Instance.ChangeTitleState(15, true); //"Gladiator"
            }
        }


        UILeaderboard.Instance.AddMetricToDisplay("Cameras destroyed", currentMetrics.camerasHit.ToString() + "/" + countOfCameras.ToString(), "", true);
        UILeaderboard.Instance.AddMetricToDisplay("Armor pads destroyed", currentMetrics.collectiblesHit +"/"+ countOfCollectibles, "", true);
        UILeaderboard.Instance.AddMetricToDisplay("Precision", currentMetrics.aim > 0? Mathf.FloorToInt(currentMetrics.aim).ToString()+"%" : "None", "", true);
        UILeaderboard.Instance.AddMetricToDisplay("Damage Taken", Mathf.FloorToInt(currentMetrics.totalDamageTaken).ToString("N0"), "", true);
        UILeaderboard.Instance.AddMetricToDisplay("Environmental Kills", Mathf.FloorToInt(currentMetrics.numberOfEnvKills).ToString("N0"), "", true);
        UILeaderboard.Instance.AddMetricToDisplay("Reloads", Mathf.FloorToInt(currentMetrics.numberOfReloads).ToString("N0"), "", true);
        UILeaderboard.Instance.AddMetricToDisplay("Perfect Reloads", Mathf.FloorToInt(currentMetrics.numberOfPerfectReloads).ToString("N0"), "", true);
        UILeaderboard.Instance.AddMetricToDisplay("Player Revived", (currentMetrics.playerHasBeenRaised ? "Yes":"No"), "", true);

        TimeSpan t = TimeSpan.FromSeconds(currentMetrics.timeOfGame);
        string timeStringed = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        //string timeStringed = string.Format("{0:D2}m:{1:D2}s", t.Minutes, t.Seconds);
        UILeaderboard.Instance.AddMetricToDisplay("Time Elapsed", timeStringed, "", true);
        //UILeaderboard.Instance.AddMetricToDisplay("Health Damage Taken", Mathf.FloorToInt(currentMetrics.DamageTakenOnHealth).ToString("N0"), "", true);
        Debug.Log("AddTotalScoreGained");
    }

    public void SaveMetrics()
    {
        //Date du metrics
        string fileName = ((DateTime.Now + "").Split(' ')[0] + "_" + (DateTime.Now + "").Split(' ')[1]).Replace('/','_').Replace(':','-');

        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(Metrics));

        //Sauvegarde try
        try
        {
            FileStream stream = new FileStream(Application.persistentDataPath + "/Metrics/"+fileName+".xml", FileMode.Create);
            serializer.Serialize(stream, currentMetrics);

            //Fermeture du stream
            stream.Close();
        }
        catch (Exception e)
        {
            Debug.LogError($"Erreur lors de la sauvegarde des metrics ! Error : {e.Message}");
        }
    }

    public Metrics GetMetrics()
    {
        return currentMetrics;
    }

    public enum MetricsEventType
    {
        DamageTaken,
        DamageTakenOnHealth,
        Death,
        Shoot,
        ShootHit,
        Resurrection,
        Reload,
        PerfectReload,
        CameraDestroyed,
        ArmorPadDestroyed,
        RevivedByCrowd,
        UsedCheatCode,
        UsedShotgun,
        UsedGravity,
        EnvironmentalKill,
        InExtremisKill,
        SwarmerKill,
        ShooterKill,
        MissileKill
    }
}

public class Metrics
{
    //Metrics
    public int camerasHit = 0;
    public float aim = 0;
    public float aimReload = 0;
    public int collectiblesHit = 0;
    public int numberOfEnvKills = 0;
    public float numberOfShots = 0;
    public float numberOfHits = 0;
    public float numberOfMissilesDestroyed = 0;
    public float numberOfInExtremisSwarmerKills = 0;
    public int numberOfPerfectReloads = 0;
    public int numberOfReloads = 0;
    public float totalDamageTaken = 0;
    public bool playerHasBeenRaised = false;
    public bool shotgunUsed = false;
    public bool gravityUsed = false;
    public int orbUsedTimes = 0;
    public int shotgunUsedTimes = 0;
    public float timeOfGame = 0;
    public int swarmerKills = 0;
    public int shooterKills = 0;

}
