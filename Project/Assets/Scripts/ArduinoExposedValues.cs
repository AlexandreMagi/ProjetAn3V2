using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArduinoExposedValues : MonoBehaviour
{
    public static ArduinoExposedValues Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this.gameObject);
    }

    public int n_whatPlayerCanDo = 0;
    public int n_remainingBullet = 0;
    public int n_orbCharge = 0;

    void Update()
    {
        if (Main.Instance != null && Weapon.Instance != null)
        { 
            // --- Setup de n_whatPlayerCanDo
            if (Main.Instance.playerCanReload && Main.Instance.playerCanPerfectReload && Main.Instance.playerCanShoot && Main.Instance.playerCanShotgun && Main.Instance.playerCanOrb) n_whatPlayerCanDo = 3;
            else if (Main.Instance.playerCanReload && Main.Instance.playerCanPerfectReload && Main.Instance.playerCanShoot && Main.Instance.playerCanShotgun) n_whatPlayerCanDo = 2;
            else if (Main.Instance.playerCanReload && Main.Instance.playerCanShoot) n_whatPlayerCanDo = 1;
            else n_whatPlayerCanDo = 0;

            // --- Setup de n_remainingBullet
            if (Weapon.Instance.IsMinigun)
            {
                n_remainingBullet = 9;
            }
            else
            {
                Vector2 bulletAmmounts = Weapon.Instance.GetBulletAmmount();
                bulletAmmounts.y = Weapon.Instance.GetRealMaxBulletAmmount();
                n_remainingBullet = Mathf.RoundToInt(bulletAmmounts.x / bulletAmmounts.y * 9);
            }

            // --- Setup de n_orbCharge
            n_orbCharge = Mathf.RoundToInt(Weapon.Instance.GetOrbValue() * 9);
        }
        else
        {
            n_whatPlayerCanDo = 3;
            n_remainingBullet = 9;
            n_orbCharge = 9;
        }
    }
}
