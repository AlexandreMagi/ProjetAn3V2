using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRightFade : MonoBehaviour
{
    [SerializeField] float timeBeforeArrowAppears = 5;
    float timeRemainingBeforeArrowAppears = 0;
    [SerializeField] float timeToAppear = 3;
    [SerializeField] float timeToDisappear = 0.2f;
    [SerializeField] float idleScaleMagnitude = 0.1f;
    [SerializeField] float idleScaleSpeed = 1f;
    [SerializeField] Image img = null;
    
    float timeBeforeCanAppear = 0;
    float currScale = 0;

    float randomSeedToTimeIdle = 0;

    public bool canAppear = false;

    void Start()
    {
        randomSeedToTimeIdle = Random.Range(0, 8000);
        ChangeOfScreen(18);
    }

    void Update()
    {
        if (canAppear && !UILeaderboard.Instance.cvsVars.nextButton.GetIfMouseOverForced())
        {
            timeRemainingBeforeArrowAppears -= Time.unscaledDeltaTime;
            if (timeRemainingBeforeArrowAppears < 0) UILeaderboard.Instance.cvsVars.nextButton.ForceAppeareance();
        }
        else timeRemainingBeforeArrowAppears = timeBeforeArrowAppears;

        if (timeBeforeCanAppear > 0)
        {
            timeBeforeCanAppear -= Time.unscaledDeltaTime;
            if (timeBeforeCanAppear < 0)
            {
                timeBeforeCanAppear = 0;
                canAppear = true;
                UILeaderboard.Instance.cvsVars.fadeParticle.Resume();
            }
        }

        if (canAppear)
        {
            currScale = Mathf.MoveTowards(currScale, 1, Time.unscaledDeltaTime / timeToAppear);
        }
        else
        {
            currScale = Mathf.MoveTowards(currScale, 0, Time.unscaledDeltaTime / timeToDisappear);
        }
        img.transform.localScale = new Vector3(currScale + Mathf.Lerp(0, Mathf.Sin(Time.time * idleScaleSpeed + randomSeedToTimeIdle) * idleScaleMagnitude, Mathf.Clamp01(currScale)), 1, 1);
    }

    public void playerClicked()
    {
        timeRemainingBeforeArrowAppears = timeBeforeArrowAppears;
    }

    public void ChangeOfScreen(float timer)
    {
        timeBeforeCanAppear = timer;
        canAppear = false;
        UILeaderboard.Instance.cvsVars.fadeParticle.Pause();
    }

}
