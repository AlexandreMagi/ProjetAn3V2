using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class ScriptFlicker : MonoBehaviour
{

    [SerializeField] bool independantFromTimeScale = false;
    [SerializeField] bool flickerPosition = false;
    [SerializeField, ShowIf("flickerPosition")] float moveRange = 0.1f;
    [SerializeField, ShowIf("flickerPosition")] float moveFlickDuration = 0.1f;
    Vector3 initPos;
    Vector3 currentPos;
    Vector3 targetPos = Vector3.zero;
    float timeLeftPos;

    // Start is called before the first frame update
    void Start()
    {
        initPos = transform.localPosition;// Save de la position de base
        currentPos = initPos; // Setup de la var de position à celle de base
    }

    // Update is called once per frame
    void Update()
    {
        if (flickerPosition)
        {
            float dt = independantFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            if (timeLeftPos < dt)
            {
                targetPos = initPos + Random.insideUnitSphere * moveRange;
                timeLeftPos = moveFlickDuration;
            }
            else
            {
                float valueLerp = dt / timeLeftPos; // "poids" du changement en fonction du dt, on divise par le temps restant pour que le deplacement soit fluide
                currentPos = Vector3.Lerp(currentPos, targetPos, valueLerp); // lerp de la valeur actuelle vers la valeur visée
                transform.localPosition = currentPos;
                timeLeftPos -= dt;
            }

        }
    }
}
