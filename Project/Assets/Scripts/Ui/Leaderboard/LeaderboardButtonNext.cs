using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class LeaderboardButtonNext : MonoBehaviour
{

    RectTransform rect = null;
    [SerializeField] Image img = null;
    [SerializeField] Outline outline = null;

    [Header("Anim")]
    [SerializeField] DataSimpleAnim animClick = null;
    bool doAnimClicked = false;
    float animClickedPurcentage = 1;
    float currentBaseScale = 1;

    [Header("Color and scales")]
    [SerializeField] Color baseColorButton = Color.white;
    [SerializeField] Color highlightedColorButtons = Color.white;
    [SerializeField] Color baseColorOutline = Color.white;
    [SerializeField] Color highlightedColorOutline = Color.white;
    [SerializeField] float scaleLerp = 8;
    [SerializeField] float scaleNormal = 0.8f;
    [SerializeField] float scaleWhenMouseOvered = 1.2f;


    [Header("Idle")]
    [SerializeField] float idleSpeed = 4;
    [SerializeField] float idleSpeedMouseOvered = 6;
    [SerializeField] float idleMagnitude = 0.1f;
    float idlePurcentageApplied = 1f;
    float customTimeIdle = 0;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        img.color = baseColorButton;
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
            img.color = Color.Lerp(img.color, highlightedColorButtons, Time.unscaledDeltaTime * scaleLerp);
            outline.effectColor = Color.Lerp(outline.effectColor, highlightedColorOutline, Time.unscaledDeltaTime * scaleLerp);
            currentBaseScale = Mathf.Lerp(currentBaseScale, scaleWhenMouseOvered, Time.unscaledDeltaTime * scaleLerp);
            customTimeIdle += Time.unscaledDeltaTime * idleSpeedMouseOvered;
        }
        else
        {
            img.color = Color.Lerp(img.color, baseColorButton, Time.unscaledDeltaTime * scaleLerp);
            outline.effectColor = Color.Lerp(outline.effectColor, baseColorOutline, Time.unscaledDeltaTime * scaleLerp);
            currentBaseScale = Mathf.Lerp(currentBaseScale, scaleNormal, Time.unscaledDeltaTime * scaleLerp);
            customTimeIdle += Time.unscaledDeltaTime * idleSpeed;
        }

        float currScale = currentBaseScale;
        if (doAnimClicked)
        {
            doAnimClicked = !animClick.AddPurcentage(animClickedPurcentage, Time.unscaledDeltaTime, out animClickedPurcentage);
            currScale += animClick.ValueAt(animClickedPurcentage);
        }
        img.transform.localScale = Vector3.one * (currScale + Mathf.Lerp(0, Mathf.Sin(customTimeIdle) * idleMagnitude, idlePurcentageApplied));
    }

    public void PlayerClicked()
    {
        Debug.Log(CheckIfMouseOver());
        if (CheckIfMouseOver())
        {
            ClickedButton();
            doAnimClicked = true;
            animClickedPurcentage = 0;
        }
    }

    void ClickedButton() { UILeaderboard.Instance.NextScreen(); }
}
