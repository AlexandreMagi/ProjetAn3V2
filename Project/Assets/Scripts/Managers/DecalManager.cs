using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Material maxDecal = null;
    [SerializeField]
    GameObject planeForDecal = null;

    [SerializeField]
    bool activeDecal = true;

    [SerializeField]
    float scalePlane = 0.5f;
    [SerializeField]
    float safeTranslateValue = 0.1f;

    List<Material> allMats = new List<Material>();
    List<GameObject> allGo = new List<GameObject>();
    List<float> allLifeTimes = new List<float>();

    [SerializeField]
    float timeStayNormal = 1;
    [SerializeField]
    float timeFade = 1;
    [SerializeField]
    float baseAlpha = 1;

    public Transform ProjectDecal(RaycastHit hitBase, Material overrideMaterial = null)
    {

        if (!activeDecal)
        {
            Debug.Log("Decal not activated");
            return null;
        }

        //EasyDecal decalInstance = FindDecal(name);
        if (maxDecal == null)
            return null;

        GameObject planeInstance = Instantiate(planeForDecal, hitBase.point + hitBase.normal.normalized * safeTranslateValue, Quaternion.LookRotation(hitBase.normal*-1));
        planeInstance.transform.localScale = Vector3.one * scalePlane;
        planeInstance.transform.Rotate(Vector3.forward * Random.Range(0, 360), Space.Self);
        planeInstance.transform.SetParent(hitBase.collider.transform, true);

        MeshRenderer planeRenderer = planeInstance.GetComponent<MeshRenderer>();
        planeRenderer.material = overrideMaterial != null ? overrideMaterial : maxDecal;

        allMats.Add(planeRenderer.material);
        allLifeTimes.Add(timeStayNormal + timeFade);
        allGo.Add(planeInstance);

        return planeInstance.transform;
    }

    private void Update()
    {
        for (int i = allLifeTimes.Count-1; i > -1; i--)
        {
            float currAlpha = baseAlpha;
            allLifeTimes[i] -= Time.deltaTime;

            if (allLifeTimes[i] < 0)
            {
                GameObject planeInstance = allGo[i];
                allMats.RemoveAt(i);
                allLifeTimes.RemoveAt(i);
                allGo.RemoveAt(i);
                Destroy(planeInstance);                                     
            }
            else
            {
                if (allLifeTimes[i] < timeFade) currAlpha = allLifeTimes[i] * baseAlpha / timeFade;
                if (allGo[i] != null)
                    allGo[i].transform.localScale = Vector3.one * scalePlane * currAlpha;
                //SETUP ALPHA ICI
            }


        }
    }


}
