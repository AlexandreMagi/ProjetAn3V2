using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LightHandler : MonoBehaviour
{
    Light objLight = null;
    float baseIntensity = 0;

    [SerializeField] bool blink = false;
    [SerializeField, ShowIf("blink")] Vector2 offRandomTime = new Vector2(0, 1);
    [SerializeField, ShowIf("blink")] Vector2 onRandomTime = new Vector2(0, 1);
    float blinkTimer = 0;
    float currentTimerToNextState = 0;
    // on pourrait faire un changement de couleur à chaque blink si necessaire

    [SerializeField] bool oscillate = false;
    [SerializeField, ShowIf("oscillate")] float frequencyIntensity = 1;
    [SerializeField, ShowIf("oscillate")] float minValueIntensityRelative = -1;
    [SerializeField, ShowIf("oscillate")] float maxValueIntensityRelative = 1;

    [SerializeField] bool changeColor = false;
    enum modOfColor { RandomBetweenColor, OscillateBetweenTwoColor, Gradient, RandomBetweenGradient };
    [SerializeField, ShowIf("changeColor")] modOfColor currentMod = 0;
    float colorTimer = 0;
    float currentTimerToNextColor = 0;

    [SerializeField, ShowIf("currentMod", modOfColor.RandomBetweenColor), ShowIf("changeColor")] Vector2 changeEvery = new Vector2(0, 1);
    [SerializeField, ShowIf("currentMod", modOfColor.RandomBetweenColor), ShowIf("changeColor")] Color colorRandOne = Color.white;
    [SerializeField, ShowIf("currentMod", modOfColor.RandomBetweenColor), ShowIf("changeColor")] Color colorRandTwo = Color.white;

    [SerializeField, ShowIf("currentMod", modOfColor.OscillateBetweenTwoColor), ShowIf("changeColor")] float frequencyColor = 1;
    [SerializeField, ShowIf("currentMod", modOfColor.OscillateBetweenTwoColor), ShowIf("changeColor")] Color colorOsciOne = Color.white;
    [SerializeField, ShowIf("currentMod", modOfColor.OscillateBetweenTwoColor), ShowIf("changeColor")] Color colorOsciTwo = Color.white;



    // Start is called before the first frame update
    void Start()
    {
        objLight = GetComponent<Light>();
        baseIntensity = objLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (blink)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer > currentTimerToNextState)
            {
                // Ici pour play Fx
                blinkTimer -= currentTimerToNextState;
                if (objLight.enabled)
                {
                    currentTimerToNextState = Random.Range(offRandomTime.x, offRandomTime.y);
                    objLight.enabled = false;
                }
                else
                {
                    currentTimerToNextState = Random.Range(onRandomTime.x, onRandomTime.y);
                    objLight.enabled = true;
                }
            }
        }
        if (oscillate)
        {
            objLight.intensity = baseIntensity + Mathf.Lerp(minValueIntensityRelative, maxValueIntensityRelative, (Mathf.Sin(Time.time * frequencyIntensity) + 1) / 2);
        }
        if (changeColor)
        {
            if (currentMod == modOfColor.RandomBetweenColor)
            {
                colorTimer += Time.deltaTime;
                if (colorTimer > currentTimerToNextColor)
                {
                    colorTimer -= currentTimerToNextColor;
                    currentTimerToNextColor = Random.Range(changeEvery.x, changeEvery.y);
                    objLight.color = new Color(
                        Random.Range(colorRandOne.r, colorRandTwo.r),
                        Random.Range(colorRandOne.g, colorRandTwo.g),
                        Random.Range(colorRandOne.b, colorRandTwo.b),
                        Random.Range(colorRandOne.a, colorRandTwo.a));
                }
            }
            else if (currentMod == modOfColor.OscillateBetweenTwoColor)
            {
                objLight.color = Color.Lerp(colorOsciOne, colorOsciTwo, (Mathf.Sin(Time.time * frequencyColor) + 1) / 2);
            }
        }
    }
}
