using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FractureAnimHandler : MonoBehaviour
{
    [SerializeField]
    GameObject baseMesh = null;

    [SerializeField]
    GameObject fracturedMeshs = null;

    [SerializeField]
    GameObject fracturedMeshsStatic = null;

    private void Start()
    {
        baseMesh.SetActive(true);
        fracturedMeshs.SetActive(false);
        fracturedMeshsStatic.SetActive(false);
    }

    public void CallFracturedMesh()
    {
        if (fracturedMeshs != null)
        {
            baseMesh.SetActive(false);
            fracturedMeshs.SetActive(true);
        }
    }

    public void CallStaticMesh()
    {
        if (fracturedMeshsStatic != null)
        {
            fracturedMeshs.SetActive(false);
            fracturedMeshsStatic.SetActive(true);
        }
    }

}
