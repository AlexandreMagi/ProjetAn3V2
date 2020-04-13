using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteScreenEffect : MonoBehaviour
{

    public static WhiteScreenEffect Instance { get; private set; }
    void Awake() { Instance = this; }

    [SerializeField] Image screenEffect = null;
    float scaleMin = 0;
    float scaleMax = 6;
    float timeToScaleMax = 3;
    float timeStayAtMax = 1;
    float timeFadeAlpha = 2;
    bool independentFromTimeScale = false;
    float purcentageAnim = 1;

    public void StartWhiteScreenEffect(float _scaleMin = 0, float _scaleMax = 6, float _timeToScaleMax = 3, float _timeStayAtMax = 1, float _timeFadeAlpha = 2, bool _independentFromTimeScale = false)
    {
        purcentageAnim = 0;
        scaleMin = _scaleMin;
        scaleMax = _scaleMax;
        timeToScaleMax = _timeToScaleMax;
        timeStayAtMax = _timeStayAtMax;
        timeFadeAlpha = _timeFadeAlpha;
        independentFromTimeScale = _independentFromTimeScale;

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
