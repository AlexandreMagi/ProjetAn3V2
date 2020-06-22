﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBonusManager : MonoBehaviour
{

    [SerializeField] SpecialBonus[] allSpecialBonus = null;
    //[SerializeField] string booleanSequenceName = "secretBooleanSequence";
    //int nbBonusDestroyed = 0;
    //int nbBonusToDestroy = 0;


    void Start()
    {
        if (allSpecialBonus != null)
        {
            for (int i = 0; i < allSpecialBonus.Length; i++)
            {
                if (allSpecialBonus[i] != null) allSpecialBonus[i].manager = this;
            }
            //nbBonusToDestroy = allSpecialBonus.Length;
        }
    }

    public void BonusDestroyed(EasterEggHandler.SpecialBonusType bonusType)
    {
        switch (bonusType)
        {
            case EasterEggHandler.SpecialBonusType.juggernaut:
                TitlesManager.Instance.ChangeTitleState(18, true);
                break;
            case EasterEggHandler.SpecialBonusType.aikant:
                TitlesManager.Instance.ChangeTitleState(19, true);
                break;
            case EasterEggHandler.SpecialBonusType.fanfaron:
                TitlesManager.Instance.ChangeTitleState(20, true);
                break;
        }
        //nbBonusDestroyed++;
        //Debug.Log(nbBonusDestroyed + " / " + nbBonusToDestroy);
        //if (nbBonusDestroyed == nbBonusToDestroy)
        //{
        //    Debug.Log("Change Sequence");
        //    BooleanSequenceManager.Instance.SetStateOfBoolSequence(booleanSequenceName, true);
        //}
    }

}
