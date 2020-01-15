using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataReloadGraph")]
public class DataReloadGraph : ScriptableObject
{


    public Color barColor = Color.white;
    public Color extremityColor = Color.white;
    public Color checkBarColor = Color.green;
    public Color checkBarColorFailed = Color.red;
    public Color perfectSpotColor = Color.blue;

    [Header("Reload Anim")]
    public AnimationCurve verticalScaleAnimAatSpawn = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve horizontalScaleAnimAatSpawn = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve scaleAnimOnPerfectIndicator = AnimationCurve.Linear(0, 0, 1, 1);
    public float animDuration = 0.2f;

    public float idleSpeed = 7;
    public float idleMagnitude = 0.3f;
    public float reducingTime = 0.2f;
    public float perfectAnimScaleMultiplier = 1;
    public float perfectAnimtime = 0.2f;
}

