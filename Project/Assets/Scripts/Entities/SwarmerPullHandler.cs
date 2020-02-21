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
        for (int i = 0; i < 20; i++)
        {
            GameObject current = Instantiate(swarmerPrefab, Vector3.one * 66, Quaternion.identity);
            current.SetActive(false);
            allSwarmers.Add(current);
        }
    }

    public GameObject GetSwarmer(DataEntity _entDataToGive)
    {

        DataSwarmer entDataToGive = _entDataToGive as DataSwarmer;
        foreach (var swarmer in allSwarmers)
        {
            if (swarmer.activeSelf == false)
            {
                swarmer.SetActive(true);
                swarmer.GetComponent<Swarmer>().ResetSwarmer(entDataToGive);
                return swarmer;
            }
        }
        // ---
        GameObject current = Instantiate(swarmerPrefab);
        allSwarmers.Add(current);
        current.GetComponent<Swarmer>().ResetSwarmer(entDataToGive);
        return current;
    }


}
