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


    [Header("Anim")]
    [SerializeField] DataSimpleAnim animClick = null;
    bool doAnimClicked = false;
    float animClickedPurcentage = 1;
    float currentBaseScale = 1;

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
        if (CheckIfMouseOver())
        {
            img.color = dataLeaderboard.highlightedColorButtons;
            currentBaseScale = Mathf.Lerp(currentBaseScale, dataLeaderboard.scaleWhenMouseOvered, Time.unscaledDeltaTime * dataLeaderboard.scaleLerp);
        }
        else
        {
            img.color = dataLeaderboard.baseColorButtons;
            currentBaseScale = Mathf.Lerp(currentBaseScale, dataLeaderboard.scaleNormal, Time.unscaledDeltaTime * dataLeaderboard.scaleLerp);
        }

        transform.localScale = Vector3.one * currentBaseScale;
        if (doAnimClicked)
        {
            doAnimClicked = !animClick.AddPurcentage(animClickedPurcentage, Time.unscaledDeltaTime, out animClickedPurcentage);
            transform.localScale = Vector3.one * currentBaseScale + Vector3.one * animClick.ValueAt(animClickedPurcentage);
        }
    }

    public void PlayerClicked() 
    {
        if (CheckIfMouseOver())
        {
            ClickedButton();
            doAnimClicked = true;
            animClickedPurcentage = 0;
        }
    }
    void ClickedButton() { manager.changeTitle(changeOnTitle); }
}
