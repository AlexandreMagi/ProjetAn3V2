using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesSpritesAutoChange : MonoBehaviour
{
    [SerializeField]
    Texture[] collectiblesSprites = null;

    MeshRenderer meshRenderer;

    Material instancedMaterial;

    [SerializeField]
    GameObject collectibleMesh = null;

    void Start()
    {
        meshRenderer = collectibleMesh.gameObject.GetComponent<MeshRenderer>();

        instancedMaterial = meshRenderer.material;

        if (collectiblesSprites != null)
        {
            Texture text;

            text = collectiblesSprites[Random.Range(0, collectiblesSprites.Length)];

            instancedMaterial.SetTexture("_CollectibleTexture", text);
        }
    }

    public void HideMesh()
    {
        collectibleMesh.gameObject.SetActive(false);
    }

}
