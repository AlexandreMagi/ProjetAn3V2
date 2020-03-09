﻿using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;
using System;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    //On assumera en permanence que le score est trié en ordre croissant
    LeaderboardDatabase scoreData = null;

    [SerializeField]
    public int maxKeptScores = 10;

    public void Start()
    {
        Instance = this;

        //RefabricXMLDataDefault();

        LoadScores();
    }

    public LeaderboardData GetHighestScore()
    {
        return scoreData.data[0];
    }

    public List<LeaderboardData> GetLeaderboard()
    {
        return scoreData.data;
    }

    public void SaveScores()
    {
        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(LeaderboardDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/Saves/Leaderboard/scores.xml", FileMode.Create);

        //Sauvegarde try
        try {
            serializer.Serialize(stream, scoreData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Erreur lors de la sauvegarde ! Error : {e.Message}");
        }
       

        //Fermeture du stream
        stream.Close();
    }

    public void RefabricXMLDataDefault()
    {
        scoreData = new LeaderboardDatabase();
        LeaderboardData temp = new LeaderboardData("AAA", 50);
        scoreData.data.Add(temp);

        SaveScores();
    }

    void LoadScores()
    {
        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(LeaderboardDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/Saves/Leaderboard/scores.xml", FileMode.Open);

        try
        {
            //Lecture
            scoreData = serializer.Deserialize(stream) as LeaderboardDatabase;
        }
        catch(Exception e)
        {
            //Si le fichier est pas trouvé en gros, en créer un nouveau
            RefabricXMLDataDefault();
        }
        

        //Fermeture du stream
        stream.Close();
    }

    public bool SubmitScoreToLeaderboard(string name, int score)
    {
        bool scoreValuable = false;
        int indexOfNewScore = -1;

        for(int i=0; i<scoreData.data.Count; i++)
        {
            LeaderboardData currentData = scoreData.data[i];

            if (score > currentData.score)
            {
                indexOfNewScore = i;
                scoreValuable = true;
                break;
            }
        }

        //Si les high scores sont pas pleins, on met à la fin
        if(!scoreValuable && scoreData.data.Count < maxKeptScores)
        {
            scoreValuable = true;
            indexOfNewScore = scoreData.data.Count;
        }

        //Ajout du score
        if (scoreValuable)
        {
            LeaderboardData dataNewScore = new LeaderboardData(name, score);

            List<LeaderboardData> newData = new List<LeaderboardData>(maxKeptScores);

            for(int i = 0; i<scoreData.data.Count || i<maxKeptScores; i++)
            {
                bool hasDecaled = false;
                if (i == indexOfNewScore)
                {
                    hasDecaled = true;
                    newData[i] = dataNewScore;
                }
                else
                {
                    newData[i] = scoreData.data[i - (hasDecaled ? 1 : 0)];
                }
            }

            scoreData.SetData(newData);

            SaveScores();
        }

        return scoreValuable;
    }

}

public class LeaderboardData
{
    public string name;
    public int score;

    public LeaderboardData()
    {
        name = "DEF";
        score = 0;
    }

    public LeaderboardData(string p_name, int p_score)
    {
        name = p_name;
        score = p_score;
    }
}

public class LeaderboardDatabase
{
    public List<LeaderboardData> data;

    public LeaderboardDatabase()
    {
        data = new List<LeaderboardData>();
    }

    public void SetData(List<LeaderboardData> datas)
    {
        data = datas;
    }
}
