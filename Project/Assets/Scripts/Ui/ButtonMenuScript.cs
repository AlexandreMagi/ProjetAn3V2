using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonMenuScript : MonoBehaviour
{
    RectTransform rect;

    enum typeButton { quit, settings, play }
    [SerializeField]
    typeButton typeOfThisButton = typeButton.play;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    private void Start()
    {
        if (MenuMain.Instance != null) MenuMain.Instance.buttonMenuScripts.Add(this);
    }

    public bool CheckIfMouseOver(Vector2 mousePosition)
    {
        if (gameObject.activeSelf)
        {
            float distX = rect.sizeDelta.x / 2 * transform.localScale.x;
            float distY = rect.sizeDelta.y / 2 * transform.localScale.y;
            if (mousePosition.x < rect.position.x + (rect.sizeDelta.x / 2) + distX && mousePosition.x > rect.position.x + (rect.sizeDelta.x / 2) - distX && mousePosition.y < rect.position.y + distY && mousePosition.y > rect.position.y - distY)
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

    public void Click(Vector2 mousePosition)
    {
        if (CheckIfMouseOver(mousePosition))
        {
            switch (typeOfThisButton)
            {
                case typeButton.quit:
                    MenuMain.Instance.QuitAppli();
                    break;
                case typeButton.settings:
                    break;
                case typeButton.play:
                    MenuMain.Instance.GoToGame();
                    break;
                default:
                    break;
            }
        }
    }

}
