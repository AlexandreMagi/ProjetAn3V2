using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ch.sycoforge.Decal;

public class DecalManager : MonoBehaviour
{
    private static DecalManager _instance;
    public static DecalManager Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }

    [SerializeField]
    EasyDecal[] decalTab = null;

    [SerializeField]
    float safeDistanceValue = 2;

    public EasyDecal ProjectDecal (string name, float castRadius,Ray rayBase, RaycastHit hitBase, int layer)
    {
        if (layer == 9)
        {
            EasyDecal decalInstance = FindDecal(name);
            if (decalInstance == null)
                return null;

            // Set the first hit as parent of the decal
            GameObject parent = hitBase.collider.gameObject;
            Vector3 pos = hitBase.point;

            RaycastHit[] hits = Physics.SphereCastAll(rayBase, castRadius, Vector3.Distance(CameraHandler.Instance.renderingCam.transform.position, pos) + safeDistanceValue);
            Vector3 averageNormal = hitBase.normal;

            // Check if sphere cast hit something
            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    // Sum all collison point normals
                    averageNormal += hit.normal;
                }
            }

            // Normalize normal
            averageNormal /= hits.Length + 1;

            // Instantiate the decal prefab according the hit normal
            EasyDecal sendDecal = EasyDecal.ProjectAt(decalInstance.gameObject, parent, pos, averageNormal);


            return sendDecal;
        }
        return null;
    }

    EasyDecal FindDecal (string name)
    {
        for (int i = 0; i < decalTab.Length; i++)
        {
            if (decalTab[i].name == name)
            {
                return decalTab[i];
            }
        }
        Debug.Log("ERROR - Decal named : '" + name + "' doesn't exist");
        return null;
    }


}
