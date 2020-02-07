using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonMenuScript : MonoBehaviour
{
    RectTransform rect;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        if (MenuMain.Instance != null) MenuMain.Instance.buttonMenuScripts.Add(this);
    }

    public bool CheckIfMouseOver(Vector2 mousePosition)
    {
        float distX = rect.sizeDelta.x / 2 * transform.localScale.x;
        float distY = rect.sizeDelta.y / 2 * transform.localScale.y;
        if (mousePosition.x < rect.position.x + distX && mousePosition.x > rect.position.x - distX && mousePosition.y < rect.position.y + distY && mousePosition.y > rect.position.y - distY) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Click(Vector2 mousePosition)
    {
        if (CheckIfMouseOver(mousePosition))
        {
            transform.localScale = Vector3.zero;
        }
    }

}
