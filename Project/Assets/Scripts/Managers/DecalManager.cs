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
    float maxDistToDecal = 12;

    [SerializeField]
    float scalePlane = 0.5f;
    [SerializeField]
    float safeTranslateValue = 0.1f;


    List<DecalInstance> allDecal = new List<DecalInstance>();

    [SerializeField]
    float timeStayNormal = 1;
    [SerializeField]
    float timeFade = 1;
    [SerializeField]
    float baseAlpha = 1;


    [SerializeField]
    List<Material> allDecalMat = new List<Material>();

    public Transform ProjectDecal(RaycastHit hitBase, string decalName = "")
    {

        if (CameraHandler.Instance == null || CameraHandler.Instance.GetDistanceWithCam(hitBase.point) < maxDistToDecal) 
        {
            if (!activeDecal)
            {
                Debug.Log("Decal not activated");
                return null;
            }

            //EasyDecal decalInstance = FindDecal(name);
            if (maxDecal == null)
                return null;

            Material overrideMaterial = null;
            if (decalName != "") overrideMaterial = FindDecalMat(decalName);


            GameObject planeInstance = Instantiate(planeForDecal, hitBase.point + hitBase.normal.normalized * safeTranslateValue, Quaternion.LookRotation(hitBase.normal * -1));
            planeInstance.transform.localScale = Vector3.one * scalePlane;
            planeInstance.transform.Rotate(Vector3.forward * Random.Range(0, 360), Space.Self);
            planeInstance.transform.SetParent(hitBase.collider.transform, true);

            MeshRenderer planeRenderer = planeInstance.GetComponent<MeshRenderer>();
            planeRenderer.material = overrideMaterial != null ? overrideMaterial : maxDecal;

            DecalInstance newDecal = new DecalInstance(planeRenderer.material, planeInstance, timeStayNormal + timeFade);

            allDecal.Add(newDecal);

            return planeInstance.transform;
        }
        else
        {
            return null;
        }

    }

    Material FindDecalMat(string decalName = "")
    {
        for (int i = 0; i < allDecalMat.Count; i++)
        {
            if (allDecalMat[i].name == decalName)
            {
                return allDecalMat[i];
            }
        }
        Debug.Log("Decal named '" + decalName + "' doesn't exist in the mat tab in Decal Manager");
        return null;
    }

    private void Update()
    {
        for (int i = allDecal.Count-1; i > -1; i--)
        {
            float currAlpha = baseAlpha;
            allDecal[i].lifeTime -= Time.deltaTime;

            if (allDecal[i].lifeTime < 0 || CameraHandler.Instance != null && allDecal[i].go != null && CameraHandler.Instance.GetDistanceWithCam(allDecal[i].go.transform.position) > maxDistToDecal)
            {
                GameObject planeInstance = allDecal[i].go;
                allDecal.RemoveAt(i);
                Destroy(planeInstance);                                     
            }
            else
            {
                if (allDecal[i].lifeTime < timeFade) currAlpha = allDecal[i].lifeTime * baseAlpha / timeFade;
                if (allDecal[i].go != null)
                    allDecal[i].go.transform.localScale = Vector3.one * scalePlane * currAlpha;
                //SETUP ALPHA ICI
            }


        }
    }


}

public class DecalInstance
{
    public Material mat = null;
    public GameObject go = null;
    public float lifeTime = 0;

    public DecalInstance(Material _mat, GameObject _go, float _lifeTime)
    {
        mat = _mat;
        go = _go;
        lifeTime = _lifeTime;
    }
}
