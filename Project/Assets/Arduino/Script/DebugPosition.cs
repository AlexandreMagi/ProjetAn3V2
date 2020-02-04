using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPosition : MonoBehaviour
{

    [SerializeField]
    GameObject[] hImageDebug;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        int[,] iTablePositionRecup = GetComponent<IRCameraParser>().iTablePosition;

        funcPosition(iTablePositionRecup);
               
    }


    void funcPosition(int[,] iTableReference)
    {

        //Debug.Log("funcPosition");

        for (int i = 0; i < 2; i++)
        {

            hImageDebug[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(iTableReference[i, 0], iTableReference[i, 1]);
            //hImageDebug[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 10);
        }

        int iResolutionX = 1920;
        int iResolutionY = 1080;


        float factorX = 3f;
        float factorY = 1.6f;
        //hImageDebug[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(iTableReference[2, 0], iTableReference[2, 1]);
        hImageDebug[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(iTableReference[2, 0] * factorX + (iResolutionX / 2), iTableReference[2, 1] * factorY + (iResolutionY / 2));


    }
}
