using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoDetectionOrb : MonoBehaviour
{

    [SerializeField]
    GameObject pipeLine = null;

    [SerializeField]
    Material newMatPipe = null;

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            MeshRenderer[] matList = pipeLine.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mat in matList)
            {
                if (mat != null)
                {
                    mat.material = newMatPipe;
                }

            }
        }
    }
}
