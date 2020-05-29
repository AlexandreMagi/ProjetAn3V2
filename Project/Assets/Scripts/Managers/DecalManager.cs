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

    [SerializeField] int maxNbDecal = 30;
    [SerializeField] float timeBeforeCreatingPull = 4;

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


    DecalInstance[] allDecal = null;

    [SerializeField]
    float timeStayNormal = 1;
    [SerializeField]
    float timeFade = 1;
    [SerializeField]
    float baseAlpha = 1;


    [SerializeField]
    List<Material> allDecalMat = new List<Material>();

    public void Start()
    {
        Invoke("CreatePull", timeBeforeCreatingPull);   
    }

    public void CreatePull()
    {
        allDecal = new DecalInstance[maxNbDecal];
        for (int i = 0; i < maxNbDecal; i++)
        {
            GameObject decalGo = Instantiate(planeForDecal);
            decalGo.SetActive(false);
            DecalInstance newDecal = new DecalInstance(decalGo.GetComponent<Renderer>(), decalGo, 0);
            allDecal[i] = newDecal;
        }
    }

    public Transform ProjectDecal(RaycastHit hitBase, string decalName = "", float sizeMultiplier = 1)
    {

        if ((CameraHandler.Instance == null || CameraHandler.Instance.GetDistanceWithCam(hitBase.point) < maxDistToDecal) && allDecal != null) 
        {
            if (!activeDecal)
            {
                Debug.Log("Decal not activated");
                return null;
            }

            //EasyDecal decalInstance = FindDecal(name);

            Material overrideMaterial = null;
            if (decalName != "") overrideMaterial = FindDecalMat(decalName);
            else if (maxDecal == null) return null;
            if (overrideMaterial == null) return null;

            DecalInstance decalUsed = null;
            for (int i = 0; i < maxNbDecal; i++)
            {
                if (allDecal[i] == null || allDecal[i].go == null)
                {
                    GameObject decalGo = Instantiate(planeForDecal);
                    DecalInstance newDecal = new DecalInstance(decalGo.GetComponent<Renderer>(), decalGo, 0);
                    allDecal[i] = newDecal;
                }

                if (!allDecal[i].go.activeSelf)
                {
                    decalUsed = allDecal[i];
                    break;
                }
            }
            if (decalUsed == null)
            {
                int index = Random.Range(0, allDecal.Length);
                //Debug.Log("Override : " + index);
                decalUsed = allDecal[index];
            }

            decalUsed.lifeTime = timeStayNormal + timeFade;
            decalUsed.go.SetActive(true);
            decalUsed.go.transform.position = hitBase.point + hitBase.normal.normalized * safeTranslateValue;
            decalUsed.go.transform.rotation = Quaternion.LookRotation(hitBase.normal * -1);

            //GameObject planeInstance = Instantiate(planeForDecal, hitBase.point + hitBase.normal.normalized * safeTranslateValue, Quaternion.LookRotation(hitBase.normal * -1));
            decalUsed.go.transform.localScale = Vector3.one * scalePlane * sizeMultiplier;
            decalUsed.go.transform.Rotate(Vector3.forward * Random.Range(0, 360), Space.Self);
            decalUsed.go.transform.SetParent(hitBase.collider.transform, true);
            decalUsed.sizeMultiplier = sizeMultiplier;

            //MeshRenderer planeRenderer = planeInstance.GetComponent<MeshRenderer>();
            decalUsed.render.material = overrideMaterial != null ? overrideMaterial : maxDecal;

            //DecalInstance newDecal = new DecalInstance(planeRenderer, planeInstance, timeStayNormal + timeFade);

            //allDecal.Add(newDecal);

            return decalUsed.go.transform;
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

    public void RemoveAllDecal()
    {
        if (allDecal != null)
        {
            for (int i = 0; i < allDecal.Length; i++)
            {
                if (allDecal[i] != null)
                {
                    allDecal[i].go.SetActive(false);
                    allDecal[i].lifeTime = 0;
                }
            }
        }
    }

    private void Update()
    {
        if (allDecal != null)
        {
            for (int i = 0; i < allDecal.Length; i++)
            {
                float currScaleMultiplier = baseAlpha;
                if (allDecal[i].lifeTime > 0 && allDecal[i].go != null && allDecal[i].go.activeSelf)
                {
                    allDecal[i].lifeTime -= Time.deltaTime;
                    if (allDecal[i].lifeTime < 0 || CameraHandler.Instance != null && allDecal[i].go != null && CameraHandler.Instance.GetDistanceWithCam(allDecal[i].go.transform.position) > maxDistToDecal)
                    {
                        allDecal[i].go.SetActive(false);
                        allDecal[i].lifeTime = 0;
                    }
                    else
                    {
                        if (allDecal[i].lifeTime < timeFade) currScaleMultiplier = allDecal[i].lifeTime * baseAlpha / timeFade;
                        if (allDecal[i].go != null)
                            allDecal[i].go.transform.localScale = Vector3.one * scalePlane * currScaleMultiplier * allDecal[i].sizeMultiplier;
                    }
                }
            }
        }
    }


}

public class DecalInstance
{
    public Renderer render = null;
    public GameObject go = null;
    public float lifeTime = 0;
    public float sizeMultiplier = 1;

    public DecalInstance(Renderer _render, GameObject _go, float _lifeTime)
    {
        render = _render;
        go = _go;
        lifeTime = _lifeTime;
    }
}
