using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiFade : MonoBehaviour
{
    void Awake()
    {
        Instance = this;
    }
    public static UiFade Instance { get; private set; }

    [SerializeField] Image fonduNoir = null;
    [SerializeField] float currentAlpha = 1;

    float alphaAimed = 0;
    float alphaTimeTo = 2;

    private void Start()
    {
        fonduNoir.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, alphaAimed, Time.unscaledDeltaTime / alphaTimeTo);
        fonduNoir.color = new Color(Color.black.r, Color.black.g, Color.black.b, currentAlpha);
    }

    public void ChangeAlpha (float alphaGoTo, float alphaTime)
    {
        alphaAimed = alphaGoTo;
        alphaTimeTo = alphaTime;
    }


}
