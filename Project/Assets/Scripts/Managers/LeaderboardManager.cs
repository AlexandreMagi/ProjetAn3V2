using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    //On assumera en permanence que le score est trié en ordre croissant
    LeaderboardDatabase scoreData = null;

    int maxKeptScores = 10;

    public void Start()
    {
        Instance = this;

        LoadScores();
    }

    public void SaveScores()
    {
        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(LeaderboardDatabase));
        FileStream stream = new FileStream(Application.persistentDataPath + "/Saves/Leaderboard/scores.xml", FileMode.Create);

        //Sauvegarde
        serializer.Serialize(stream, scoreData);

        //Fermeture du stream
        stream.Close();
    }

    void LoadScores()
    {
        //Création du sérializer et du stream de fichier.
        XmlSerializer serializer = new XmlSerializer(typeof(LeaderboardDatabase));
        FileStream stream = new FileStream(Application.persistentDataPath + "/Saves/Leaderboard/scores.xml", FileMode.OpenOrCreate);

        //Lecture
        scoreData = serializer.Deserialize(stream) as LeaderboardDatabase;

        //Fermeture du stream
        stream.Close();
    }

    public bool SubmitScoreToLeaderboard(string name, int score)
    {
        bool scoreValuable = false;
        int indexOfNewScore = -1;

        for(int i=0; i<scoreData.data.Length; i++)
        {
            LeaderboardData currentData = scoreData.data[i];

            if (score > currentData.score)
            {
                indexOfNewScore = i;
                scoreValuable = true;
                break;
            }
        }

        if (scoreValuable)
        {
            LeaderboardData dataNewScore = new LeaderboardData(name, score);

            LeaderboardData[] newData = new LeaderboardData[maxKeptScores];

            for(int i = 0; i<scoreData.data.Length || i<maxKeptScores; i++)
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

    public LeaderboardData(string p_name, int p_score)
    {
        name = p_name;
        score = p_score;
    }
}

public class LeaderboardDatabase
{
    public LeaderboardData[] data;

    public void SetData(LeaderboardData[] datas)
    {
        data = datas;
    }
}
