using UnityEngine;
using UnityEngine.UI;

public class FastForwardButton : MonoBehaviour
{

    [SerializeField] RectTransform rect = null;
    [SerializeField] Animator anmtr = null;
    [SerializeField] Image imgToFill = null;
    bool poped = false;
    public static FastForwardButton Instance { get; private set; }
    void Awake() { Instance = this; }

    float purcentageHold = 0;

    /*
    void Update()
    {
        if (poped) anmtr.SetBool("mouseOvered", CheckIfMouseOver());
    }

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
            return true;
        }
        return false;
    }*/
    public void InputUnHold(float timeNecessary)
    {
        if (poped)
        {
            purcentageHold -= Time.unscaledDeltaTime / timeNecessary;
            purcentageHold = Mathf.Clamp01(purcentageHold);
            imgToFill.fillAmount = Mathf.Clamp01(purcentageHold);
        }
    }
    public bool InputHold(float timeNecessary)
    {
        if (poped)
        {
            purcentageHold += Time.unscaledDeltaTime / timeNecessary;
            imgToFill.fillAmount = Mathf.Clamp01(purcentageHold);
        }
        return purcentageHold > 1;
    }

    public void Pop()
    {
        if (!poped)
        {
            anmtr.SetTrigger("pop");
            poped = true;
            purcentageHold = 0;
            imgToFill.fillAmount = Mathf.Clamp01(purcentageHold);
        }
    }

    public void Depop() 
    {
        if (poped)
        {
            anmtr.SetTrigger("depop");
            poped = false;
            purcentageHold = 0;
            imgToFill.fillAmount = Mathf.Clamp01(purcentageHold);
        }
    }

}
