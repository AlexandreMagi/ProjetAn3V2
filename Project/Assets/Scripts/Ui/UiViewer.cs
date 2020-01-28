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
}
