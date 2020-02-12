using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataFx/DataBulletFx")]
public class DataBulletFx : ScriptableObject
{
    public LayerMask mask;
    public string fxName;
    public string decalName = "Bullet Hole";
}
