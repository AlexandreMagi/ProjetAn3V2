using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class lastChanceButton : MonoBehaviour
{
    RectTransform rect = null;
    enum typeOfButton { beg, vote};
    [SerializeField]
    typeOfButton buttonType = typeOfButton.beg;

    public static List<lastChanceButton> allButtons;

    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
        if (allButtons == null) allButtons = new List<lastChanceButton>();
        allButtons.Add(this);
    }

    public bool CheckIfMouseOver()
    {
        Vector2 mousePosition = Main.Instance.GetCursorPos();

        if (gameObject.activeSelf)
        {
            float distX = rect.sizeDelta.x / 2 * transform.localScale.x;
            float distY = rect.sizeDelta.y / 2 * transform.localScale.y;
            if (mousePosition.x < rect.position.x + distX && mousePosition.x > rect.position.x  - distX && mousePosition.y < rect.position.y + distY && mousePosition.y > rect.position.y - distY)
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

    public void Click()
    {
        if (CheckIfMouseOver())
        {
            switch (buttonType)
            {
                case typeOfButton.beg:
                    Main.Instance.ReviveChoice();
                    break;
                case typeOfButton.vote:
                    Main.Instance.VoteChoice();
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        allButtons.Remove(this);
    }

}
