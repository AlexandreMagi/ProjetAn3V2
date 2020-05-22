using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureManager : MonoBehaviour
{
    
    [SerializeField] List<DeathBodyPart> allBodyParts = new List<DeathBodyPart>();

    [HideInInspector] public bool available = true;
    [SerializeField] float timeBeforeCanBeActivatedAgain = 10;
    [HideInInspector] public float timeRemainingBeforeActivation = 0;

    public void SpawnBodyParts(Vector3 pos)
    {
        available = false;
        timeRemainingBeforeActivation = timeBeforeCanBeActivatedAgain;
        DepopAll();
        for (int i = 0; i < allBodyParts.Count; i++)
        {
            allBodyParts[i].CheckIfMustPop(pos);
        }
    }

    public void DepopAll()
    {
        for (int i = 0; i < allBodyParts.Count; i++)
        {
            allBodyParts[i].Depop();
        }
    }

    void Update()
    {
        if (timeRemainingBeforeActivation > 0)
        {
            timeRemainingBeforeActivation -= Time.deltaTime;
            if (timeRemainingBeforeActivation < 0)
            {
                timeRemainingBeforeActivation = 0;
                available = true;
                gameObject.SetActive(false);
                DepopAll();
            }
        }
    }

}
