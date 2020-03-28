using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class LeaderboardButtonChar : MonoBehaviour
{
    [SerializeField, PropertyRange(-26,26)] int changeOnChar = 1;
    [HideInInspector] public CharSelect manager;

    DataLeaderboardUI dataLeaderboard = null;

    RectTransform rect = null;
    Image img = null;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        img = GetComponent<Image>();
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
    void ClickedButton() { manager.changeChar(changeOnChar); }
}
