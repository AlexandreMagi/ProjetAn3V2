using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessEffects : MonoBehaviour
{
    PostProcessVolume Volume;

    ChromaticAberration chromaticAberrationEffect;

    bool isChroma = false;

    [SerializeField]
    float maxChromaAberration = 1f;

    private void Start()
    {
        chromaticAberrationEffect = ScriptableObject.CreateInstance<ChromaticAberration>();
        chromaticAberrationEffect.enabled.Override(true);
        chromaticAberrationEffect.intensity.Override(0);

        Volume = PostProcessManager.instance.QuickVolume(11, 100f, chromaticAberrationEffect);
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
        Debug.Log(b);
        if (b)
        {
            isChroma = true;
        }
        else if (!b)
        {
            isChroma = false;
        }
    }
}
