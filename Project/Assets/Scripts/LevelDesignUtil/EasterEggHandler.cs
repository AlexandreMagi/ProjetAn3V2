using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EasterEggHandler : MonoBehaviour
{

    public static EasterEggHandler Instance { get; private set; }
    private void Awake() { Instance = this; DontDestroyOnLoad(gameObject); }

    public enum SpecialBonusType { juggernaut, aikant, fanfaron };
    public bool FanfaronUnlocked = false;
    public bool JuggernautUnlocked = false;
    public bool AikantUnlocked = false;

    [ShowIf("FanfaronUnlocked")] public bool FanfaronEnabled = false;
    [ShowIf("JuggernautUnlocked")] public bool JuggernautEnabled = false;
    [ShowIf("AikantUnlocked")] public bool AikantEnabled = false;

}
