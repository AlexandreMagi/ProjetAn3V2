using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamsManager : MonoBehaviour
{
    private int nbTeams = 4;
    public static TeamsManager Instance { get; private set; }
    private List<Transform>[] teams;


    public void Awake()
    {
        Instance = this;

        teams = new List<Transform>[nbTeams];
        for (int i = 0; i < nbTeams; i++)
        {
            teams[i] = new List<Transform>();
        }
    }

    //Gets all Transforms registered in all teams except the one sent
    public List<Transform> GetAllEnemiesFromTeam(int teamNumber, int[] exceptions = null)
    {
        List<Transform> tempList = new List<Transform>();
        List<int> excList = new List<int>();
        if (exceptions != null) excList.AddRange(exceptions);

        for (int i = 0; i < nbTeams; i++)
        {
            if (i == teamNumber || excList.Contains(i)) continue;

            tempList.AddRange(teams[i]);
        }

        return tempList;
    }

    public List<Transform> GetAllTeams()
    {
        return GetAllEnemiesFromTeam(-1);
    }

    public List<Transform> GetTeam(int teamNumber)
    {
        if (teamNumber >= nbTeams || teamNumber < 0) return new List<Transform>();

        return teams[teamNumber];
    }

    public void RegistertoTeam(Transform obj, int teamNumber)
    {
        if (teamNumber < nbTeams || teamNumber >= 0)
        {
            teams[teamNumber].Add(obj);
        }
    }

    public void RemoveFromTeam(Transform obj, int teamNumber)
    {
        if (teamNumber < nbTeams || teamNumber >= 0)
        {
            teams[teamNumber].Remove(obj);
        }
    }

    public void SwapTeam(Transform obj, int initialTeam, int newTeam)
    {
        if ((initialTeam < nbTeams || initialTeam >= 0) && (newTeam < nbTeams || newTeam >= 0))
        {
            RemoveFromTeam(obj, initialTeam);
            RegistertoTeam(obj, newTeam);
        }
    }
}
