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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        InitChilds();

        if (pathTransforms != null)
        {
            Gizmos.color = rayColor;

            for (int i = 0; i < totalPaths; i++)
            {
                Vector3 pos = GetPathAt(i).position;
                if (i > 0)
                {
                    Vector3 previous = GetPathAt(i - 1).position;
                    Gizmos.DrawLine(previous, pos);
                }
                Gizmos.DrawWireSphere(pos, .3f);
            }

        }

    }
#endif

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

    public Transform GetPathAt(int index)
    {
        if (index < totalPaths)
        {
            return pathTransforms[index];
        }
        else
        {
            return null;
        }

    }
}
