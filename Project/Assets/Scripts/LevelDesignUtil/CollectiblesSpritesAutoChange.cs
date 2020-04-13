using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesSpritesAutoChange : MonoBehaviour
{
    [SerializeField]
    Texture[] collectiblesSprites;

    MeshRenderer meshRenderer1;
    MeshRenderer meshRenderer2;

    Material instancedMaterial1;
    Material instancedMaterial2;

    [SerializeField]
    GameObject collectibleMesh1;
    
    [SerializeField]
    GameObject collectibleMesh2;

    void Start()
    {
        meshRenderer1 = collectibleMesh1.gameObject.GetComponent<MeshRenderer>();
        meshRenderer2 = collectibleMesh2.gameObject.GetComponent<MeshRenderer>();

        instancedMaterial1 = meshRenderer1.material;
        instancedMaterial2 = meshRenderer2.material;

        if (collectiblesSprites != null)
        {
            Texture text;

            text = collectiblesSprites[Random.Range(0, collectiblesSprites.Length)];

            instancedMaterial1.SetTexture("_CollectibleTexture", text);
            instancedMaterial2.SetTexture("_CollectibleTexture", text);
        }
    }

}
