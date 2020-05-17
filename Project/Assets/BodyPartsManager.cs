using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartsManager : MonoBehaviour
{
    Queue<DeathBodyPart> allBodyPartsInTheGame;

    [SerializeField]
    int maximumBodyPartsTolerence = 50;

    public static BodyPartsManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        allBodyPartsInTheGame = new Queue<DeathBodyPart>();

        StartCoroutine("RecheckBodyParts");
    }

    public void NotifyApparition(DeathBodyPart part)
    {
        if(maximumBodyPartsTolerence == allBodyPartsInTheGame.Count)
        {
            DeathBodyPart partToKill = allBodyPartsInTheGame.Dequeue();

            if(partToKill != null && partToKill.gameObject.activeSelf)
            {
                partToKill.gameObject.SetActive(false);
            }
        }

        allBodyPartsInTheGame.Enqueue(part);
    }

    IEnumerator RecheckBodyParts()
    {
        while (true)
        {
            //Nettoyage de la queue

            yield return new WaitForSecondsRealtime(10);

            if (allBodyPartsInTheGame != null)
            {
                Queue<DeathBodyPart> newQueue = new Queue<DeathBodyPart>();

                while(allBodyPartsInTheGame.Count > 0)
                {
                    DeathBodyPart part = allBodyPartsInTheGame.Dequeue();
                    if(part != null && part.gameObject.activeSelf)
                    {
                        newQueue.Enqueue(part);
                    }
                }

                allBodyPartsInTheGame = newQueue;
            }
        }

    }
}
