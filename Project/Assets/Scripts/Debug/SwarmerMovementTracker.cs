using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SwarmerMovementTracker : MonoBehaviour
{
    [ShowInInspector]
    List<Vector3> historyOfVelocities;

    Rigidbody rb;

    int historySaveLength = 10;

    // Start is called before the first frame update
    void Start()
    {
        historyOfVelocities = new List<Vector3>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        historyOfVelocities.Add(rb.velocity);

        if(rb.velocity.y >= 5f)
        {
            Debug.Log("Anomaly detected");

            int sizeOfhistory = historyOfVelocities.Count;
            for (int i = sizeOfhistory - historySaveLength >= 0 ? sizeOfhistory - historySaveLength : 0; i<sizeOfhistory; i++)
            {
                Vector3 currentVel = historyOfVelocities[i];
                Debug.Log($"Velocity : X = {currentVel.x} - Y = {currentVel.y} - Z = {currentVel.z}");
            }
           
            Debug.Break();
        }
    }
}
