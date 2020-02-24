using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDiorama : MonoBehaviour
{


    [SerializeField]
    GameObject hCam;

    [SerializeField]
    [Tooltip("en seconde pour un tour")]
    float floatVitesseRotation;

    // Start is called before the first frame update
    void Start()
    {
        
        

    }

    // Update is called once per frame
    void Update()
    {

        transform.Rotate(new Vector3(0,360/ floatVitesseRotation, 0)*Time.deltaTime,Space.Self);

    }
}
