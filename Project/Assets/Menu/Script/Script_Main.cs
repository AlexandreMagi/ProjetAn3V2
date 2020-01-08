using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Script_Main : MonoBehaviour
{

    public GameObject hTablePanelMenu;
    public GameObject hPangoblin;
    public GameObject hEtpa;
    public GameObject hFond;
    public Slider hSlider;

    int iEtape = -1;


    [Header("Code Menu ")]
    [Tooltip("les numeraux dans le tableaux forme une sequence qui permet d'activer les mise a jouer")]
    public int[] iTableCode = new int[3]; // code etape
    public int[] iTableNumbrePanel = new int[3]; // code etape


    

    // Start is called before the first frame update
    void Start()
    {
        hSlider.gameObject.SetActive(false);
        hEtpa.GetComponent<CanvasGroup>().alpha = 0;
        hPangoblin.GetComponent<CanvasGroup>().alpha = 0;

    }

    float TotalSecondeEcouler = 0;
    float DPM = 0.5f;

    // Update is called once per frame
    void Update()
    {

        if (iEtape == -1)
        {

            hPangoblin.GetComponent<CanvasGroup>().alpha = hPangoblin.GetComponent<CanvasGroup>().alpha + 0.5f * Time.deltaTime;

            if (hPangoblin.GetComponent<CanvasGroup>().alpha >= 1)
            {

                iEtape = 0;

            }

        }


        if (iEtape == 0)
        {
            StartCoroutine(CouAttente(2,1));

            iEtape = 1;

        }

        if(iEtape == 1)
        {


        }

        if (iEtape == 2)
        {

            hPangoblin.GetComponent<CanvasGroup>().alpha = hPangoblin.GetComponent<CanvasGroup>().alpha - 1 * Time.deltaTime;

            if (hPangoblin.GetComponent<CanvasGroup>().alpha <= 0)
            {

                iEtape = 3;

            }

        }

        if (iEtape == 3)
        {

            hEtpa.GetComponent<CanvasGroup>().alpha = hEtpa.GetComponent<CanvasGroup>().alpha + 1 * Time.deltaTime;

            if (hEtpa.GetComponent<CanvasGroup>().alpha >= 1)
            {

                iEtape = 4;
                StartCoroutine(CouAttente(5, 1));

            }

        }

        if (iEtape == 4)
        {


        }


        if (iEtape == 5)
        {

            hFond.GetComponent<CanvasGroup>().alpha = hFond.GetComponent<CanvasGroup>().alpha - 1 * Time.deltaTime;
            hEtpa.GetComponent<CanvasGroup>().alpha = hEtpa.GetComponent<CanvasGroup>().alpha - 1 * Time.deltaTime;

            if (hEtpa.GetComponent<CanvasGroup>().alpha <= 0)
            {

                iEtape = 6;

            }

        }





        if (iEtape == 6)
        {

            hEtpa.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 2000);
            hFond.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 2000);
            hPangoblin.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 2000);

            Animator A_Animator = hTablePanelMenu.GetComponent<Animator>();
            A_Animator.SetInteger("iEtapeAnimation", 1);

            iEtape = 11;


        }
        
        //------------------------------------------------------------- Pour le Rithme
        TotalSecondeEcouler = TotalSecondeEcouler + Time.deltaTime;

        if(TotalSecondeEcouler >= DPM)
        {

            TotalSecondeEcouler = 0;
            //Debug.Log("boom");

        }




    }

    GameObject hBouttonDernierCliquer;
    public void funcStockBouton(GameObject hBoutton)
    {

        hBouttonDernierCliquer = hBoutton;

    }

    public void funcSuivant(int iNumber)
    {
        
        //Debug.Log("bojour : " + iNumber);
        //Debug.Log("hBouttonDernierCliquer : " + hBouttonDernierCliquer);

        //--------------------------------------------recupération de tout les bouton
        //Crée un tableau de components vierge
        Component[] Button;

        //Récupère dans chaque chaque enfant tel type de component (Button) et le fout dans le tableau
        GameObject hPanel = hBouttonDernierCliquer.transform.parent.gameObject;
        Button = hPanel.GetComponentsInChildren<Button>();

        for (int i = 0; i < Button.Length-1; i++)
        {
            //Debug.Log(i );
            if (i != iNumber-1)
            {

                //Debug.Log(i + "  " + Button[i]);

                Animator A_AnimatorBoutonNonSelec = Button[i].GetComponent<Animator>();
                A_AnimatorBoutonNonSelec.SetInteger("iEtapeAnimator", -1);

            }           

        }

        Animator A_AnimatorBoutonSelection = Button[iNumber-1].GetComponent<Animator>();
        A_AnimatorBoutonSelection.SetInteger("iEtapeAnimator", 1);



        

        
        //Debug.Log(hPanel);
        StartCoroutine(CouAnimationReset(hPanel, Button));

        iEtape++;
        
    }

    



    public void funcRetour(GameObject hPanelPresedant)
    {


        Animator A_AnimationPanelPresedant = hPanelPresedant.GetComponent<Animator>();
        A_AnimationPanelPresedant.SetInteger("iEtapeAnimation", 3);


        //Debug.Log("transform.parent.gameObject : " + hBouttonDernierCliquer.transform.parent.gameObject);
        Animator A_AnimationPanelParent = hBouttonDernierCliquer.transform.parent.gameObject.GetComponent<Animator>();
        A_AnimationPanelParent.SetInteger("iEtapeAnimation", 0);


        //Debug.Log("hBouttonDernierCliquer : " + hBouttonDernierCliquer);
        //Animator A_AnimationBouton = hBouttonDernierCliquer.GetComponent<Animator>();
        //A_AnimationBouton.SetInteger("iTransitions", 1);
        

    }

    IEnumerator CouAnimationReset(GameObject hPanel, Component[] hBoutton)
    {
        //Debug.Log("Starte CouAnimationReset");
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("Starte anime");

        

        Animator A_AnimationPanel = hPanel.GetComponent<Animator>();
        A_AnimationPanel.SetInteger("iEtapeAnimation", 2);

        yield return new WaitForSeconds(0.2f);

        Animator A_AnimationPanelSuivant = hBouttonDernierCliquer.GetComponent<Script_ButtonStockage>().hPanelSuivant.GetComponent<Animator>();
        A_AnimationPanelSuivant.SetInteger("iEtapeAnimation", 1);


        for (int i = 0; i < hBoutton.Length-1; i++)
        {

            //Debug.Log(i + "  " + hBoutton[i]);

            Animator A_AnimatorBoutonNonSelec = hBoutton[i].GetComponent<Animator>();
            A_AnimatorBoutonNonSelec.SetInteger("iEtapeAnimator", 0);

        }


        yield return new WaitForSeconds(DPM - TotalSecondeEcouler);

        for (int i = 0; i < hBoutton.Length; i++)
        {

            //Debug.Log(i + "  " + hBoutton[i]);

            Animator A_AnimatorBoutonNonSelec = hBoutton[i].GetComponent<Animator>();

            A_AnimatorBoutonNonSelec.Play("Attente", 0, 0);

        }
        


        yield break;
                     
    }

    IEnumerator CouAttente(int iEtapeAmodifier, float fTempsAttente)
    {
        

        yield return new WaitForSeconds(fTempsAttente);

        iEtape = iEtapeAmodifier; 

        yield break;

    }


    public void funcCliqueStatScene()
    {
        hFond.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        hFond.GetComponent<CanvasGroup>().alpha = 1;
        StartCoroutine(LoadingScreen());

    }

    AsyncOperation async;
    IEnumerator LoadingScreen()
    {
        hSlider.gameObject.SetActive(true);
        async = SceneManager.LoadSceneAsync("LD_03", LoadSceneMode.Single);
        async.allowSceneActivation = false;

        while (async.isDone == false)
        {
            hSlider.value = async.progress;
            if (async.progress == 0.9f)
            {
                hSlider.value = 1f;
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
