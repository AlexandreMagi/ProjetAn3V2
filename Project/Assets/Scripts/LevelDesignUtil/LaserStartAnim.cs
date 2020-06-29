using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserStartAnim : MonoBehaviour
{

    [SerializeField]
    float minTimeBeforeStart = 1f;

    [SerializeField]
    float maxTimeBeforeStart = 3f;

    Animator laserAnimator = null;

    private void Awake()
    {
        laserAnimator = GetComponent<Animator>();

        StartCoroutine(startAnimation());
    }

    IEnumerator startAnimation()
    {
        yield return new WaitForSeconds(Random.Range(minTimeBeforeStart, maxTimeBeforeStart));

        laserAnimator.SetTrigger("MakeAction");

        yield break;
    }
}
