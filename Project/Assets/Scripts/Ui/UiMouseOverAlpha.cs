using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UiMouseOverAlpha : MonoBehaviour
{
    RectTransform rect = null;
    CanvasGroup cvsGroup = null;
    float alphaMouseOver = 0.1f;
    float transitionSpeed = 8;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (cvsGroup == null) cvsGroup = transform.parent.gameObject.AddComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        float alphaGoTo = 1;
        if (CheckIfMouseOver(Main.Instance.GetCursorPos())) alphaGoTo = alphaMouseOver;

        cvsGroup.alpha = Mathf.Lerp(cvsGroup.alpha, alphaGoTo, Time.unscaledDeltaTime * transitionSpeed);

    }

    public bool CheckIfMouseOver(Vector2 mousePosition)
    {
        if (gameObject.activeSelf)
        {
            float distX = rect.sizeDelta.x / 2 * transform.localScale.x;
            float distY = rect.sizeDelta.y / 2 * transform.localScale.y;
            if (mousePosition.x < rect.position.x + + distX && mousePosition.x > rect.position.x  - distX && mousePosition.y < rect.position.y + distY && mousePosition.y > rect.position.y - distY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
}
