using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulbLightManager : MonoBehaviour
{

    [SerializeField, ColorUsage(true, true)]
    Color bulbColor = Color.white;

    Renderer _renderer;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
        Material _mat = _renderer.material;

        _mat.SetColor("_EmissionColor", bulbColor);

    }
}
