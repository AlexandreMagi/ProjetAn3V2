using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDiorama : MonoBehaviour
{

    [SerializeField]
    [Tooltip("en seconde pour un tour")]
    float floatVitesseRotation = 1;

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(new Vector3(0,360/ floatVitesseRotation, 0)*Time.deltaTime,Space.World);

    }
}
