using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLifeBar : MonoBehaviour
{
    private static UiLifeBar _instance;
    public static UiLifeBar Instance
    {
        get
        {
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }

    [SerializeField] DataLifeBarUi dataLifebar = null;

    [SerializeField] Transform rootVerticalShield = null;
    [SerializeField] Transform rootMiddleShield = null;

    [SerializeField] GameObject[] lifeCapsules = new GameObject[0];
    [SerializeField] GameObject gameOverRoot = null;

    float stockArmor = 300;
    float stockMaxArmor = 300;
    float stockLife = 300;
    float stockMaxLife = 300;

    private void Start()
    {
        stockArmor = Player.Instance.GetBaseValues().x;
        stockMaxArmor = stockArmor;
        stockLife = Player.Instance.GetBaseValues().y;
        stockMaxLife = stockLife;
    }

    public void PlayerTookDamage(float armor, float life)
    {
        if (armor < stockArmor)
        {
            float damageValue = stockArmor - armor;

            if (damageValue > 0)
            {
                if (armor > 0)
                {
                    Debug.Log("Armure prend dégats");
                }
                else
                {
                    Debug.Log("Armure casse");
                }
                UpdateArmor(armor);
            }

            stockArmor = armor;

        }
            
        if (life < stockLife)
        {
            UpdateCapsules(life);
            stockLife = life;
        }
    }

    public void UpdateCapsules(float life)
    {
        for (int i = 0; i < lifeCapsules.Length; i++)
        {
            lifeCapsules[i].SetActive(i < life / stockMaxLife * lifeCapsules.Length);
        }
    }
    public void UpdateArmor(float armor)
    {
        rootVerticalShield.transform.localScale = new Vector3(1, armor / stockMaxArmor, 1);
    }
    public void EndGame()
    {
        gameOverRoot.SetActive(true);
    }

}
