using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealLightStaticObjetsManager : MonoBehaviour
{
    [SerializeField]
    Material revealLightMaterial = null;

    Material[] matArray;

    Renderer matRenderer;

    void Start()
    {
        matRenderer = GetComponent<Renderer>();

        matArray = new Material[2];
        matArray[0] = matRenderer.material;
        
        StartCoroutine(addRevealMat());
    }

    IEnumerator addRevealMat()
    {
        yield return new WaitForSeconds(0.1f);

        matArray[1] = revealLightMaterial;
        matRenderer.materials = matArray;

        yield break;
    }

}
