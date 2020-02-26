using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiSwarmerAnim : MonoBehaviour
{

    private Animator Swanimator;
    private int SwanIdle = Animator.StringToHash("Idle");

    [SerializeField] private string[] allTrigger = new string[0];
    private int[] allTriggerHash = new int[0];

    private int OtherAnim = Animator.StringToHash("Intimidation");
    private float TimeLeft;

    void Start()
    {
        Swanimator = GetComponent<Animator>();
        TimeLeft = Random.Range(1.0f, 3.0f);

        allTriggerHash = new int[allTrigger.Length];
        for (int i = 0; i < allTrigger.Length; i++)
        {
            allTriggerHash[i] = Animator.StringToHash(allTrigger[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeLeft < Time.deltaTime)
        {
            Swanimator.SetTrigger(allTriggerHash[Random.Range(0, allTriggerHash.Length)]);
            TimeLeft += Random.Range(3f, 8f);
        }
        else
        {
            TimeLeft -= Time.deltaTime;
        }
    }
}
