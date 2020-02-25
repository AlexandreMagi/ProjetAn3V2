using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class ButtonMenuScript : MonoBehaviour
{
    RectTransform rect;

    enum typeButton { quit, settings, play }
    [SerializeField]
    typeButton typeOfThisButton = typeButton.play;

    [SerializeField]
    bool move = false;
    [SerializeField]
    string triggerToPop = "";
    [SerializeField, ShowIf("move")]
    float speed = 5;
    Vector2 currentSpeed = Vector2.zero;
    Vector3 basePos = Vector3.zero;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
    private void Start()
    {
        if (MenuMain.Instance != null) MenuMain.Instance.buttonMenuScripts.Add(this);
        currentSpeed = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * speed;
        basePos = rect.position;
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

    public void UpdatePos (bool mustMove)
    {
        if (move && mustMove)
        {
            float halfX = rect.sizeDelta.x / 2;
            float distX = halfX * transform.localScale.x;
            float halfY = rect.sizeDelta.y / 2;
            float distY = halfY * transform.localScale.y;


            transform.Translate(currentSpeed * Time.unscaledDeltaTime, Space.World);

            float distWithRight = Screen.width - (rect.position.x + halfX + distX);
            if (distWithRight < 0)
            {
                currentSpeed.x *= -1;
                transform.Translate(distWithRight,0,0,Space.World);
            }
            float distWithLeft = (rect.position.x);
            if (distWithLeft < 0)
            {
                currentSpeed.x *= -1;
                transform.Translate(-distWithLeft, 0, 0, Space.World);
            }

            float distWithTop = Screen.height - (rect.position.y + distY);
            if (distWithTop < 0)
            {
                currentSpeed.y *= -1;
                transform.Translate(0, distWithTop, 0,Space.World);
            }

            float distWithBottom = (rect.position.y - distY);
            if (distWithBottom < 0)
            {
                currentSpeed.y *= -1;
                transform.Translate(0, -distWithBottom, 0, Space.World);
            }
        }
        else
        {
            rect.position = basePos;
        }
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
                    if (triggerToPop != "" && GetComponent<Animator>()) GetComponent<Animator>().SetTrigger(Animator.StringToHash(triggerToPop));
                    MenuMain.Instance.GoToGame();
                    break;
                default:
                    break;
            }
        }
    }

}
