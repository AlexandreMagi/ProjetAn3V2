using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRCameraParser : MonoBehaviour
{
    [Header("Paramettre du jeux")]
    public int iResolutionX = 1920;
    public int iResolutionY = 1080;

    [SerializeField]
    float factorX = 3f;
    [SerializeField]
    float factorY = 1.6f;

    bool[] bTableBouton = new bool[3];



    //------ data a transmettre
    [Header("Valeur a récupérer pour le jeux")]
    public bool isShotDown = false;
    public bool isShotHeld = false;
    public bool isShotUp = false;

    public bool isGravityDown = false;
    public bool isGravityHeld = false;
    public bool isGravityUp = false;

    public bool isReloadDown = false;
    public bool isReloadHeld = false;
    public bool isReloadUp = false;

    public float fPositionX = 0;
    public float fPositionY = 0;
    //------ ------------------------


    int[] iTablePosition = new int[2];
    int[] iTableInputs = new int[3];
    int iDistance = 0;

    private ARdunioConnect scrptArduinoConnect = null;

    public static IRCameraParser Instance { get; private set; }

    void Awake()
    {

        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {
        scrptArduinoConnect = GetComponent<ARdunioConnect>();

        for (int i = 0; i < 2; i++)
        {
            iTablePosition[i] = 0;
        }

        V3LastData = Vector3.zero;

    }

    //--------------------------- Update ----------------------//
    void Update()
    {
        
        funcUpdateReception();
        funcUpdateEnvoie();

    }


    float fIncrémentationTime;
    float fMaxTime = 3;
    void funcUpdateEnvoie()
    {
        if(fIncrémentationTime > fMaxTime)
        {

            fIncrémentationTime = fIncrémentationTime + Time.deltaTime;

        }
        else
        {

            scrptArduinoConnect.funcSendArduino("Bonjour");


        }


    }

    void funcUpdateReception() 
    {

        string data = scrptArduinoConnect.getLastDataFromDevice();
        //Debug.Log($"Mes data arduino : {data}");

        if (data != null && data.Split(',').Length > 3)
        {
            string[] sTableDataType = funcTraitementSectorisation(data);

            iTableInputs = funcTraitementDataSimpleEntrer(sTableDataType[0], 3);

            iTablePosition = funcTraitementDataSimpleEntrer(sTableDataType[2], 2);

            int[] iTableSauv = funcTraitementDataSimpleEntrer(sTableDataType[1], 1);
            iDistance = iTableSauv[0];

            funcTransmition();

        }


    }



    //--------------------------- traitement data ----------------------//
    string[] funcTraitementSectorisation(string sDataBrut)
    {
        string[] sTableDataFinal = new string[3];

        sTableDataFinal = sDataBrut.Split('|');

        return sTableDataFinal;
    }


    //int[,] funcTraitementDataDoubleEntrer(string sData)
    //{

    //    int[,] iTableSauvegardeData = new int[3, 2]; // ligne = numéro du capteur | colone = coordonée x,y

    //    string[] sTableData = sData.Split(',');

    //    for (int i = 0; i < 3; i++) // 3
    //    {

    //        for (int j = 0; j < 2; j++) // 2
    //        {

    //            iTableSauvegardeData[i, j] = int.Parse(sTableData[2 * i + j]);

    //        }
    //    }

    //    return iTableSauvegardeData;
    //}

    int[] funcTraitementDataSimpleEntrer(string sData,int iTailleTableau)
    {

        int[] iTableSauvegardeData = new int[iTailleTableau]; // ligne = numéro du capteur | colone = coordonée x,y

        string[] sTableData = sData.Split(',');

        //Debug.Log("sTableData longeur :" + sTableData.Length);

        for (int i = 0; i < iTailleTableau; i++)
        {
            //Debug.Log("i = " + i + "  sTable = " + sTableData[i]);
            iTableSauvegardeData[i] = int.Parse(sTableData[i]);

        }

        return iTableSauvegardeData;
    }

    //----------- getion envoie


    void funcTransmition()
    {

        for (int i = 0; i < 3; i++)
        {

            if (iTableInputs[i] == 0)
            {

                bTableBouton[i] = true;

            }
            else
            {

                bTableBouton[i] = false;

            }

        }


        if (iTablePosition[0] != -1000)
        {

            fPositionX = iTablePosition[0] * factorX + (iResolutionX / 2);
            fPositionY = iTablePosition[1] * factorY + (iResolutionY / 2);

        }

        //------------------------------------------------ gestion bouton 

        if (bTableBouton[0])
        {
            if (isShotDown)
            {
                isShotDown = false;
            }
            else
            {
                if (!isShotHeld)
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

        if (bTableBouton[1])
        {
            if (isGravityDown)
            {
                isGravityDown = false;
            }
            else
            {
                if (!isGravityHeld)
                    isGravityDown = true;
            }

            isGravityHeld = true;
        }
        else
        {
            if (isGravityHeld)
            {
                isGravityUp = true;
            }
            else
            {
                isGravityUp = false;
            }
            isGravityHeld = false;
            isGravityDown = false;
        }

        if (bTableBouton[2])
        {
            if (isReloadDown)
            {
                isReloadDown = false;
            }
            else
            {
                if (!isReloadHeld)
                    isReloadDown = true;
            }

            isReloadHeld = true;
        }
        else
        {
            if (isReloadHeld)
            {
                isReloadUp = true;
            }
            else
            {
                isReloadUp = false;
            }
            isReloadHeld = false;
            isReloadDown = false;
        }


    }

    Vector3 V3LastData;

    public Vector3 funcPositionsCursorArduino()
    {
        Vector3 V3Positon = Vector3.zero;

        Vector3 V3Data = new Vector3(fPositionX, fPositionY, 0);

        Vector3 V3Delta = V3Data - V3LastData;

        if (V3Delta.magnitude > 10)
        {

            V3Positon = V3Data;
            V3LastData = V3Data;

        }
        else
        {

            V3Positon = V3LastData + (V3Data - V3LastData) * (0.05f * V3Delta.magnitude);
            V3LastData = V3Positon;

        }

        return V3Positon;

    }

}
