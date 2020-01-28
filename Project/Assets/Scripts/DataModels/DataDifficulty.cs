using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Difficulty/DataDifficulty")]
public class DataDifficulty : ScriptableObject
{
    public enum Difficulty
    {
        Easy = 2,
        Normal = 4,
        Hard = 6
    }

    [EnumToggleButtons]
    public Difficulty difficulty;

    public bool playerCanReraise = false;

    public int armorOnRaise = 50;
    public int armorOnRaiseBonus = 30;

    public int maxChanceOfSurvival = 90;
}
