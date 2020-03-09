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
    bool isChroma = false;
    public void setChroma(bool b) { isChroma = b; }
    float multiplierByDistance = 0;

    // --- Vignette
    Vignette vignetteEffect;

    // --- DepthOfField
    DepthOfField dofEffect;

    // --- LensDistortion
    LensDistortion distortionEffect;
    float distortionAnimPurcentage = 1;

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

        ppVolume = PostProcessManager.instance.QuickVolume(11, 101f, vignetteEffect);

        // --- DOF
        dofEffect = ScriptableObject.CreateInstance<DepthOfField>();
        dofEffect.enabled.Override(dataPp.activateDof);
        dofEffect.focusDistance.Override(50);

        // --- Lens Distortion
        distortionEffect = ScriptableObject.CreateInstance<LensDistortion>();
        distortionEffect.enabled.Override(true);
        distortionEffect.intensity.Override(0);
        distortionEffect.centerX.Override(0);
        distortionEffect.centerY.Override(0);

        ppVolume = PostProcessManager.instance.QuickVolume(11, 101f, distortionEffect);
    }

    // Update is called once per frame
    void Update()
    {
        HandlerChroma();
        if (dataPp.activateDof)
            HandleDepthOfFieldDynamic();
        HandleLensDistortion();
    }

    void HandlerChroma()
    {
        float valueGoTo = dataPp.chromaMin;
        if (isChroma) valueGoTo = dataPp.chromaMax;
        chromaticAberrationEffect.intensity.value = Mathf.MoveTowards(chromaticAberrationEffect.intensity.value, valueGoTo, (dataPp.chromaChangeDependentFromTimeScale ? Time.deltaTime: Time.unscaledDeltaTime) * Mathf.Abs(dataPp.chromaMax - dataPp.chromaMin) / dataPp.chromaTimeTransition);
    }

    void HandleDepthOfFieldDynamic()
    {

        Ray rayBullet = CameraHandler.Instance.renderingCam.ScreenPointToRay(Main.Instance.GetCursorPos());

        //Shoot raycast
        RaycastHit hit;
        if (Physics.Raycast(rayBullet, out hit, Mathf.Infinity, dataPp.lmask))
        {
            dofEffect.focusDistance.value = Mathf.Lerp(dofEffect.focusDistance.value, hit.distance, (dataPp.dofDependentFromTimeScale ? Time.deltaTime : Time.unscaledDeltaTime) * dataPp.transitionSpeed);
        }
    }

    public void doDistortion(Transform target) 
    { 

        Vector3 posScreen = CameraHandler.Instance.renderingCam.WorldToScreenPoint(target.position);

        float distanceBetween = Vector3.Distance(target.position, CameraHandler.Instance.renderingCam.transform.position);
        if (distanceBetween > dataPp.maxDistToFade) multiplierByDistance = 0;
        else { multiplierByDistance = 1 - (distanceBetween / dataPp.maxDistToFade); }

        if (posScreen.z > 0)
        {
            Vector2 pos = new Vector2(posScreen.x / Screen.width, posScreen.y / Screen.height) * 2 - Vector2.one;
            distortionEffect.centerX.value = pos.x;
            distortionEffect.centerY.value = pos.y;
            distortionAnimPurcentage = 0;
        }
    }

    void HandleLensDistortion()
    {
        if (distortionAnimPurcentage < 1)
        {
            distortionEffect.intensity.value = dataPp.animDistortionAtOrb.Evaluate(distortionAnimPurcentage) * dataPp.animDistortionMultiplier * multiplierByDistance;
            distortionAnimPurcentage += (dataPp.distortionDependentFromTimeScale ? Time.deltaTime : Time.unscaledDeltaTime) / dataPp.animDistortionDuration;
            if (distortionAnimPurcentage > 1)
            {
                distortionAnimPurcentage = 1;
                distortionEffect.intensity.value = 0;
            }
        }
    }


}
