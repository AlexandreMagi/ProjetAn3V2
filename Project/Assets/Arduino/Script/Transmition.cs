using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transmition : MonoBehaviour
{
    public int iResolutionX = 1920;
    public int iResolutionY = 1080;

    [SerializeField]
    float factorX = 3f;
    [SerializeField]
    float factorY = 1.6f;


    public bool isShotDown = false;
    public bool isShotHeld = false;
    public bool isShotUp = false;


    //------ data a transmettre
    [Header("Valeur a récupérer pour le jeux")]
    public bool[] bTableBouton = new bool[3];
    public float fPositionX = 0;
    public float fPositionY = 0;

    IRCameraParser scriptData = null;

    public static Transmition Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        scriptData = GetComponent<IRCameraParser>();

    }

    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < 3; i++)
        {

            if(scriptData.iTableInputs[i] == 0)
            {

                bTableBouton[i] = true;

            } else
            {

                bTableBouton[i] = false;

            }

        }


        if(scriptData.iTablePosition[2, 0] != -1000)
        {

            fPositionX = scriptData.iTablePosition[2, 0] * factorX + (iResolutionX / 2);
            fPositionY = scriptData.iTablePosition[2, 1] * factorY + (iResolutionY / 2);

        }
        else
        {
            
            fPositionX = -1000;
            fPositionY = -1000;


        }


        if (bTableBouton[0])
        {
            if (isShotDown)
            {
                isShotDown = false;
            }
            else
            {
                isShotDown = true;
            }

            isShotHeld = true;
        }
        else
        {
            if (isShotHeld)
            {
                isShotUp = true;
            }
            else
            {
                isShotUp = false;
            }
            isShotHeld = false;
            isShotDown = false;
        }
    }

    public Vector3 positions()
    {
        return new Vector3(fPositionX, fPositionY,0);
    }
}
