using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SetupDecalManager : MonoBehaviour
{
    [SerializeField]
    bool changeColor = false;

    [SerializeField, ShowIf("changeColor"), ColorUsage(true,true)]
    Color colorToApply = Color.white;

    [SerializeField, ShowIf("changeColor")]
    string colorRefToChange = "_Reveallightcolor";

    Renderer meshRenderer;

    Material instancedMaterial;

    void Start()
    {

        meshRenderer = GetComponent<Renderer>();
        instancedMaterial = meshRenderer.material;

        if (changeColor)
            instancedMaterial.SetColor(colorRefToChange, colorToApply);
        
    }
}
