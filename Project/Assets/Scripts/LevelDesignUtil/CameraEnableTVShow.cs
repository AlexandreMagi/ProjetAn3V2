using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEnableTVShow : MonoBehaviour
{
    [SerializeField]
    Camera CameraTVToEnable = null;

    [SerializeField]
    float camDisabled = 10f;

    [SerializeField]
    float timeBeforeStart = 0f;
    float timeBeforeStartIncrem;

    bool collide = false;

    void OnTriggerEnter(Collider other)
    {
        timeBeforeStartIncrem = timeBeforeStart;
        collide = true;
    }

    private void Update()
    {
        if (collide)
        {
            if (timeBeforeStartIncrem <= 0)
                StartAction();
            else
                timeBeforeStartIncrem -= Time.deltaTime;
        }
    }

    void StartAction()
    {
        collide = false;
        CameraTVToEnable.GetComponent<Camera>().enabled = true;
        Invoke("DisableCam", camDisabled);
    }


    void DisableCam()
    {
        CameraTVToEnable.gameObject.SetActive(false);
        Destroy(this);
    }
}
