using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class publicMaterialHandler : MonoBehaviour
{

    [SerializeField]Transform mesh = null;
    [SerializeField]Renderer rdr = null;
    MaterialPropertyBlock propBlock = null;
    Animator anmtr = null;

    [SerializeField] Texture[] allSprites = null;

    // Start is called before the first frame update
    void Start()
    {
        anmtr=GetComponent<Animator>();
        anmtr.speed = Random.Range(0.5f, 1.1f);
        propBlock = new MaterialPropertyBlock();

        mesh.transform.localScale = new Vector3(mesh.transform.localScale.x * Mathf.Sign(Random.Range(-1, 1)), mesh.transform.localScale.y, mesh.transform.localScale.z);

        if (rdr != null)
        {
            propBlock = new MaterialPropertyBlock();
            rdr.GetPropertyBlock(propBlock);
            propBlock.SetTexture("_MainTex", allSprites[Random.Range(0, allSprites.Length)]);
            rdr.SetPropertyBlock(propBlock);
        }
        
    }
}
