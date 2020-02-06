using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiViewer : MonoBehaviour
{

    int fontSize = 0;

    void Awake()
    {
        Instance = this;
    }
    public static UiViewer Instance { get; private set; }

    [SerializeField] Text ViewerText = null;
    float currentViewerFluid = 0;
    [SerializeField] float viewerSpeedDisplay = 5;
    [SerializeField] float considerDifference = 50;
    [SerializeField] GameObject upArrow = null;
    [SerializeField] GameObject downArrow = null;

    [SerializeField] Slider voteSlider = null;
    [SerializeField] GameObject publicBackground = null;
    [SerializeField] GameObject publicSpriteonDeath = null;
    [SerializeField] GameObject rootLiveOrDie = null;

    [SerializeField] GameObject cap = null;

    [SerializeField] AnimationCurve animOnBar = null;

    private void Start()
    {
        fontSize = ViewerText.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (PublicManager.Instance != null)
        {
            int actualViewer = PublicManager.Instance.GetNbViewers();
            currentViewerFluid = Mathf.Lerp(currentViewerFluid, actualViewer, Time.deltaTime * viewerSpeedDisplay);

            if (Mathf.Abs(currentViewerFluid - actualViewer) > considerDifference)
            {
                if (currentViewerFluid > actualViewer)
                {
                    // Go Down
                    upArrow.SetActive(false);
                    downArrow.SetActive(true);
                }
                else
                {
                    // Go Up
                    upArrow.SetActive(true);
                    downArrow.SetActive(false);
                }
            }
            else
            {
                // Is Stable
                upArrow.SetActive(false);
                downArrow.SetActive(false);
            }

            ViewerText.fontSize = fontSize;
            ViewerText.text = $"{Mathf.RoundToInt(currentViewerFluid)}";
            if (Mathf.RoundToInt(currentViewerFluid) == 0)
            {
                ViewerText.fontSize = 25    ;
                ViewerText.text = "1 (Your mom)";
            }


        }
        else ViewerText.text = "NO MANAGER";
    }

    public void PlayerJustDied (bool willRevive, float result, float bonusFromRez)
    {
        StartCoroutine(DeathCoroutine(willRevive, result, bonusFromRez));
    }

    IEnumerator DeathCoroutine(bool revive, float result, float bonusFromRez)
    {
        TimeScaleManager.Instance.AddStopTime(60);
        publicBackground.SetActive(true);
        publicSpriteonDeath.SetActive(true);
        voteSlider.gameObject.SetActive(true);
        rootLiveOrDie.gameObject.SetActive(true);
        cap.gameObject.SetActive(true);

        if (revive) rootLiveOrDie.GetComponent<Animator>().SetTrigger("live");
        else rootLiveOrDie.GetComponent<Animator>().SetTrigger("die");

        RectTransform voteSliderRectTransform = voteSlider.GetComponent<RectTransform>();
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, new Vector2 (voteSliderRectTransform.position.x, (voteSliderRectTransform.position.y - voteSliderRectTransform.sizeDelta.x /2) + voteSliderRectTransform.sizeDelta.x * (result/100)), this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
        cap.transform.position = transform.TransformPoint(pos);

        float timer = 4;
        float timerAlpha = 1;
        while (timer > 0)
        {
            float valueAlpha = 0.9f;
            if (timerAlpha > 0) timerAlpha -= Time.unscaledDeltaTime / 0.3f;
            else timerAlpha = 0;

            publicBackground.GetComponent<Image>().color = new Color(0, 0, 0, (1 - timerAlpha) * valueAlpha);

            voteSlider.value = Main.Instance.GetCurrentChacesOfSurvival() / 100;// + animOnBar.Evaluate (1 - ((timer/2) /2)) / 8;

            float value = voteSliderRectTransform.sizeDelta.x * (result / 100) + animOnBar.Evaluate(1 - (timer / 4))/4 * (voteSliderRectTransform.sizeDelta.x);
            value = Mathf.Clamp(value, 0, voteSliderRectTransform.sizeDelta.x);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, new Vector2(voteSliderRectTransform.position.x, (voteSliderRectTransform.position.y - voteSliderRectTransform.sizeDelta.x / 2) + value), this.gameObject.GetComponent<Canvas>().worldCamera, out pos);
            cap.transform.position = transform.TransformPoint(pos);

            timer -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        //yield return new WaitForSecondsRealtime(4);

        


        publicBackground.SetActive(false);
        publicSpriteonDeath.SetActive(false);
        voteSlider.gameObject.SetActive(false);
        rootLiveOrDie.gameObject.SetActive(false);
        cap.gameObject.SetActive(false);
        TimeScaleManager.Instance.Stop();
        Main.Instance.EndReviveSituation(revive, bonusFromRez);
        if (revive) UiLifeBar.Instance.AddLife();
        yield break;
    }


}
