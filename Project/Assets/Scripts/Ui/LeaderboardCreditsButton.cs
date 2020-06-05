using Newtonsoft.Json.Converters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardCreditsButton : MonoBehaviour
{

    [SerializeField] RectTransform rect = null;

    public bool CheckIfMouseOver()
    {
        Vector2 mousePosition = MenuMain.Instance.GetCursorPos();

        if (gameObject.activeSelf && rect != null)
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
        return false;
    }

    public bool Click()
    {
        if (CheckIfMouseOver())
        {
            MenuMain.Instance.ButtonLeaderboardCredits();
            return true;
        }
        return false;
    }

}
