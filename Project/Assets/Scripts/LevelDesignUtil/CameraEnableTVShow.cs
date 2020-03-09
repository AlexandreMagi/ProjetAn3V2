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

    BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other)
    {
        timeBeforeStartIncrem = timeBeforeStart;
        collide = true;
        boxCollider.enabled = false;
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
