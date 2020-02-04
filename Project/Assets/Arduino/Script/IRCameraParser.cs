using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IRCameraParser : MonoBehaviour
{

    public int[,] iTablePosition = new int [3, 2];
    public int[] iTableInputs = new int [3];

    private ARdunioConnect scrptArduinoConnect = null;

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
        Debug.Log(data);

        if (data != null && data.Split(',').Length > 7)
        {
            string[] sTableDataType = funcTraitementSectorisation(data);

            iTableInputs = funcTraitementDataSimpleEntrer(sTableDataType[0]);

            iTablePosition = funcTraitementDataDoubleEntrer(sTableDataType[1]);

            
        }
        else
        {

            
            for (int i = 0; i < 3; i++)
            {
                iTableInputs[i] = 1;


                for (int j = 0; j < 2; j++)
                {

                    iTablePosition[i, j] = -1000;
                    

                }

            }

        }

        
    }


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
               
                iTableSauvegardeData[i, j] =  int.Parse(sTableData[2 * i + j]);

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



}
