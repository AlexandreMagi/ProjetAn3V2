using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Public/DataPublic")]
public class DataPublic : ScriptableObject
{
    [SerializeField]
    public int startViewers = 500;

    [SerializeField]
    public int bufferSize = 8;

    [SerializeField]
    public int baseViewerGrowth = 50;

    [SerializeField]
    public int baseViewerLoss = 40;

    [SerializeField]
    public int randomViewerLoss = 10;

    [SerializeField]
    public int randomViewerGrowth = 8;

    [SerializeField]
    public float bufferStallAffect = .8f;

    [SerializeField]
    public int antiFarmCap = 3;
}
