using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatDisplayHandler : MonoBehaviour
{

    public static CheatDisplayHandler Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] Text hasCheatedText = null;
    [SerializeField] Text godModText = null;

    bool textsDisplayed = true;

    // Start is called before the first frame update
    void Start()
    {
        if (godModText != null)
            godModText.text = "";
        if (hasCheatedText != null)
            hasCheatedText.text = "";
    }

    public void GodMod (bool enabled)
    {
        if (godModText != null)
            godModText.text = enabled ? "GodMod Enabled" : "";
    }
    public void HasCheated ()
    {
        if (hasCheatedText != null)
            hasCheatedText.text = "Cheated Run";
    }

    public void ChangeCheatDisplay()
    {
        textsDisplayed = !textsDisplayed;

        if (godModText != null)
            godModText.gameObject.SetActive(textsDisplayed);
        if (hasCheatedText != null)
            hasCheatedText.gameObject.SetActive(textsDisplayed);

    }


}
