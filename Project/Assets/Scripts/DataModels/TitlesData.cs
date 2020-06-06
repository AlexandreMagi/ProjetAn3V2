using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Titles")]
public class TitlesData : ScriptableObject
{
    //This is the class for all metrics affected datas

    [Header("Sniper")]
    public float aimRequiredForTitle = 60;

    [Header("Speedrunner")]
    public float timeForTitle = 900000; //Must be in milliseconds, corresponds to 15 mins

    [Header("In extremis")]
    public float healthThreshold = 1;

    [Header("Unshakable")]
    public float damageTakenRequired = 2500;

    [Header("Perfect reloads")]
    public float percentPerfectReloadForTitle = 50;

    [Header("Environmental kills")]
    public int numberOfEnviroKillsForTitle = 3;
}
