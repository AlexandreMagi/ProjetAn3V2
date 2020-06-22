using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EasterEggHandler : MonoBehaviour
{

    public static EasterEggHandler Instance { get; private set; }
    private void Awake() { Instance = this; DontDestroyOnLoad(gameObject); }

    public enum SpecialBonusType { juggernaut, aikant, fanfaron };
    [HideInInspector] public bool FanfaronUnlocked = false;
    [HideInInspector] public bool JuggernautUnlocked = false;
    [HideInInspector] public bool AikantUnlocked = false;

    [HideInInspector] public bool FanfaronUnlockedNextGame = false;
    [HideInInspector] public bool JuggernautUnlockedNextGame = false;
    [HideInInspector] public bool AikantUnlockedNextGame = false;

    [ShowIf("FanfaronUnlocked")] public bool FanfaronEnabled = false;
    [ShowIf("JuggernautUnlocked")] public bool JuggernautEnabled = false;
    [ShowIf("AikantUnlocked")] public bool AikantEnabled = false;

    public void UnlockAllBonusAtNextGame()
    {
        FanfaronUnlockedNextGame = true;
        JuggernautUnlockedNextGame = true;
        AikantUnlockedNextGame = true;
    }
    public void UnlockAllBonusAtNow()
    {
        FanfaronUnlocked = true;
        JuggernautUnlocked = true;
        AikantUnlocked = true;
    }

    public void EndGameHandleEasterEgg()
    {
        FanfaronUnlocked = FanfaronUnlockedNextGame;
        JuggernautUnlocked = JuggernautUnlockedNextGame;
        AikantUnlocked = AikantUnlockedNextGame;

        FanfaronUnlockedNextGame = false;
        JuggernautUnlockedNextGame = false;
        AikantUnlockedNextGame = false;

        FanfaronEnabled = false;
        JuggernautEnabled = false;
        AikantEnabled = false;
    }

}
