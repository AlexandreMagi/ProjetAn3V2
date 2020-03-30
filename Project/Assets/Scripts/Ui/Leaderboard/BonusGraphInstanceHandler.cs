using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusGraphInstanceHandler : MonoBehaviour
{

    [SerializeField] Transform graphRoot = null;
    [SerializeField] CanvasGroup cvsGroup = null;
    [SerializeField] Text scoreText = null;
    [SerializeField] Text titleText = null;
    [SerializeField] Text descriptionText = null;
    [SerializeField] float speedLerp = 8;
    [SerializeField] float timeApear = 0.5f;
    public BonusHandler manager = null;

    float purcentageAlpha = 1;

    int lastFrameCountToTp = 0;

    // Update is called once per frame
    void Update()
    {
        float dt = Time.unscaledDeltaTime * UILeaderboard.Instance.deltaTimeMultiplier;
        if (Time.frameCount == lastFrameCountToTp + 1)
        {
            graphRoot.transform.position = transform.position;
            graphRoot.gameObject.SetActive(true);
            purcentageAlpha = 0;
        }
        if (purcentageAlpha < 1)
        {
            purcentageAlpha += dt / timeApear;
            if (purcentageAlpha > 1)
            {
                purcentageAlpha = 1;
            }
        }
        cvsGroup.alpha = purcentageAlpha;
        graphRoot.transform.position = Vector3.Lerp(graphRoot.transform.position, transform.position, dt * speedLerp);
    }

    public void InitGraph(string _scoreText,string _titleText, string _descriptionText)
    {
        scoreText.text = _scoreText;
        titleText.text = _titleText;
        descriptionText.text = _descriptionText;

        graphRoot.transform.SetParent(manager.rootGraph);
        graphRoot.transform.position = transform.position;
        graphRoot.gameObject.SetActive(false);
        lastFrameCountToTp = Time.frameCount;
    }
}
