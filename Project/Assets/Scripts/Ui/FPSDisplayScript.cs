using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplayScript : MonoBehaviour
{

    [SerializeField] Text FpsText = null;
    [SerializeField] bool activated = false;

    private void Start()
    {
        if (!activated) FpsText.text = "";
    }

    void Update()
    {
        if (Time.frameCount % 5 == 0 && FpsText!=null && activated)
        {   
            FpsText.text = Mathf.RoundToInt(1/Time.unscaledDeltaTime).ToString();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            activated = !activated;
            if (!activated) FpsText.text = "";
        }

    }
}
