using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class TitleSelecButton : MonoBehaviour
{
    [SerializeField, PropertyRange(-1, 1)] int changeOnTitle = 1;
    [SerializeField] Image img = null;
    [HideInInspector] public TitleSelecHandler manager;

    DataLeaderboardUI dataLeaderboard = null;

    RectTransform rect = null;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        dataLeaderboard = UILeaderboard.Instance.dataLeaderboard;
        img.color = dataLeaderboard.baseColorButtons;
    }

    bool CheckIfMouseOver()
    {
        Vector2 mousePosition = Main.Instance.GetCursorPos();

        if (gameObject.activeSelf)
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

    void Update()
    {
        if (CheckIfMouseOver()) img.color = dataLeaderboard.highlightedColorButtons;
        else img.color = dataLeaderboard.baseColorButtons;
    }

    public void PlayerClicked() { if (CheckIfMouseOver()) ClickedButton(); }
    void ClickedButton() { manager.changeTitle(changeOnTitle); }
}
