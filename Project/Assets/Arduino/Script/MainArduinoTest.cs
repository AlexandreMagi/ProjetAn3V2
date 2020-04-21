using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainArduinoTest : MonoBehaviour
{

    [SerializeField]
    RectTransform hImageData = null;
    [SerializeField]
    RectTransform hImageMir = null;

    [SerializeField]
    IRCameraParser ScriptRef = null;


    //float fTimeIncrementation = 0;
    //float fTimeMax = 1.5f;

    Vector3 V3LastData;


    // Start is called before the first frame update
    void Start()
    {
        V3LastData = ScriptRef.funcPositionsCursorArduino();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 V3Data = ScriptRef.funcPositionsCursorArduino();

        hImageData.anchoredPosition = V3Data;

        Vector3 V3Delta = V3Data - V3LastData;
        Debug.Log(V3Delta.magnitude);


        if (V3Delta.magnitude > 10)
        {

            hImageMir.anchoredPosition = V3Data;
            V3LastData = V3Data;

        }
        else 
        {
            
            hImageMir.anchoredPosition = V3LastData + (V3Data - V3LastData) * (0.05f * V3Delta.magnitude);
            V3LastData = hImageMir.anchoredPosition;

        }

        

    }
}
