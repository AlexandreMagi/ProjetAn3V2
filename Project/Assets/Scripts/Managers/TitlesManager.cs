using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class TitlesManager : MonoBehaviour
{
    public static TitlesManager Instance { get; private set; }

    TitlesDatabase dbTitles;

    public struct Title
    {
        public string titleName;
        public string titleDesc;
        public bool isUnlocked;
        public int titleID;
        public int bonusScore;
        public string titleType;
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        dbTitles = new TitlesDatabase();
        //SaveTitles();
        LoadTitles();
    }
    
    void SaveTitles()
    {
        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(TitlesDatabase));

        //Sauvegarde try
        try
        {
            FileStream stream = new FileStream(Application.dataPath + "/Leaderboard/titles.xml", FileMode.Create);
            serializer.Serialize(stream, dbTitles);

            //Fermeture du stream
            stream.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erreur lors de la sauvegarde ! Error : {e.Message}");
        }
    }

    void LoadTitles()
    {
        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(TitlesDatabase));

        try
        {
            FileStream stream = new FileStream(Application.dataPath + "/Leaderboard/titles.xml", FileMode.Open);

            //Lecture
            dbTitles = serializer.Deserialize(stream) as TitlesDatabase;

            //Fermeture du stream
            stream.Close();
        }
        catch (System.Exception e)
        {
            //Si le fichier est pas trouvé en gros, en créer un nouveau
            Debug.LogWarning($"Aucune donnée de titres n'a été détectée.");
            Debug.LogWarning(e);
        }

        Debug.Log(dbTitles.titlesRegistered.Count);
    }

    public void CalculateScores()
    {
        MetricsGestionnary mI = MetricsGestionnary.Instance;

        foreach(Title title in dbTitles.titlesRegistered)
        {
            switch (title.titleID)
            {
                case 0: case 1: case 2: case 8: case 9: case 11: case 12: case 14: case 15:
                    //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc);
                    break;
                case 10:
                    Debug.Log(title.titleType + " = " + title.isUnlocked);
                    if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(title.isUnlocked ? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc);
                    break;
                case 3:
                    //if (title.isUnlocked)
                    Main.Instance.AddEndGameBonus(float.IsNaN(mI.GetMetrics().aim) ? 0 : Mathf.RoundToInt(mI.GetMetrics().aim), 60, title.titleType, title.bonusScore, title.titleName, title.titleDesc, null, "%");
                    break;
                case 4:
                    //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(Mathf.RoundToInt(mI.GetMetrics().timeOfGame), 10, title.titleType, title.bonusScore, title.titleName, title.titleDesc, null, "ms");
                    break;
                case 5:
                    //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().camerasHit, mI.countOfCameras, title.titleType, title.bonusScore, title.titleName, title.titleDesc, null, "");
                    break;
                case 7:
                    //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().collectiblesHit, mI.countOfCollectibles, title.titleType, title.bonusScore, title.titleName, title.titleDesc, null, "");
                    break;
                case 6:
                    //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().collectiblesHit + mI.GetMetrics().camerasHit, mI.countOfCollectibles + mI.countOfCameras, title.titleType, title.bonusScore, title.titleName, title.titleDesc, null, "");
                    break;
                case 13:
                    //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().totalDamageTaken, mI.dataTitles.damageTakenRequired, title.titleType, title.bonusScore, title.titleName, title.titleDesc);
                    break;
            }
        }
    }

    public void ChangeTitleState(string titleName, bool unlocked)
    {
        Title titleToChange;
        bool changeDoable = false;
        foreach(Title titleInTab in dbTitles.titlesRegistered)
        {
            if(titleName == titleInTab.titleName)
            {
                titleToChange = titleInTab;
                changeDoable = true;
                break;
            }
        }

        if(changeDoable)
        {
            titleToChange.isUnlocked = unlocked;
        }
    }

    public void ChangeTitleState(uint titleID, bool unlocked)
    {
        Title titleToChange;
        bool changeDoable = false;
        foreach (Title titleInTab in dbTitles.titlesRegistered)
        {
            if (titleID == titleInTab.titleID)
            {
                titleToChange = titleInTab;
                changeDoable = true;
                break;
            }
        }

        if (changeDoable)
        {
            titleToChange.isUnlocked = unlocked;
        }
    }

    public bool GetTitleState(string titleName)
    {
        foreach (Title titleInTab in dbTitles.titlesRegistered)
        {
            if (titleName == titleInTab.titleName)
            {
                return titleInTab.isUnlocked;
            }
        }

        return false;
    }

    public string GetTitleDesc(string titleName)
    {
        foreach (Title titleInTab in dbTitles.titlesRegistered)
        {
            if (titleName == titleInTab.titleName)
            {
                return titleInTab.titleDesc;
            }
        }

        return "";
    }

    public List<Title> GetAllTitles()
    {
        return dbTitles.titlesRegistered;
    }
}

public class TitlesDatabase
{
    public List<TitlesManager.Title> titlesRegistered;

    public TitlesDatabase()
    {
        titlesRegistered = new List<TitlesManager.Title>();

        /*
        TitlesManager.Title init = new TitlesManager.Title
        {
            titleName = "Truc",
            titleDesc = "Desc",
            isUnlocked = false
        };

        titlesRegistered.Add(init);
        */
    }
}


