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

    // --- Outline
    [SerializeField] bool outlineActivatedAtInit = true;
    PostProcessOutline outlineEffect;

    // --- DepthOfField
    DepthOfField dofEffect;
    //float dofEffectDamageRecoverSpeed = 500;
    float recoverTimerDof = 0;

    // --- Lens Distortion
    LensDistortion distortionEffect;
    float distortionAnimPurcentage = 1;

    // --- Color Grading
    ColorGrading gradingEffect;
    float saturationGoTo = 0;
    float saturationSpeed = 0;
    float currentSaturation = 0;

    float lifeImpactSaturation = 0;

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

        // --- Outine
        outlineEffect = ScriptableObject.CreateInstance<PostProcessOutline>();
        outlineEffect.enabled.Override(outlineActivatedAtInit);

        ppVolume = PostProcessManager.instance.QuickVolume(11, 101f, outlineEffect);

        // --- DOF
        dofEffect = ScriptableObject.CreateInstance<DepthOfField>();
        //dofEffect.enabled.Override(dataPp.activateDof);
        dofEffect.enabled.Override(false);
        dofEffect.focusDistance.Override(dataPp.baseValueDof);

        ppVolume = PostProcessManager.instance.QuickVolume(11, 101f, dofEffect);

        // --- Lens Distortion
        distortionEffect = ScriptableObject.CreateInstance<LensDistortion>();
        distortionEffect.enabled.Override(true);
        distortionEffect.intensity.Override(0);
        distortionEffect.centerX.Override(0);
        distortionEffect.centerY.Override(0);

        ppVolume = PostProcessManager.instance.QuickVolume(11, 101f, distortionEffect);

        // --- Grading Effect
        gradingEffect = ScriptableObject.CreateInstance<ColorGrading>();
        gradingEffect.enabled.Override(true);
        gradingEffect.saturation.Override(0);
        
        ppVolume = PostProcessManager.instance.QuickVolume(11, 101f, gradingEffect);
    }

    // Update is called once per frame
    void Update()
    {
        HandlerChroma();
        //if (dataPp.activateDof)
        //    HandleDepthOfFieldDynamic();
        HandleLensDistortion();

        if (recoverTimerDof > 0)
        {
            recoverTimerDof -= Time.unscaledDeltaTime;
            if (recoverTimerDof < dataPp.takeDamageTimeDofRecover) dofEffect.focusDistance.value = Mathf.Lerp(dataPp.baseValueDof, 0, recoverTimerDof / dataPp.takeDamageTimeDofRecover);
            else dofEffect.focusDistance.value = 0;
            if (recoverTimerDof < 0)
            {
                recoverTimerDof = 0;
                dofEffect.focusDistance.value = dataPp.baseValueDof;
            }
        }

        Vector2 playerLifeValues = Player.Instance.GetLifeValues();
        SetupLifeSaturation(Player.Instance.getArmor() > 0 ? 0 : Mathf.RoundToInt((1 - playerLifeValues.x / playerLifeValues.y) * dataPp.saturationLostViaPlayerLife));
        currentSaturation = Mathf.MoveTowards(currentSaturation, saturationGoTo, Time.unscaledDeltaTime * saturationSpeed);
        gradingEffect.saturation.value = Mathf.Clamp(currentSaturation + lifeImpactSaturation, -100, 100);
    }

    void HandlerChroma()
    {
        float valueGoTo = dataPp.chromaMin;
        if (isChroma) valueGoTo = dataPp.chromaMax;
        chromaticAberrationEffect.intensity.value = Mathf.MoveTowards(chromaticAberrationEffect.intensity.value, valueGoTo, (dataPp.chromaChangeDependentFromTimeScale ? Time.deltaTime: Time.unscaledDeltaTime) * Mathf.Abs(dataPp.chromaMax - dataPp.chromaMin) / dataPp.chromaTimeTransition);
    }

    public void SetupSaturation (int value, float timeGoTo)
    {
        saturationGoTo = value;
        saturationSpeed = Mathf.Abs(gradingEffect.saturation.value - saturationGoTo) / timeGoTo;
    }

    public void SetupLifeSaturation (int value)
    {
        lifeImpactSaturation = value;
    }

    public void SetupDepthOfField ()
    {
        recoverTimerDof = dataPp.takeDamageTimeDofRecover + dataPp.takeDamageTimeDofStayBlur;
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
        else { multiplierByDistance = 1 - (Mathf.Clamp(distanceBetween, dataPp.maxDistToFade * dataPp.clampMinPurcentageDistortion, dataPp.maxDistToFade, ) / dataPp.maxDistToFade); }

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

    public void OutlineSetActive (bool active)
    {
        outlineEffect.active = active;
    }


}
