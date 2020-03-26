using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeScript : MonoBehaviour
{
    public static WelcomeScript Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }

    [SerializeField] bool activate = false;

    [SerializeField] float timerBeforeCall = 1;
    [SerializeField] GameObject textMesh = null;
    [SerializeField] AnimationCurve anim = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float scaleMultiplier = 0.3f;
    [SerializeField] float animTime = 0.8f;
    float welcomeAnimPurcentage = 1;

    private void Start()
    {
        if (!activate) Destroy(this);
        else
        Invoke("LaunchWelcome", timerBeforeCall);
    }

    // Update is called once per frame
    void Update()
    {
        if (welcomeAnimPurcentage < 1)
        {
            textMesh.transform.localScale = anim.Evaluate(welcomeAnimPurcentage) * scaleMultiplier * Vector3.one;
            welcomeAnimPurcentage += Time.unscaledDeltaTime / animTime;
        }
        if (welcomeAnimPurcentage > 1)
        {
            textMesh.transform.localScale = Vector3.zero;
            welcomeAnimPurcentage = 1;
        }
    }

    public void LaunchWelcome()
    {
        welcomeAnimPurcentage = 0;
    }
}
