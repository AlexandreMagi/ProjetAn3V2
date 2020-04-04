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

    // Update is called once per frame
    void Update()
    {
        
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
                    break;

                case MetricsEventType.UsedGravity:
                    if (!currentMetrics.gravityUsed)
                    {
                        currentMetrics.gravityUsed = true;
                        TitlesManager.Instance.ChangeTitleState(12, false); //"Gravity is for the weaks"
                    }
                    break;

                case MetricsEventType.EnvironmentalKill:
                    TitlesManager.Instance.ChangeTitleState(14, true); //"Environmentalist"
                    currentMetrics.numberOfEnvKills++;

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

        if (currentMetrics.camerasHit == countOfCameras && currentMetrics.collectiblesHit == countOfCollectibles)
        {
            TitlesManager.Instance.ChangeTitleState(6, true); //"All bonuses"

            if (currentMetrics.totalDamageTaken == 0)
            {
                TitlesManager.Instance.ChangeTitleState(15, true); //"Gladiator"
            }
        }
    }

    public void SaveMetrics()
    {
        

        //Date du metrics
        string fileName = DateTime.Now + "";

        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(Metrics));

        //Sauvegarde try
        try
        {
            FileStream stream = new FileStream(Application.dataPath + "/Pangoblin/Metrics/"+fileName+".xml", FileMode.Create);
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
        EnvironmentalKill
    }
}

public class Metrics
{
    //Metrics
    public int camerasHit = 0;
    public float aim = 0;
    public int collectiblesHit = 0;
    public int numberOfEnvKills = 0;
    public float numberOfShots = 0;
    public float numberOfHits = 0;
    public int numberOfPerfectReloads = 0;
    public int numberOfReloads = 0;
    public float totalDamageTaken = 0;
    public bool playerHasBeenRaised = false;
    public bool shotgunUsed = false;
    public bool gravityUsed = false;
    public float timeOfGame = 0;

}
