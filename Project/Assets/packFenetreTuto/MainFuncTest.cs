using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainFuncTest : MonoBehaviour
{
    public float fEtape = 0;

    [Header("A remplir")]
    [Space]
    public float fTempsAttente;
    public float iSpeedTranslate;
    public float iSpeedScale;

    RectTransform FenetreRectTransform;

    Vector2 v2Anchor;

    Vector2 v2Pivot;

    Rect RectFenetreBase;
    public Rect RectFenetreFinal;

    [Header("A activé dans le code")]
    [Space]
    public bool bActivation = false;


    [Header("PAS TOUCHER")]
    [Space]
    //------
    public GameObject hTexte;
    public GameObject hBouton;

    public Text uiTextTutoDisplayed = null;

    // Start is called before the first frame update
    void Start()
    {
        FenetreRectTransform = GetComponent<RectTransform>();

        //AxisFenetreBaseX = FenetreRectTransform.anchoredPosition.x;
        //AxisFenetreBaseY = FenetreRectTransform.anchoredPosition.y;

        v2Anchor = FenetreRectTransform.anchorMin;

        if(v2Anchor.x == 0)
        {

            v2Anchor.x = -1;

        }

        if (v2Anchor.y == 0)
        {

            v2Anchor.y = -1;

        }

        v2Pivot = FenetreRectTransform.pivot;


        RectFenetreBase = FenetreRectTransform.rect;

        //Debug.Log("RectFenetreBase : " + RectFenetreBase);

        RectFenetreBase.x = FenetreRectTransform.anchoredPosition.x;
        RectFenetreBase.y = FenetreRectTransform.anchoredPosition.y;


        //Debug.Log("RectFenetreBase : " + RectFenetreBase);
        //funcDebug();
        funcPositionnemntEnfant();
    }


    void funcDebug()
    {

        Debug.Log("v2Anchor  : " + v2Anchor);
        Debug.Log("v2Pivot  : " + v2Pivot);
        Debug.Log("RectFenetreBase  : " + RectFenetreBase);
        Debug.Log("RectFenetreFinal  : " + RectFenetreFinal);

    }
    
    void funcPositionnemntEnfant()
    {
        //bouton------------
        RectTransform RectBouton = hBouton.GetComponent<RectTransform>();

        if(v2Anchor.y == -1)
        {

            RectBouton.anchorMin = new Vector2(0.5f, 0);
            RectBouton.anchorMax = new Vector2(0.5f, 0);

        }
        else
        {

            RectBouton.anchorMin = new Vector2(0.5f, v2Anchor.y);
            RectBouton.anchorMax = new Vector2(0.5f, v2Anchor.y);

        }
        

        RectBouton.anchoredPosition = new Vector2(0,0);
        RectBouton.sizeDelta = new Vector2(80 + FenetreRectTransform.sizeDelta.x, 50);

        //Texte--------------
        RectTransform RectText = hTexte.GetComponent<RectTransform>();

        if (v2Anchor.y == -1)
        {

            RectText.anchorMin = new Vector2(0.5f, 0);
            RectText.anchorMax = new Vector2(0.5f, 0);
            RectText.pivot = new Vector2(0.5f, 0);

        }
        else
        {

            RectText.anchorMin = new Vector2(0.5f, v2Anchor.y);
            RectText.anchorMax = new Vector2(0.5f, v2Anchor.y);
            RectText.pivot = new Vector2(0.5f, v2Anchor.y);

        }

        

        RectText.anchoredPosition = new Vector2(0, 40* v2Anchor.y*-1);
        RectText.sizeDelta = new Vector2(-20 + FenetreRectTransform.sizeDelta.x, RectFenetreFinal.height - 40 - 20);

        hTexte.GetComponent<Text>().enabled = false ;

    }

    
    public void ChangeText (string sText)
    {
        uiTextTutoDisplayed.text = sText;
    }


    // Update is called once per frame
    void Update()
    {
        

        if (fEtape == 0 && bActivation == true)
        {
            
            fEtape = 1;

        }


        if (fEtape == 1)//Début translate
        {

            FenetreRectTransform.anchoredPosition = new Vector2(FenetreRectTransform.anchoredPosition.x + (iSpeedTranslate * (v2Anchor.x * -1)), FenetreRectTransform.anchoredPosition.y);

            //transform.Translate(new Vector3(iSpeedTranslate*(v2Anchor.x *- 1), 0, 0) * Time.deltaTime, Space.Self);

            if (FenetreRectTransform.anchoredPosition.x < RectFenetreFinal.x && v2Anchor.x == 1)
            {

                fEtape = 2;

            }else if (FenetreRectTransform.anchoredPosition.x > RectFenetreFinal.x && v2Anchor.x == -1)
            {

                fEtape = 2;

            }

        }

        if (fEtape == 2)//Début seize
        {
            
            FenetreRectTransform.sizeDelta = new Vector2(FenetreRectTransform.sizeDelta.x, FenetreRectTransform.sizeDelta.y + iSpeedScale * Time.deltaTime);

            if (FenetreRectTransform.sizeDelta.y > RectFenetreFinal.height)
            {

                fEtape = 2.1f;

            }

        }

        if (fEtape == 2.1f)//AFFICHAGE DU TEXT + ATTENTE
        {
            
            hTexte.GetComponent<Text>().enabled = true;
            
            StartCoroutine(funcTimer(fTempsAttente));

            fEtape = 2.2f;
        }

        if (fEtape == 2.9f)//retoure seize
        {

            hTexte.GetComponent<Text>().enabled = false;

            fEtape = 3;

        }


        if (fEtape == 3)//retoure seize
        {

            FenetreRectTransform.sizeDelta = new Vector2(FenetreRectTransform.sizeDelta.x, FenetreRectTransform.sizeDelta.y + iSpeedScale * (-1  * Time.deltaTime));

            if (FenetreRectTransform.sizeDelta.y < RectFenetreBase.height)
            {

                //Debug.Log("iEtape Condition = 4");
                fEtape = 4;

            }

        }

        if (fEtape == 4)//retoure translate
        {
            FenetreRectTransform.anchoredPosition = new Vector2(FenetreRectTransform.anchoredPosition.x + (iSpeedTranslate * (v2Anchor.x)), FenetreRectTransform.anchoredPosition.y);

            //transform.Translate(new Vector3(iSpeedTranslate * (v2Anchor.x), 0, 0) * Time.deltaTime, Space.Self);

            if (FenetreRectTransform.anchoredPosition.x > RectFenetreBase.x && v2Anchor.x == 1)
            {

                fEtape = 5;

            }else if (FenetreRectTransform.anchoredPosition.x < RectFenetreBase.x && v2Anchor.x == -1)
            {

                fEtape = 5;

            }

        }
        
        if (fEtape == 5)//retoure translate
        {

            bActivation = false;
            fEtape = 0;

        }



    }


    



    IEnumerator funcTimer(float fTime)
    {
        
        yield return new WaitForSeconds(fTime);
        
        fEtape = 2.9f;

        yield break;

    }


}
