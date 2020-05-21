using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplayScript : MonoBehaviour
{

    [SerializeField] Text FpsText = null;

    void Update()
    {
        if (Time.frameCount % 5 == 0 && FpsText!=null)
        {   
            FpsText.text = Mathf.RoundToInt(1/Time.unscaledDeltaTime).ToString();
        }

    }
}
