using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataUiTemporarySprite")]
public class DataUiTemporarySprite : ScriptableObject
{
    public Color colorRandomOne = Color.red;
    public Color colorRandomTwo = Color.yellow;
    public Color colorOutline = Color.red;

    public Vector2 sizeRandom = new Vector2(100, 500);

    public float timeToScaleToMax = 0.5f;
    public float timeStayVisible = 1;
    public float timeToFade = 1;

    public float outlineDistance = 5;

    [PropertyRange(0f,1f)]
    public float rangePositionRandom = 0.2f;

    public Sprite spriteToSend = null;
}


