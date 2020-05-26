using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]//, typeof(Animator))]
public class lastChanceButton : MonoBehaviour
{
    RectTransform rect = null;
    enum typeOfButton { beg, vote};
    [SerializeField]
    typeOfButton buttonType = typeOfButton.beg;

    public static List<lastChanceButton> allButtons;

    [SerializeField] Transform headLine = null;
    [SerializeField] float headLineIdleScale = 0.1f;
    [SerializeField] float headLineIdleScaleSpeed = 0.1f;
    [SerializeField] float headLinerecoverLerp = 8;

    [SerializeField] float idleScale = 0.1f;
    [SerializeField] float idleScaleSpeed = 15f;
    [SerializeField] float recoverLerp = 8;
    float sizeAddedByIdle = 0;

    CanvasGroup cvsGroup = null;
    public CanvasGroup CvsGroup { get { return cvsGroup; } }

    [HideInInspector]
    public float baseScale = 1;

    [SerializeField]
    Transform glow = null;
    [SerializeField] float glowIdleScale = 0.1f;
    [SerializeField] float glowIdleScaleSpeed = 9f;
    [SerializeField] float glowRecoverLerp = 8;
    [SerializeField] float glowGrowLerp = 8;
    float glowScaleValue = 0;

    Animator anmtrButton = null;

    [SerializeField] public bool isMouseOvered = false;

    private void Awake()
    {
        if (allButtons == null) allButtons = new List<lastChanceButton>();
        allButtons.Add(this);
        rect = GetComponent<RectTransform>();
        cvsGroup = GetComponent<CanvasGroup>();
        //anmtrButton = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //anmtrButton.enabled = false;
    }

    private void OnDisable()
    {
        //anmtrButton.enabled = false;
    }


    public void DoAnim (int choiceMade)
    {
        anmtrButton.enabled = true;
        if (choiceMade == 0 || choiceMade == 1)
        {
            if (choiceMade == (int)buttonType)
            {
                anmtrButton.SetTrigger("Validated");
            }
            else
            {
                anmtrButton.SetTrigger("OtherValidated");
            }
        }
        else
        {
            anmtrButton.SetTrigger("NotValidated");
        }
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
                isMouseOvered = true;
                return true;
            }
            else
            {
                isMouseOvered = false;
                return false;
            }
        }
        return false;
    }

    public void AnimateIfMouseOver()
    {
        sizeAddedByIdle = Mathf.Sin(Time.unscaledTime * idleScaleSpeed) * idleScale;
        transform.localScale = Vector3.one * (baseScale + sizeAddedByIdle);
        headLine.transform.localScale = Vector3.one + Vector3.one * Mathf.Sin(Time.unscaledTime * headLineIdleScaleSpeed) * headLineIdleScale;

        glowScaleValue = Mathf.Lerp(glowScaleValue, 1, Time.unscaledDeltaTime * glowGrowLerp);
        glow.transform.localScale = Vector3.one * glowScaleValue + Vector3.one * glowScaleValue * Mathf.Sin(Time.unscaledTime * glowIdleScaleSpeed) * glowIdleScale;
    }

    public void unanimateIfNoMouseOver()
    {
        sizeAddedByIdle = Mathf.Lerp(sizeAddedByIdle, 0, Time.unscaledDeltaTime * recoverLerp);
        transform.localScale = Vector3.one * (baseScale + sizeAddedByIdle);
        headLine.transform.localScale = Vector3.Lerp(headLine.transform.localScale, Vector3.one, Time.unscaledDeltaTime * headLinerecoverLerp);

        glowScaleValue = Mathf.Lerp(glowScaleValue, 0, Time.unscaledDeltaTime * glowRecoverLerp);
        glow.transform.localScale = Vector3.Lerp(glow.transform.localScale, Vector3.zero, Time.unscaledDeltaTime * glowRecoverLerp);
    }

    public int Click()
    {
        if (CheckIfMouseOver())
        {
            CustomSoundManager.Instance.PlaySound("SE_EndGameButtonChoice", "UI", null, 2, false, 1, 0, 0, 3);
            return (int)buttonType;
            //switch (buttonType)
            //{
            //    case typeOfButton.beg:
            //        Main.Instance.ReviveChoice();
            //        break;
            //    case typeOfButton.vote:
            //        Main.Instance.VoteChoice();
            //        break;
            //}
        }
        return -1;
    }

    private void OnDestroy()
    {
        allButtons.Remove(this);
    }

}
