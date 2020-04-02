using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteScreenEffect : MonoBehaviour
{

    public static WhiteScreenEffect Instance { get; private set; }
    void Awake() { Instance = this; }

    [SerializeField] Image screenEffect = null;
    [SerializeField] float scaleMin = 0;
    [SerializeField] float scaleMax = 6;
    [SerializeField] float timeToScaleMax = 3;
    [SerializeField] float timeStayAtMax = 1;
    [SerializeField] float timeFadeAlpha = 2;
    [SerializeField] bool independentFromTimeScale = false;
    float purcentageAnim = 1;

    public void StartWhiteScreenEffect()
    {
        purcentageAnim = 0;
        screenEffect.transform.localScale = Vector3.one * scaleMin;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.X)) StartWhiteScreenEffect();

        float dt = independentFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        if (purcentageAnim < 1)
        {
            purcentageAnim += dt / (timeToScaleMax + timeStayAtMax + timeFadeAlpha);
            float purcentageInTimeDone = purcentageAnim * (timeToScaleMax + timeStayAtMax + timeFadeAlpha);

            // Scale
            if (purcentageInTimeDone > timeToScaleMax) screenEffect.transform.localScale = Vector3.one * scaleMax;
            else screenEffect.transform.localScale = Vector3.one * Mathf.Lerp(scaleMin, scaleMax, purcentageInTimeDone / timeToScaleMax);

            // Opacity
            if (purcentageInTimeDone < timeToScaleMax + timeStayAtMax) screenEffect.color = Color.white;
            else screenEffect.color = new Color(1, 1, 1, 1 - ((purcentageInTimeDone - timeToScaleMax - timeStayAtMax) / timeFadeAlpha));


            //if (purcentageAnim > 1)
            //{
            //    screenEffect.color = new Color(1, 1, 1, 0);
            //    screenEffect.transform.localScale = Vector3.one * scaleMin;
            //}
        }
    }

}
