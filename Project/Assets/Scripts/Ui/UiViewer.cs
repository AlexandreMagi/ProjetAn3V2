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
    [SerializeField] GameObject live = null;
    [SerializeField] GameObject die = null;
    [SerializeField] GameObject thumb = null;

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

            voteSlider.value = Main.Instance.GetCurrentChacesOfSurvival() / 100;

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
        thumb.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2);

        if (revive)
        {
            live.SetActive(true);
            //thumb.GetComponent<Animator>().SetTrigger("up");
        }
        else
        {
            die.SetActive(true);
            //thumb.GetComponent<Animator>().SetTrigger("down");
        }

        float timerUntilReviveOrDeath = 2;
        while (timerUntilReviveOrDeath > 0)
        {
            float speed = 15;
            float amplitude = 1;
            float scaleValue = 1 + Mathf.Sin(Time.unscaledTime * speed) * 0.2f;
            live.transform.localScale = Vector3.one * scaleValue;
            die.transform.localScale = Vector3.one * scaleValue;
            timerUntilReviveOrDeath -= Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }


        publicBackground.SetActive(false);
        publicSpriteonDeath.SetActive(false);
        voteSlider.gameObject.SetActive(false);
        thumb.gameObject.SetActive(false);
        Main.Instance.EndReviveSituation(revive, bonusFromRez);
        live.SetActive(false);
        die.SetActive(false);
        TimeScaleManager.Instance.Stop();
        yield break;
    }


}
