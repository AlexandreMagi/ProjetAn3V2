using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealLightManager : MonoBehaviour
{

    Renderer render;

    Material instancedMaterial;

    [SerializeField]
    float valueStart = 0;

    void Start()
    {
        render = GetComponent<Renderer>();
        instancedMaterial = render.materials[1];
        instancedMaterial.SetFloat("_SelfEmittingValue", valueStart);
    }
}
