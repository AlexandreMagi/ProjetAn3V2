using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(PostProcessVolume))]  
public class PostprocessManager : MonoBehaviour
{

    PostProcessVolume ppVolume = null;

    [SerializeField]
    DataPostEffect dataPp = null;

    // --- Chroma
    ChromaticAberration chromaticAberrationEffect;
    [HideInInspector]
    public bool isChroma = false;

    // --- Vignette
    Vignette vignetteEffect;

    // --- DepthOfField
    DepthOfField dofEffect;

    public static PostprocessManager Instance { get; private set; }
    void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        // --- Chroma
        chromaticAberrationEffect = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromaticAberrationEffect.enabled.Override(true);
        chromaticAberrationEffect.intensity.Override(0);

        ppVolume = PostProcessManager.instance.QuickVolume(11, 100f, chromaticAberrationEffect);

        // --- Vignette
        vignetteEffect = ScriptableObject.CreateInstance<Vignette>();
        vignetteEffect.enabled.Override(true);
        vignetteEffect.intensity.Override(0.25f);
        vignetteEffect.smoothness.Override(1);

        // --- Vignette
        dofEffect = ScriptableObject.CreateInstance<DepthOfField>();
        dofEffect.enabled.Override(true);


        ppVolume = PostProcessManager.instance.QuickVolume(11, 101f, vignetteEffect);
    }

    // Update is called once per frame
    void Update()
    {
        HandlerChroma();
        HandleDepthOfField();
    }

    void HandlerChroma()
    {
        float valueGoTo = dataPp.chromaMin;
        if (isChroma) valueGoTo = dataPp.chromaMax;
        chromaticAberrationEffect.intensity.value = Mathf.MoveTowards(chromaticAberrationEffect.intensity.value, valueGoTo, (dataPp.chromaChangeDependentFromTimeScale ? Time.deltaTime: Time.unscaledDeltaTime) * Mathf.Abs(dataPp.chromaMax - dataPp.chromaMin) / dataPp.chromaTimeTransition);
    }

    void HandleDepthOfField()
    {

    }


}
