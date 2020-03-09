using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pather : MonoBehaviour
{
    [SerializeField]
    Color rayColor = Color.white;

    Transform[] pathTransforms = null;

    int totalPaths = 0;

    public void Awake()
    {
        InitChilds();
    }

    private void OnDrawGizmos()
    {
        InitChilds();

        if (pathTransforms != null)
        {
            Gizmos.color = rayColor;

            for (int i = 0; i < totalPaths; i++)
            {
                Vector3 pos = GetPathAt(i);
                if (i > 0)
                {
                    Vector3 previous = GetPathAt(i - 1);
                    Gizmos.DrawLine(previous, pos);
                }
                Gizmos.DrawWireSphere(pos, .3f);
            }

        }

    }


    void InitChilds()
    {
        if (totalPaths != this.transform.childCount)
        {
            totalPaths = this.transform.childCount;

            Transform[] tempPathTransforms = new Transform[totalPaths];

            for (int i = 0; i < this.transform.childCount; i++)
            {
                tempPathTransforms[i] = this.transform.GetChild(i);
            }

            pathTransforms = tempPathTransforms;
        }

    }

    public Vector3 GetPathAt(int index)
    {
        if (index < totalPaths)
        {
            //Debug.Log($"Returned position at : {pathTransforms[index].position}");
            return pathTransforms[index].position;
        }
        else
        {
            return Vector3.zero;
        }

    }
}
