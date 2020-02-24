using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintScript : MonoBehaviour
{

    int hintFontSize = 0;
    int hintFontSizeSaved = 0;

    float timerUntilDepop = 0;
    bool hasTimer = false;

    [SerializeField] Text hintText = null;
    [SerializeField] Animator anmtr = null;

    public static HintScript Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        hintFontSize = hintText.fontSize;
        hintFontSizeSaved = hintText.fontSize;
    }

    public void ChangeFontSize(int _size) { hintFontSize = _size; }
    public void ChangeFontSize() { hintFontSize = hintFontSizeSaved; }

    public void PopHint (string _text) { TruePopHint(_text, true); }
    public void PopHint (string _text, float _stayTime) { TruePopHint(_text, false, _stayTime); }

    void TruePopHint (string _text, bool _unlimited, float _stayTime = 0)
    {
        hintText.text = _text;
        hintText.fontSize = hintFontSize;
        hasTimer = !_unlimited;
        if (hasTimer) timerUntilDepop = _stayTime;
        anmtr.SetTrigger("pop");
    }

    void Depop()
    {
        anmtr.SetTrigger("depop");
        hasTimer = false;
        timerUntilDepop = 0;
        ChangeFontSize();
    }

    private void Update()
    {
        if (hasTimer)
        {
            if (timerUntilDepop > 0) timerUntilDepop -= Time.unscaledDeltaTime;
            if (timerUntilDepop < 0) Depop();
        }
        else timerUntilDepop = 0;
    }
}
