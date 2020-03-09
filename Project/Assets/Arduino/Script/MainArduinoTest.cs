using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainArduinoTest : MonoBehaviour
{

    [SerializeField]
    RectTransform hImageData;
    [SerializeField]
    RectTransform hImageMir;

    [SerializeField]
    IRCameraParser ScriptRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        hImageData.anchoredPosition = ScriptRef.funcPositionsCursorArduino();

    }
}
