using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEasterEggHandler : MonoBehaviour
{

    public static UIEasterEggHandler Instance { get; private set; }
    private void Awake() { Instance = this; }

    [SerializeField] Animator anmtr = null;
    [SerializeField] string trigger = "";

    [SerializeField] GameObject Juggernaut = null;
    [SerializeField] GameObject Aikant = null;
    [SerializeField] GameObject Fanfarons = null;

    public void TriggerVisualDisplayEasterEgg(EasterEggHandler.SpecialBonusType type)
    {
        Juggernaut.SetActive(false);
        Aikant.SetActive(false);
        Fanfarons.SetActive(false);

        switch (type)
        {
            case EasterEggHandler.SpecialBonusType.juggernaut:
                Juggernaut.SetActive(true);
                break;
            case EasterEggHandler.SpecialBonusType.aikant:
                Aikant.SetActive(true);
                break;
            case EasterEggHandler.SpecialBonusType.fanfaron:
                Fanfarons.SetActive(true);
                break;
            default:
                break;
        }
        anmtr.SetTrigger(trigger);
    }

}
