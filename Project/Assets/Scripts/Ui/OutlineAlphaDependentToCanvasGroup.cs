using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineAlphaDependentToCanvasGroup : MonoBehaviour
{

    Outline[] allOutline = null;
    Color[] allOutlineColor = null;
    [SerializeField] CanvasGroup  cvsGroupUsed = null;
    [SerializeField] float pow = 4;

    void Start()
    {
        allOutline = GetComponentsInChildren<Outline>();
        allOutlineColor = new Color[allOutline.Length];
        for (int i = 0; i < allOutline.Length; i++)
        {
            if (allOutline[i] != null) allOutlineColor[i] = allOutline[i].effectColor;
        }
        UpdateOutlineColor();
    }

    void UpdateOutlineColor()
    {
        if (cvsGroupUsed != null)
        {
            for (int i = 0; i < allOutline.Length; i++)
            {
                if (allOutline[i] != null && allOutlineColor[i] != null)
                {
                    allOutline[i].effectColor = new Color(allOutlineColor[i].r, allOutlineColor[i].g, allOutlineColor[i].b, allOutlineColor[i].a * Mathf.Pow(cvsGroupUsed.alpha, pow));
                }
            }
        }
    }

    void Update()
    {
        UpdateOutlineColor();
    }
}
