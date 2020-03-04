﻿using System.Collections;
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


    int[,] iTablePosition = new int[3, 2];
    int[] iTableInputs = new int[3];

    private ARdunioConnect scrptArduinoConnect = null;

    public static IRCameraParser Instance { get; private set; }

    void Awake()
    {

        Instance = this;
        iResolutionX = Screen.width;
        iResolutionY = Screen.height;

    }

    // Start is called before the first frame update
    void Start()
    {
        scrptArduinoConnect = GetComponent<ARdunioConnect>();

        for (int i = 0; i < 3; i++)
        {

            for (int j = 0; j < 2; j++)
            {
                iTablePosition[i, j] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(iTablePosition[2,0] + "  " + iTablePosition[2, 1]);

        string data = scrptArduinoConnect.getLastDataFromDevice();
        //Debug.Log(data);

        if (data != null && data.Split(',').Length > 7)
        {
            string[] sTableDataType = funcTraitementSectorisation(data);

            iTableInputs = funcTraitementDataSimpleEntrer(sTableDataType[0]);

            iTablePosition = funcTraitementDataDoubleEntrer(sTableDataType[1]);

            funcTransmition();


        }
        else
        {

            //Debug.Log("Perte Signial");

            //for (int i = 0; i < 3; i++)
            //{
            //    //iTableInputs[i] = 1;


            //    for (int j = 0; j < 2; j++)
            //    {

            //        /// -------------------------------------a modifier dans arduino (gardé la derniere valeur quand sortie de l'écrand)
            //        iTablePosition[i, j] = -1000;


            //    }

            //}

        }




    }


    //--------------------------- traitement data ----------------------//
    string[] funcTraitementSectorisation(string sDataBrut)
    {
        string[] sTableDataFinal = new string[2];

        sTableDataFinal = sDataBrut.Split('|');

        return sTableDataFinal;
    }


    int[,] funcTraitementDataDoubleEntrer(string sData)
    {

        int[,] iTableSauvegardeData = new int[3, 2]; // ligne = numéro du capteur | colone = coordonée x,y

        string[] sTableData = sData.Split(',');

        for (int i = 0; i < 3; i++) // 3
        {

            for (int j = 0; j < 2; j++) // 2
            {

                iTableSauvegardeData[i, j] = int.Parse(sTableData[2 * i + j]);

            }
        }

        return iTableSauvegardeData;
    }

    int[] funcTraitementDataSimpleEntrer(string sData)
    {

        int[] iTableSauvegardeData = new int[3]; // ligne = numéro du capteur | colone = coordonée x,y

        string[] sTableData = sData.Split(',');

        //Debug.Log("sTableData longeur :" + sTableData.Length);

        for (int i = 0; i < 3; i++)
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


        if (iTablePosition[2, 0] != -1000)
        {

            fPositionX = iTablePosition[2, 0] * factorX + (iResolutionX / 2);
            fPositionY = iTablePosition[2, 1] * factorY + (iResolutionY / 2);

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

    public Vector3 funcPositionsCursorArduino()
    {

        return new Vector3(fPositionX, fPositionY, 0);

    }

}
