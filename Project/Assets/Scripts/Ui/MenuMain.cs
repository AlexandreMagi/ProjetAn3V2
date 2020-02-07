using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMain : MonoBehaviour
{

    private static MenuMain _instance;
    public static MenuMain Instance { get { return _instance; } }
    void Awake() { _instance = this; }

    public List<ButtonMenuScript> buttonMenuScripts;

    enum menustate { intro, home, mainmenu }
    menustate currentState = menustate.intro;

    // Update is called once per frame
    void Update()
    {

        switch (currentState)
        {
            case menustate.intro:
                if (Input.GetKeyDown(KeyCode.Mouse0)) SkipToHome();
                break;
            case menustate.home:
                break;
            case menustate.mainmenu:
                foreach (var button in buttonMenuScripts)
                {
                    bool mouseOver = button.CheckIfMouseOver(Input.mousePosition);
                    if (mouseOver)
                    {
                        button.transform.localScale = Vector3.Lerp(button.transform.localScale, Vector3.one * 1.2f, Time.unscaledDeltaTime);
                    }
                    else
                    {
                        button.transform.localScale = Vector3.Lerp(button.transform.localScale, Vector3.one * 1.0f, Time.unscaledDeltaTime);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse0)) Click(Input.mousePosition);
                break;
            default:
                break;
        }

    }

    void Click(Vector2 mousePosition)
    {
        foreach (var button in buttonMenuScripts)
        {
            button.Click(mousePosition);
        }
    }

    void SkipToHome()
    {
    }

    public void IsOnMainMenu()
    {
        currentState = menustate.mainmenu;
    }

    public void IsOnHome()
    {
        currentState = menustate.home;
    }

    public void IsOnIntro()
    {
        currentState = menustate.intro;
    }

}
