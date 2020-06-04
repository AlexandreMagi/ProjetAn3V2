using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class TitlesManager : MonoBehaviour
{
    public static TitlesManager Instance { get; private set; }

    public TitlesDatabase dbTitles;

    public class Title
    {
        public string titleName;
        public string titleDesc;
        public bool isUnlocked;
        public int titleID;
        public int bonusScore;
        public string titleType;
        public bool usable;
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
            TextAsset mytxtData = (TextAsset)Resources.Load("titles");
            string txt = mytxtData.text;

            using (TextReader sr = new StringReader(txt))
            {
                dbTitles = serializer.Deserialize(sr) as TitlesDatabase;
            }

            //FileStream stream = new FileStream(Application.dataPath + "/Leaderboard/titles.xml", FileMode.Open);

            //Lecture
            

            //Fermeture du stream
            //stream.Close();
        }
        catch (System.Exception e)
        {
            //Si le fichier est pas trouvé en gros, en créer un nouveau
            Debug.LogWarning($"Aucune donnée de titres n'a été détectée.");
            Debug.LogWarning(e);
        }

        //Debug.Log(dbTitles.titlesRegistered.Count);
    }

    public void CalculateScores()
    {
        MetricsGestionnary mI = MetricsGestionnary.Instance;

        foreach(Title title in dbTitles.titlesRegistered)
        {
            if (title.usable)
            {
                switch (title.titleID)
                {
                    #region Code En Enum
                    //case 0:
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.Unkillable);
                    //    break;
                    //case 1:
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.Immaculate);
                    //    break;
                    //case 2:
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.WellProtected);
                    //    break;
                    //case 8: 
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.Inextremis);
                    //    break;
                    //case 9:
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.Chouchou);
                    //    break;
                    //case 11:
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.WhoNeedsAShotgun);
                    //    break;
                    //case 12: 
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.GravityIsWeak);
                    //    break;
                    //case 15: 
                    //        Main.Instance.AddEndGameBonus(title.isUnlocked? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, DataProgressSprite.SpriteNeeded.Gladiator);
                    //    break;
                    #endregion
                    case 0:
                    case 1:
                    case 2:
                    case 8:
                    case 9:
                    case 11:
                    case 12:
                    case 15:
                        //Debug.Log(title.titleType + " = " + title.isUnlocked);
                        Main.Instance.AddEndGameBonus(title.isUnlocked ? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, title.titleID);
                        break;
                    case 10:
                        //Debug.Log(title.titleType + " = " + title.isUnlocked);
                        if (title.isUnlocked)
                            Main.Instance.AddEndGameBonus(title.isUnlocked ? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.TechWizard);
                        break;
                    case 3:
                        //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(float.IsNaN(mI.GetMetrics().aim) ? 0 : Mathf.RoundToInt(mI.GetMetrics().aim), 60, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.Sniper, "%");
                        break;
                    case 4:
                        //if (title.isUnlocked)
                        //Main.Instance.AddEndGameBonus(Mathf.RoundToInt(mI.GetMetrics().timeOfGame), 10, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.Speedrunner, "ms");
                        break;
                    case 5:
                        //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().camerasHit, mI.countOfCameras, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.Unphotogenic, "");
                        break;
                    case 6:
                        //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().collectiblesHit, mI.countOfCollectibles, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.LivingArmor, "");
                        break;
                    case 7:
                        //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().collectiblesHit + mI.GetMetrics().camerasHit, mI.countOfCollectibles + mI.countOfCameras, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.AllBonus, "");
                        break;
                    case 13:
                        //if (title.isUnlocked)
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().totalDamageTaken, mI.dataTitles.damageTakenRequired, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.Unshakable);
                        break;
                    case 14:
                        Main.Instance.AddEndGameBonus(mI.GetMetrics().numberOfEnvKills, mI.dataTitles.numberOfEnviroKillsForTitle, title.titleType, title.bonusScore, title.titleName, title.titleDesc, (int)DataProgressSprite.SpriteNeeded.Environmentalist);

                        break;
                    case 17:
                        Main.Instance.AddEndGameBonus(title.isUnlocked ? 1 : 0, 1, title.titleType, title.bonusScore, title.titleName, title.titleDesc, title.titleID);
                        break;
                }

            }

        }

    }

    public void ChangeTitleState(string titleName, bool unlocked)
    {
        bool changeDoable = false;
        int index = 0;
        foreach(Title titleInTab in dbTitles.titlesRegistered)
        {
            
            if(titleName == titleInTab.titleName)
            {
                changeDoable = true;
                break;
            }
            index++;
        }

        if(changeDoable)
        {
            dbTitles.titlesRegistered[index].isUnlocked = unlocked;


            //Debug.Log(dbTitles.titlesRegistered[index].isUnlocked);
        }
        else
        {
            //Debug.Log(titleName + " title does not exist");
        }

    }

    public void ChangeTitleState(uint titleID, bool unlocked)
    {
        bool changeDoable = false;
        int index = 0;
        foreach (Title titleInTab in dbTitles.titlesRegistered)
        {
            if (titleID == titleInTab.titleID)
            {
                changeDoable = true;
                break;
            }

            index++;
        }

        if (titleID == 1) Debug.Log("Immaculate = " + unlocked);

        if (changeDoable)
        {
            dbTitles.titlesRegistered[index].isUnlocked = unlocked;

            //Debug.Log(dbTitles.titlesRegistered[index].isUnlocked);
        }
        else
        {
            //Debug.Log($"Title with ID {titleID} does not exist");
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


