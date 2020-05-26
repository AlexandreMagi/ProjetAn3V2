using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityHandler : MonoBehaviour
{

    public static QualityHandler Instance { get; private set; }
    void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    int nbQuality = 0;

    public bool isHighQuality = true;

    // Start is called before the first frame update
    void Start()
    {
        string[] names = QualitySettings.names;
        nbQuality = names.Length;

        SetupQuality(isHighQuality);
    }

    public void SetupQuality (bool value) { isHighQuality = value; QualitySettings.SetQualityLevel(value ? nbQuality - 1 : 0, true); }

}
