using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class LeaderboardButtonChar : MonoBehaviour
{
    [SerializeField, PropertyRange(-26,26)] int changeOnChar = 1;
    [HideInInspector] public CharSelect manager;

    DataLeaderboardUI dataLeaderboard = null;

    RectTransform rect = null;
    [SerializeField] Image img = null;

    [SerializeField] float localAlphaMultiplier = 1;
    [SerializeField] float localAlphaMultiplierHighlight = 1;

    [Header("Anim")]
    [SerializeField] DataSimpleAnim animClick = null;
    bool doAnimClicked = false;
    float animClickedPurcentage = 1;
    float currentBaseScale = 1;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        //img = GetComponent<Image>();
        dataLeaderboard = UILeaderboard.Instance.dataLeaderboard;
        img.color = dataLeaderboard.baseColorButtons;
    }

    bool CheckIfMouseOver()
    {
        Vector2 mousePosition = Main.Instance.GetCursorPos();

        if (gameObject.activeSelf && rect != null && UILeaderboard.Instance.CurrentScreen == UILeaderboard.leaderboardScreens.nameAndTitleChoice)
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
            img.color = new Color(dataLeaderboard.highlightedColorButtons.r, dataLeaderboard.highlightedColorButtons.g, dataLeaderboard.highlightedColorButtons.b, dataLeaderboard.highlightedColorButtons.a * localAlphaMultiplierHighlight);
            currentBaseScale = Mathf.Lerp(currentBaseScale, dataLeaderboard.scaleWhenMouseOvered, Time.unscaledDeltaTime * dataLeaderboard.scaleLerp);
        }
        else
        {
            img.color = new Color(dataLeaderboard.baseColorButtons.r, dataLeaderboard.baseColorButtons.g, dataLeaderboard.baseColorButtons.b, dataLeaderboard.baseColorButtons.a * localAlphaMultiplier);
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
            Invoke("PlaySound", 0.1f);
        }
    }

    void PlaySound()
    {
        CustomSoundManager.Instance.PlaySound("Se_CharButton", "Leaderboard", 1);
    }

    void ClickedButton() { manager.changeChar(changeOnChar); }
}
