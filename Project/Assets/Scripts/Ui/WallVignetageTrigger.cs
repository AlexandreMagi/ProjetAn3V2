using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallVignetageTrigger : MonoBehaviour
{

    [SerializeField] Image vignettageImage = null;
    [SerializeField] float timeToGoToMax = 8; 
    [SerializeField] float timeToGoToMin = 2; 
    Color savedColor = Color.red;
    float currPurcentageAlpha = 0;
    bool playerIn = false;

    void Start()
    {
        if (vignettageImage != null)
        {
            vignettageImage.enabled = true;
            savedColor = vignettageImage.color;
            vignettageImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 0);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        currPurcentageAlpha = Mathf.MoveTowards(currPurcentageAlpha, playerIn ? 1 : 0, Time.deltaTime / (playerIn ? timeToGoToMax : timeToGoToMin));
        vignettageImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, currPurcentageAlpha);
    }

    private void OnDisable()
    {
        vignettageImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 0);
    }

    private void OnTriggerExit(Collider other) { playerIn = false; }
    private void OnTriggerEnter(Collider other) { playerIn = true; }

}
