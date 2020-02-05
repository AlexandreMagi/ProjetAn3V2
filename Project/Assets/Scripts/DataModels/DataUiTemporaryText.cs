using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataUiTemporaryText")]
public class DataUiTemporaryText : ScriptableObject
{
    public Color colorGood = Color.blue;
    public Color colorBad = Color.red;
    public Color colorOutline = Color.black;

    public float fontSize = 100;

    public float timeToScaleToMax = 0.5f;
    public float timeStayVisible = 1;
    public float timeToFade = 1;

    public float outlineDistance = 5;

    public float randomPos = 0.4f;
}


