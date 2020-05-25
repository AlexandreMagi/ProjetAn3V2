using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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

    [SerializeField] Image spritePublic = null;
    [SerializeField] Text textPublic = null;
    [SerializeField] float speedGoTo = 5;
    [SerializeField] float speedRecover = 1;
    [SerializeField] float speedIdle = 15;
    [SerializeField] float amplitudeIdle = 0.08f;

    [Title("New Vote")]
    [SerializeField] GameObject root_Vote = null;
    [SerializeField] Image blueBarFill = null;
    [SerializeField, PropertyRange(0.01f,0.499f)] float safeValueFromMiddle = 0.1f;
    [SerializeField, PropertyRange(0.01f,1f)] float graphicPropotionOfVoteFirst = 0.5f;
    [SerializeField, PropertyRange(0.01f,1f)] float graphicPropotionOfVoteLast = 0.5f;
    [SerializeField] UIParticuleSystem positivePs = null;
    [SerializeField] UIParticuleSystem negativePs = null;
    [SerializeField] float globalParticleEmission = 50;

    [SerializeField] float timerGoToFirstValue = 1;
    [SerializeField] float timerGoToLastValue = 2;
    [SerializeField] float timerWaitAtLastValue = 1;
    [SerializeField] float timerWaitDepopFinal = 1.3f;

    [SerializeField] Text lifeText = null;
    [SerializeField] Text deathText = null;

    [SerializeField] float randomFillAddedSpeed = 8;
    [SerializeField, PropertyRange(0.01f, 1f)] float randomFillAddedAmplitude = 0.5f;
    float customTimeForAddedFill = 0;


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
                    spritePublic.color = Color.Lerp(spritePublic.color, Color.red, Time.deltaTime * speedGoTo);
                    textPublic.color = Color.Lerp(spritePublic.color, Color.red, Time.deltaTime * speedGoTo);
                }
                else
                {
                    // Go Up
                    upArrow.SetActive(true);
                    downArrow.SetActive(false);
                    spritePublic.color = Color.Lerp(spritePublic.color, Color.cyan, Time.deltaTime * speedGoTo);
                    textPublic.color = Color.Lerp(spritePublic.color, Color.cyan, Time.deltaTime * speedGoTo);
                }
                spritePublic.transform.localScale = Vector3.Lerp (spritePublic.transform.localScale, Mathf.Sin(Time.time * speedIdle) * amplitudeIdle * Vector3.one + Vector3.one, Time.deltaTime * speedGoTo);
            }
            else
            {
                // Is Stable
                upArrow.SetActive(false);
                downArrow.SetActive(false);
                spritePublic.color = Color.Lerp(spritePublic.color, Color.white, Time.deltaTime * speedRecover);
                textPublic.color = Color.Lerp(spritePublic.color, Color.white, Time.deltaTime * speedRecover);
                spritePublic.transform.localScale = Vector3.Lerp(spritePublic.transform.localScale, Vector3.one, Time.deltaTime * speedRecover);
            }

            ViewerText.fontSize = fontSize;
            if (Weapon.Instance == null || !Weapon.Instance.IsMinigun)
                ViewerText.text = $"{Mathf.RoundToInt(currentViewerFluid).ToString("N0")}";
            else
            {
                if (Mathf.Repeat (Time.time, 1) < 0.75f)
                    ViewerText.text = "KILL";
                else
                    ViewerText.text = "";
            }

            if (Mathf.RoundToInt(currentViewerFluid) == 0)
            {
                //ViewerText.fontSize = 25    ;
                ViewerText.text = "1 (MOM)";
            }


        }
        else ViewerText.text = "NO MANAGER";
    }

    public void PlayerJustDied (bool willRevive, float result, float bonusFromRez, float chanceOfSurvival)
    {
        //StartCoroutine(DeathCoroutine(willRevive, result, bonusFromRez));
        StartCoroutine(NewVoteCoroutine(willRevive, result, bonusFromRez, chanceOfSurvival));
    }

    IEnumerator NewVoteCoroutine(bool revive, float result, float bonusFromRez, float chanceOfSurvival)
    {
        TimeScaleManager.Instance.AddStopTime(60);
        root_Vote.SetActive(true);

        Debug.Log("Chance = " + chanceOfSurvival + " / Result = " + result);

        chanceOfSurvival /= 100;
        result /= 100;
        float firstSpot = (0.5f - graphicPropotionOfVoteFirst/2) + graphicPropotionOfVoteFirst * chanceOfSurvival;
        float lastSpot;
        if (revive)
            lastSpot = 0.5f + safeValueFromMiddle + (graphicPropotionOfVoteLast * (0.5f - safeValueFromMiddle)) * (1 - result / chanceOfSurvival);
        else 
            lastSpot = 0.5f - safeValueFromMiddle - (graphicPropotionOfVoteLast * (0.5f - safeValueFromMiddle)) * ((result - chanceOfSurvival) / (1 - chanceOfSurvival));


        // --- SETUP EMITTER
        positivePs.duration = timerGoToFirstValue + timerGoToLastValue;
        positivePs.Play();
        negativePs.duration = timerGoToFirstValue + timerGoToLastValue;
        negativePs.Play();


        // --- FIRST SPOT
        float timer = timerGoToFirstValue;
        while (timer > 0)
        {
            customTimeForAddedFill += Time.unscaledDeltaTime * randomFillAddedSpeed;
            timer -= Time.unscaledDeltaTime;
            float currFill = Mathf.Lerp(.5f, Mathf.Clamp01(firstSpot), 1 - timer / timerGoToFirstValue);
            blueBarFill.fillAmount = currFill + (Mathf.PerlinNoise(10, customTimeForAddedFill) * 2 - 1) * randomFillAddedAmplitude;
            positivePs.rateOfParticle = globalParticleEmission * Mathf.Clamp01(firstSpot);
            negativePs.rateOfParticle = globalParticleEmission * (1- Mathf.Clamp01(firstSpot));

            lifeText.text = Mathf.RoundToInt(Mathf.Clamp01(currFill + (Mathf.PerlinNoise(10, customTimeForAddedFill) * 2 - 1) * randomFillAddedAmplitude) * 100) + "%";
            deathText.text = Mathf.RoundToInt((1 - Mathf.Clamp01(currFill + (Mathf.PerlinNoise(10, customTimeForAddedFill) * 2 - 1) * randomFillAddedAmplitude)) * 100) + "%";

            yield return new WaitForEndOfFrame();
        }

        // --- LAST SPOT
        timer = timerGoToLastValue;
        while (timer > 0)
        {
            customTimeForAddedFill += Time.unscaledDeltaTime * randomFillAddedSpeed;
            timer -= Time.unscaledDeltaTime;
            float currFill = Mathf.Lerp(Mathf.Clamp01(firstSpot), Mathf.Clamp01(lastSpot), 1 - timer / timerGoToLastValue);
            blueBarFill.fillAmount = currFill + Mathf.Lerp((Mathf.PerlinNoise(10, customTimeForAddedFill) * 2 - 1) * randomFillAddedAmplitude, 0, 1 - timer / timerGoToLastValue);
            positivePs.rateOfParticle = globalParticleEmission * Mathf.Clamp01(lastSpot);
            negativePs.rateOfParticle = globalParticleEmission * (1- Mathf.Clamp01(lastSpot));

            lifeText.text = Mathf.RoundToInt(Mathf.Clamp01(currFill + Mathf.Lerp((Mathf.PerlinNoise(10, customTimeForAddedFill) * 2 - 1) * randomFillAddedAmplitude, 0, 1 - timer / timerGoToLastValue)) * 100) + "%";
            deathText.text = Mathf.RoundToInt((1-Mathf.Clamp01(currFill + Mathf.Lerp((Mathf.PerlinNoise(10, customTimeForAddedFill) * 2 - 1) * randomFillAddedAmplitude, 0, 1 - timer / timerGoToLastValue))) * 100) + "%";

            yield return new WaitForEndOfFrame();
        }

        // --- FONCTIONS RETOUR
        yield return new WaitForSecondsRealtime(timerWaitAtLastValue);
        EndGameChoice.Instance.EndChoiceAnim(revive);
        yield return new WaitForSecondsRealtime(timerWaitDepopFinal);
        if (revive) MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.RevivedByCrowd);
        root_Vote.SetActive(false);
        EndGameChoice.Instance.EndChoice();
        TimeScaleManager.Instance.Stop();
        Main.Instance.EndReviveSituation(revive, bonusFromRez);

        yield break;
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


        if (revive)
        {
            MetricsGestionnary.Instance.EventMetrics(MetricsGestionnary.MetricsEventType.RevivedByCrowd);
        }

        publicBackground.SetActive(false);
        publicSpriteonDeath.SetActive(false);
        voteSlider.gameObject.SetActive(false);
        rootLiveOrDie.gameObject.SetActive(false);
        cap.gameObject.SetActive(false);
        TimeScaleManager.Instance.Stop();
        Main.Instance.EndReviveSituation(revive, bonusFromRez);
        //if (revive) UiLifeBar.Instance.AddLife();
        yield break;
    }


}
