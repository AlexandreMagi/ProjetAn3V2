using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataPostEffect", menuName = "ScriptableObjects/DataPostEffect")]
public class DataPostEffect : ScriptableObject
{
    [Header("Chroma")]
    public bool chromaChangeDependentFromTimeScale = true;
    public float chromaTimeTransition = 1;
    public float chromaMax = 1;
    public float chromaMin = 0;
}
