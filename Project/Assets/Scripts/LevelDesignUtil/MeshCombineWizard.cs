using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if (UNITY_EDITOR)
public class MeshCombineWizard : ScriptableWizard {

    [Header("GameObject parent of all mesh to combine")]
    public GameObject parentOfObjectsToCombine = null;

    [Header ("Path to save mesh combined")]
    public string meshPath = "Assets/AssetDA/CombinedMeshs/LD_04/Meshs/CombinedMeshes_";

    string meshPathComp;

    [Header("Path to save the new prefab")]
    public string prefabPath = "Assets/AssetDA/CombinedMeshs/LD_04/Prefabs/";

    string prefabPathComp;

    [Header("Layer combined mesh will be")]
    public int layerChoose = 9;

    [Header("Combined object is static or not")]
    public bool isStatic = true;

    [Header("Add box collider")]
    public bool boxCollider = false;

    [Header("Add mesh collider")]
    public bool meshCollider = false;

    [MenuItem("Combine tool/Mesh Combine")]
    static void CreateWizard() {
        ScriptableWizard.DisplayWizard<MeshCombineWizard>("Mesh Combine Wizard");
    }

    void OnWizardCreate() {
        if(parentOfObjectsToCombine == null) return;
        if(meshPath == null) return;
        if(prefabPath == null) return;
        if (meshCollider && boxCollider)
        {
            Debug.LogError("Combined objects cannot have mesh collider AND box collider");
            return;
        }

        Vector3 originalPosition = parentOfObjectsToCombine.transform.position;
        parentOfObjectsToCombine.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = parentOfObjectsToCombine.GetComponentsInChildren<MeshFilter>();
        Dictionary<Material, List<MeshFilter>> materialToMeshFilterList = new Dictionary<Material, List<MeshFilter>>();
        List<GameObject> combinedObjects = new List<GameObject>();

        for(int i = 0; i < meshFilters.Length; i++) {
            var materials = meshFilters[i].GetComponent<MeshRenderer>().sharedMaterials;
            if(materials == null) continue;
            if(materials.Length > 1) {
                parentOfObjectsToCombine.transform.position = originalPosition;
                Debug.LogError("Objects with multiple materials on the same mesh are not supported. Create multiple meshes from this object's sub-meshes in an external 3D tool and assign separate materials to each.");
                return;
            }
            var material = materials[0];
            if(materialToMeshFilterList.ContainsKey(material)) materialToMeshFilterList[material].Add(meshFilters[i]);
            else materialToMeshFilterList.Add(material, new List<MeshFilter>() { meshFilters[i] });
        }

        foreach(var entry in materialToMeshFilterList) {
            List<MeshFilter> meshesWithSameMaterial = entry.Value;
            string materialName = entry.Key.ToString().Split(' ')[0];

            CombineInstance[] combine = new CombineInstance[meshesWithSameMaterial.Count];
            for(int i = 0; i < meshesWithSameMaterial.Count; i++) {
                combine[i].mesh = meshesWithSameMaterial[i].sharedMesh;
                combine[i].transform = meshesWithSameMaterial[i].transform.localToWorldMatrix;
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.CombineMeshes(combine);
            materialName += "_" + combinedMesh.GetInstanceID();

            Unwrapping.GenerateSecondaryUVSet(combinedMesh);

            AssetDatabase.CreateAsset(combinedMesh, meshPath + materialName + ".asset");

            string goName = (materialToMeshFilterList.Count > 1)? "CombinedMeshes_" + materialName : parentOfObjectsToCombine.name;
            GameObject combinedObject = new GameObject(goName);
            var filter = combinedObject.AddComponent<MeshFilter>();
            filter.sharedMesh = combinedMesh;
            var renderer = combinedObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = entry.Key;
            combinedObjects.Add(combinedObject);
        }

        GameObject resultGO = null;
        if(combinedObjects.Count > 1) {
            resultGO = new GameObject(parentOfObjectsToCombine.name);
            foreach(var combinedObject in combinedObjects) combinedObject.transform.parent = resultGO.transform;
        } else {
            resultGO = combinedObjects[0];
        }

        resultGO.gameObject.isStatic = isStatic;

        resultGO.gameObject.layer = layerChoose;

        if (meshCollider)
            resultGO.gameObject.AddComponent<MeshCollider>();
        else if (boxCollider)
            resultGO.gameObject.AddComponent<BoxCollider>();

        PrefabUtility.SaveAsPrefabAssetAndConnect(resultGO, prefabPath + resultGO.name + ".prefab", InteractionMode.AutomatedAction);

        parentOfObjectsToCombine.SetActive(false);
        parentOfObjectsToCombine.transform.position = originalPosition;
        resultGO.transform.position = originalPosition;
    }
}
#endif