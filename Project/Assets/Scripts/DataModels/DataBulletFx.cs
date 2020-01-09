﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataFx/DataBulletFx")]
public class DataBulletFx : ScriptableObject
{
    public LayerMask mask;
    public string fxName;
}
