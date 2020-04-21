using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessEffects : MonoBehaviour
{
    PostProcessVolume Volume = null;

    ChromaticAberration chromaticAberrationEffect;
    Vignette vignetteEffect;
    MotionBlur motionBlurEffect;
     
    bool isChroma = false;

    //bool isVignetteFalling = true;

    [SerializeField]
    float maxChromaAberration = 1f;

    private void Start()
    {
        vignetteEffect = ScriptableObject.CreateInstance<Vignette>();
        vignetteEffect.enabled.Override(true);
        vignetteEffect.intensity.Override(0.25f);
        vignetteEffect.smoothness.Override(1);


        vignetteEffect = ScriptableObject.CreateInstance<Vignette>();
        vignetteEffect.enabled.Override(true);
        vignetteEffect.intensity.Override(0.25f);
        vignetteEffect.smoothness.Override(1);

        chromaticAberrationEffect = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromaticAberrationEffect.enabled.Override(true);
        chromaticAberrationEffect.intensity.Override(0);

        Volume = PostProcessManager.instance.QuickVolume(11, 100f, chromaticAberrationEffect);
        Volume = PostProcessManager.instance.QuickVolume(11, 101f, vignetteEffect);
    }

    void Update()
    {
        if (chromaticAberrationEffect != null)
        {
            if (isChroma && chromaticAberrationEffect.intensity.value <= maxChromaAberration)
            {
                chromaticAberrationEffect.intensity.value += Time.deltaTime;
            }
            else if (isChroma)
            {
                chromaticAberrationEffect.intensity.value = maxChromaAberration;
            }

            if (!isChroma && chromaticAberrationEffect.intensity.value >= 0)
            {
                chromaticAberrationEffect.intensity.value -= Time.deltaTime;
            }
            else if (!isChroma)
            {
                chromaticAberrationEffect.intensity.value = 0;
            }
        }
    }

    public void ChromaChanges(bool b)
    {
        isChroma = b;
    }

    public void VignetteChanges()
    {
        vignetteEffect.intensity.value = 1;
    }
}
