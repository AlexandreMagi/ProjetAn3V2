using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public static Player Instance{get; private set;}

    void Awake()
    {
        Instance = this;
    }
}
