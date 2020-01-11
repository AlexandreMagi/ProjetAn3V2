using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmerPullHandler : MonoBehaviour
{
    private static SwarmerPullHandler _instance;
    public static SwarmerPullHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    List<GameObject> allSwarmers = new List<GameObject>();

    [SerializeField]
    GameObject swarmerPrefab = null;

    void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject current = Instantiate(swarmerPrefab);
            current.SetActive(false);
            allSwarmers.Add(current);
        }
    }

    public GameObject GetSwarmer(DataEntity entDataToGive)
    {
        foreach (var swarmer in allSwarmers)
        {
            if (swarmer.activeSelf == false)
            {
                swarmer.SetActive(true);
                ChangeMat(swarmer, entDataToGive as DataSwarmer);
                return swarmer;
            }
        }
        GameObject current = Instantiate(swarmerPrefab);
        allSwarmers.Add(current);
        ChangeMat(current, entDataToGive as DataSwarmer);
        return current;
    }

    void ChangeMat(GameObject swarmer, DataSwarmer entDataToGive)
    {
        swarmer.GetComponentInChildren<Renderer>().material = entDataToGive.mat;
    }

}
