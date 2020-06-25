using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WallVignetageTrigger : MonoBehaviour
{

    [SerializeField] Image vignettageImage = null;
    [SerializeField] float timeBeforeStartFade = 5;
    [SerializeField] float timeSafeCheckIfPlayerIn = 2;
    float timeRemaningBeforeStart = 0;
    float timeRemaningBeforeCheck = 0;
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
        if (timeRemaningBeforeStart > 0)
        {
            timeRemaningBeforeStart -= Time.deltaTime;
            if (timeRemaningBeforeStart <= 0)
            {
                timeRemaningBeforeStart = -1;
                playerIn = true;
            }
        }
        if (timeRemaningBeforeCheck > 0)
        {
            timeRemaningBeforeCheck -= Time.deltaTime;
            if (timeRemaningBeforeCheck <= 0)
            {
                timeRemaningBeforeStart = 0;
                timeRemaningBeforeCheck = -1;
                playerIn = false;
            }
        }
        currPurcentageAlpha = Mathf.MoveTowards(currPurcentageAlpha, playerIn ? 1 : 0, Time.deltaTime / (playerIn ? timeToGoToMax : timeToGoToMin));
        vignettageImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, currPurcentageAlpha);
    }

    private void OnDisable()
    {
        if (vignettageImage!=null) vignettageImage.color = new Color(savedColor.r, savedColor.g, savedColor.b, 0);
    }

    private void OnTriggerExit(Collider other) 
    { 
        playerIn = false; 
        timeRemaningBeforeStart = 0;    
    }
    private void OnTriggerStay(Collider other)
    {
        if (timeRemaningBeforeStart == 0)
            timeRemaningBeforeStart = timeBeforeStartFade;
        timeRemaningBeforeCheck = timeSafeCheckIfPlayerIn;
    }

}
