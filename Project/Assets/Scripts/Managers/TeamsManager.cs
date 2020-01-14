using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamsManager : MonoBehaviour
{
    private int nbTeams = 4;
    public static TeamsManager Instance { get; private set; }
    private List<Transform>[] teams;

    public List<Transform> teamOne = new List<Transform>();
    public List<Transform> teamThree = new List<Transform>();

    private void Update()
    {
        teamOne = teams[1];
        teamThree = teams[3];
    }

    public void Awake()
    {
        Instance = this;

        teams = new List<Transform>[nbTeams];
        for (int i = 0; i < nbTeams; i++)
        {
            teams[i] = new List<Transform>();
        }
    }

    List<Transform> m_tempList = new List<Transform>();
    List<int> m_excList = new List<int>();

    //Gets all Transforms registered in all teams except the one sent
    public List<Transform> GetAllEnemiesFromTeam(int teamNumber, int[] exceptions = null)
    {
        m_tempList.Clear();
        m_excList.Clear();

        if (exceptions != null) m_excList.AddRange(exceptions);

        for (int i = 0; i < nbTeams; i++)
        {
            if (i == teamNumber || m_excList.Contains(i)) continue;

            m_tempList.AddRange(teams[i]);
        }

        return m_tempList;
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

    /*public void RemoveFromTeam(Transform obj, int teamNumber)
    {
        if (teamNumber < nbTeams || teamNumber >= 0)
        {
            teams[teamNumber].Remove(obj);
        }
    }*/

    public void RemoveFromTeam(Transform obj, int teamNumber)
    {
        if (teamNumber < nbTeams || teamNumber >= 0)
        {
            for (int i = 0; i < teams[teamNumber].Count; i++)
            {
                if (teams[teamNumber][i] == obj)
                {
                    teams[teamNumber].RemoveAt(i);
                }
            }
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
