using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataFx/DataBulletFx")]
public class DataBulletFx : ScriptableObject
{
    public LayerMask mask;
    public string tag;
    public string fxName;
    public string decalName = "Bullet Hole";
    public float decalSizeMultiplier = 1;
    public string soundPlayed = "BulletImpact";
    public string mixerPlayed = "Player";
    public float soundVolume = 1;
    public float soundPitchRandom = 0.2f;
    public float delayBeforeSound = 0.1f;
}
