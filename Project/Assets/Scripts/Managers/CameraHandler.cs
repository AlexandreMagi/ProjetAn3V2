using System;
using Cinemachine;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{

    private static CameraHandler _instance;
    public static CameraHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    // ##################################################################################################### //
    // ############################################# VARIABLES ############################################# //
    // ##################################################################################################### //
    #region Variables


    // --- Exposed variables --- //

    [Header("Cameras")]
    [Tooltip("Caméra cinémachine")]
    public Camera cinemachineCam = null;
    [Tooltip("Caméra animée via Unity")]
    public Camera animatedCam = null;
    [Tooltip("Animator de l'animatedCam")]
    public Animator animatorFromAnimatedCam = null;
    [Tooltip("Caméra qui va render")]
    public Camera renderingCam = null;

    [Header("Datas"), SerializeField]
    private DataCameraBasic camData = null;


    // --- Private variables --- //

    // Utilitary
    #region UtilitaryVars

    private GameObject camDelayPosDummy = null; // Dummy qui va permettre un retard sur les translations de la caméra
    private GameObject camDelayRotDummy = null; // Dummy qui va permettre un retard sur les rotations de la caméra
    private GameObject camDelayRotDummyParent = null;

    private Camera currentCamRef = null; // Stock de la caméra actuelle reference

    private bool currentCamIsCine = true; // Dit si l'on est sur la caméra animée ou la caméra cinemachine
    private float timerOnAnimatedCam = 0; // Stock le temps restant avant de passer en cam cinémachine

    Vector2 cursorRotateValue = Vector2.zero; // Rotation de la caméra en fonction de la position du curseur sur l'écran

    float recoilTranslationRef = 0; // Recul valeur ref
    float recoilTranslationValue = 0; // Recul actuel -> ref pow

    float recoilFovRef = 0; // Recul valeur ref
    float recoilFovValue = 0; // Recul actuel -> ref pow

    float fovModifViaSpeed = 0; // Stock la valeur modifié du fov en fonction de la vitesse du joueur

    bool feedbackActivated = true; // Dis si les feedbacks sont activés
    bool feedbackTransition = true; // Transition smooth entre feedback et sans
    float transitionTime = 0; // Vitesse de transition
    float transitionPurcentage = 0; // Vitesse de transition

    Camera camRef = null; // Caméra ref pour les calculs

    float fovAddedByTimeScale = 0; // Fov actuel affecté par le time scale

    float frequency = 0; // Vitesse actuelle du headbobing
    float currentFrequency = 0; // Vitesse actuelle du headbobing
    Vector2[] curveValues = new Vector2 [0]; // Stock des valeurs des curves
    bool stepSoundPlayed = false; // Dit si le son a été joué à ce pas
    AnimationCurve currentCinemachineCurve;
    CinemachineBlendDefinition.Style currentCinemachineStyle = CinemachineBlendDefinition.Style.Linear;

    Vector3 posCurrentSequenceEnd = Vector3.zero;
    Vector3 posCurrentSequenceStart = Vector3.zero;
    bool fadeStepsAtEndOfSequence = false;
    bool fadeStepsAtStartOfSequence = false;
    float distanceFadeStepsAtEnd = 1;
    float distanceFadeStepsAtStart = 2;

    // Short step
    bool onShortStep = false;
    AnimationCurve shortStepCurve = AnimationCurve.Linear(0, 0, 1, 1);
    float shortStepAmplitude = -1;

    float timerRemainingOnThisSequence = 0;
    float timerSequenceTotal = 0;

    float rotZBySpeed = 0;

    float chargevalue = 0;
    float currentPurcentageFBCharged = 0;
    bool feedbackChargedStarted = false;
    Transform cameraLookAt = null;
    float timeTransitionTo = 0;
    float timeTransitionBack = 0;
    float timeRemainingLookAt = 0;
    float currentPurcentageLookAt = 0;
    Vector3 savePosLookAt = Vector3.zero;
    float weightLookAt = 1;
    float weightRemoveRotLookAt = 1;

    float dt = 0;

    AnimatorOverrideController animatorOverrideController;

    float currentPurcentageIdle = 0;

    Vector3 rotateBalancePivot = Vector3.zero;
    Transform refPointBalance = null;
    Quaternion savedRotGoTo = Quaternion.identity;
    float timeToGoToRot = 0.001f;
    float currentPurcentageGoToNewRot = 0;
    float speedBalancing = 0;
    float dampingBalancing = 0;
    float personalTimeForMathfSin = 0;
    float minSpeedValue = 0;
    float balancingFrequency = 0;

    Quaternion goBackFromBalancingRotSaved = Quaternion.identity;
    Vector3 goBackFromBalancePosSaved = Vector3.zero;
    float purcentageLerpGoBack = 1;
    float returnLerpSpeedFromBalance = 1;
    float minSpeedRot = 1;

    bool breathingEnabled = false;
    float timeFadeBreathing = 1;
    float breathingIdlePurcentageActivated = 0;

    // Noise sur la caméra
    float noiseAmplitudePos = 0.5f;
    float noiseAmplitudeRot = 5f;
    float noiseFrequency = 0.01f;
    float customTimeForNoise = 0;
    float timeTransitionNoise = 1;
    float noisePurcentage = 0;
    float noisePurcentageAimed = 0;

    #endregion

    // Stock
    private CinemachineImpulseSource shakeSource = null;
    private DataWeapon weaponData = null;

    #endregion


    // ##################################################################################################### //
    // ############################################# FUNCTIONS ############################################# //
    // ##################################################################################################### //
    #region InitFunctions

    private void Awake()
    {
        _instance = this; // Stock singleton
    }

    private void Start()
    {
        animatorOverrideController = new AnimatorOverrideController(animatorFromAnimatedCam.runtimeAnimatorController);
        animatorFromAnimatedCam.runtimeAnimatorController = animatorOverrideController;


        // Desactivation des caméra animated et cinemachine pour passer sur la render cam
        if (cinemachineCam) cinemachineCam.enabled = false;
        else Debug.LogWarning("BUG : Camera isn't setup in CameraHandler");
        if (animatedCam) animatedCam.enabled = false;
        else Debug.LogWarning("BUG : Animated Camera isn't setup in CameraHandler");
        if (renderingCam)
        {
            renderingCam.enabled = true;
            shakeSource = GetComponent<CinemachineImpulseSource>();
        }
        else Debug.LogWarning("BUG : Camera isn't setup in CameraHandler");

        GameObject newCam = new GameObject();
        camRef = newCam.AddComponent<Camera>();
        camRef.fieldOfView = camData.BaseFov;
        camRef.name = "RefferenceCam";
        camRef.enabled = false;


        // Création du dummy qui va avoir un délai sur la rotation de la caméra et son parent
        camDelayRotDummyParent = new GameObject();
        camDelayRotDummyParent.name = "camDelayRotDummyParent";

        camDelayRotDummy = new GameObject();
        camDelayRotDummy.name = "camDelayRotDummy";
        camDelayRotDummy.transform.SetParent(camDelayRotDummyParent.transform);

        // Création du dummy qui va avoir un délai sur la translation de la caméra
        camDelayPosDummy = new GameObject();
        camDelayPosDummy.name = "camDelayPosDummy";

         curveValues = new Vector2[camData.CurvesAndValues.Length]; // Stock des valeurs des curves
        // Init des values des steps
        for (int i = 0; i < curveValues.Length; i++) { curveValues[i] = new Vector2(camData.CurvesAndValues[i].Decal, 1); }

        refPointBalance = new GameObject().transform;

        ResyncCamera(true); // Place les dummies
    }

    public void SetWeapon(DataWeapon _weaponData) { weaponData = _weaponData; }

    #endregion

    #region UpdateFunctions

    private void Update()
    {

        dt = camData.independentFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

        if (timerRemainingOnThisSequence > 0) timerRemainingOnThisSequence -= Time.deltaTime;
        if (timerRemainingOnThisSequence < 0) timerRemainingOnThisSequence  = 0;

        //Changement de frequence en fonction des curves cinémachines
        currentFrequency = frequency;
        if (currentCinemachineCurve != null)
        {
            if (currentCinemachineStyle == CinemachineBlendDefinition.Style.EaseOut)
                currentFrequency = frequency * currentCinemachineCurve.Evaluate(1 - (timerRemainingOnThisSequence / timerSequenceTotal));
            else if (currentCinemachineStyle == CinemachineBlendDefinition.Style.EaseIn)
                currentFrequency = frequency * currentCinemachineCurve.Evaluate(timerRemainingOnThisSequence / timerSequenceTotal);
            else if (currentCinemachineStyle == CinemachineBlendDefinition.Style.Linear)
                currentFrequency = frequency;
            else
                currentFrequency = frequency * currentCinemachineCurve.Evaluate(1 - (timerRemainingOnThisSequence / timerSequenceTotal));
        }

        //Permet de ralentir les pas à la fin d'un déplacement
        float distanceWithEnd = Vector3.Distance(currentCamRef.transform.position, posCurrentSequenceEnd);
        if (distanceWithEnd < distanceFadeStepsAtEnd && fadeStepsAtEndOfSequence)
        {
            currentFrequency = Mathf.Lerp(currentFrequency, 0, distanceWithEnd / distanceFadeStepsAtEnd);
        }
        //Permet de faire un fade des steps au lancement d'une séquence
        float distanceWithStart = Vector3.Distance(currentCamRef.transform.position, posCurrentSequenceStart);
        if (distanceWithStart < distanceFadeStepsAtStart && fadeStepsAtStartOfSequence)
        {
            currentFrequency = Mathf.Lerp(0, currentFrequency, distanceWithStart / distanceFadeStepsAtStart);
        }
        

        // Fait le switch entre la caméra cinemachine et la caméra animée
        currentCamRef = !currentCamIsCine && animatedCam.transform.position != Vector3.zero && animatedCam != null ? animatedCam : cinemachineCam;

        // Init
        camRef.transform.position = currentCamRef.transform.position;
        camRef.transform.rotation = currentCamRef.transform.rotation;
        UpdateCamValues(onShortStep);
        if (feedbackTransition)
        {
            if (feedbackActivated)
            {
                if (transitionPurcentage < 1)
                {
                    transitionPurcentage += Time.unscaledDeltaTime / transitionTime;
                    if (transitionPurcentage > 1) transitionPurcentage = 1;
                }
            }
            else
            {
                if (transitionPurcentage > 0)
                {
                    transitionPurcentage -= Time.unscaledDeltaTime / transitionTime;
                    if (transitionPurcentage < 0) transitionPurcentage = 0;
                }
            }
        }
        else
        {
            if (feedbackActivated) transitionPurcentage = 1;
            else transitionPurcentage = 0;
        }
        


        camRef.transform.position = Vector3.Lerp(currentCamRef.transform.position, camRef.transform.position, transitionPurcentage);
        camRef.transform.rotation = Quaternion.Lerp(currentCamRef.transform.rotation, camRef.transform.rotation, transitionPurcentage);
        camRef.fieldOfView = Mathf.Lerp(currentCamRef.fieldOfView, camRef.fieldOfView, transitionPurcentage);

        BalancingCamUpdate(); // Ovveride les feedbacks et la position si en balancement

        if (purcentageLerpGoBack < 1)
        {
            purcentageLerpGoBack = Mathf.Lerp(purcentageLerpGoBack, 1, Time.deltaTime * returnLerpSpeedFromBalance);
            if (purcentageLerpGoBack > 1 - 0.005f) purcentageLerpGoBack = 1;
            camRef.transform.rotation = Quaternion.Lerp(goBackFromBalancingRotSaved, camRef.transform.rotation, purcentageLerpGoBack);
            camRef.transform.position = Vector3.Lerp(goBackFromBalancePosSaved, camRef.transform.position, purcentageLerpGoBack);
        }

        renderingCam.transform.position = camRef.transform.position;
        renderingCam.transform.rotation = camRef.transform.rotation;
        renderingCam.fieldOfView = camRef.fieldOfView;

        Quaternion saveRotBeforeLookAt = camRef.transform.rotation;

        if (cameraLookAt != null && (cameraLookAt.gameObject ? cameraLookAt.gameObject.activeSelf : true))
        {
            if (currentPurcentageLookAt < 1) currentPurcentageLookAt += dt / timeTransitionTo;
            if (currentPurcentageLookAt > 1) currentPurcentageLookAt = 1;
            camRef.transform.LookAt(cameraLookAt, Vector3.up);
            savePosLookAt = cameraLookAt.position;
        }
        else
        {
            if (currentPurcentageLookAt > 0) currentPurcentageLookAt -= dt / timeTransitionBack;
            if (currentPurcentageLookAt < 0) currentPurcentageLookAt = 0;
            camRef.transform.LookAt(savePosLookAt, Vector3.up);
            cameraLookAt = null;
        }
        if (timeRemainingLookAt > 0) timeRemainingLookAt -= dt;
        if (timeRemainingLookAt < 0) ReleaselookAt();


        renderingCam.transform.rotation = Quaternion.Lerp(renderingCam.transform.rotation, Quaternion.Lerp(saveRotBeforeLookAt, camRef.transform.rotation,weightRemoveRotLookAt), currentPurcentageLookAt * weightLookAt);

        // Recuperation du shake de caméra de cinémachine si on est pas sur ce systeme
        if (!currentCamIsCine)
        {
            Vector3 cinemachineShakeAddedPos = Vector3.zero;
            Quaternion cinemachineShakeAddedRot = Quaternion.identity;
            CinemachineImpulseManager.Instance.GetImpulseAt(renderingCam.transform.position, false, 1, out cinemachineShakeAddedPos, out cinemachineShakeAddedRot);
            renderingCam.transform.position += cinemachineShakeAddedPos;
            renderingCam.transform.Rotate(cinemachineShakeAddedRot.eulerAngles);
        }

        // Noise effect
        customTimeForNoise += Time.deltaTime * noiseFrequency;
        Vector3 noiseValue = GetPerlinVectorThree();
        noisePurcentage = Mathf.MoveTowards(noisePurcentage, noisePurcentageAimed, Time.deltaTime / timeTransitionNoise);
        renderingCam.transform.position += noiseValue * noiseAmplitudePos * noisePurcentage;
        renderingCam.transform.Rotate (noiseValue * noiseAmplitudeRot * noisePurcentage);
    }

    private void BalancingCamUpdate()
    {
        if (speedBalancing > 0)
        {
            personalTimeForMathfSin += Time.deltaTime * balancingFrequency;
            camRef.transform.position = refPointBalance.position;
            camRef.transform.rotation = Quaternion.Lerp(refPointBalance.rotation, savedRotGoTo, currentPurcentageGoToNewRot);

            speedBalancing = Mathf.MoveTowards(speedBalancing, minSpeedRot, speedBalancing * dampingBalancing * Time.deltaTime);

            camRef.transform.RotateAround(rotateBalancePivot, camRef.transform.right*-1, speedBalancing * Mathf.Sin(personalTimeForMathfSin));


            currentPurcentageGoToNewRot = Mathf.MoveTowards(currentPurcentageGoToNewRot, 1, Time.deltaTime / timeToGoToRot);


            goBackFromBalancingRotSaved = camRef.transform.rotation;
            goBackFromBalancePosSaved = camRef.transform.position;
        }
        else
        {
            personalTimeForMathfSin = 0;
            currentPurcentageGoToNewRot = 0;
        }
    }

    public void StopBalancing()
    {
        if (speedBalancing > 0)
        {
            speedBalancing = 0;
            purcentageLerpGoBack = 0;
        }
    }

    public Vector3 pointDelayOnRotation()
    {
        return cinemachineCam.WorldToScreenPoint(currentCamIsCine ? (camData.followRotDummy ? camDelayRotDummy.transform.position : camDelayPosDummy.transform.position) : new Vector3(Screen.width, Screen.height, 0) / 2);
    }

    public float decalFromStepY()
    {
        return GetStepValues()[0];
    }

    private void UpdateCamDummyValues()
    {
        camDelayRotDummyParent.transform.position = currentCamRef.transform.position;
        camDelayRotDummyParent.transform.rotation = Quaternion.Lerp(camDelayRotDummyParent.transform.rotation, currentCamRef.transform.rotation, dt * camData.speedRotFollow);

        camDelayPosDummy.transform.position = Vector3.Lerp(camDelayPosDummy.transform.position, camDelayRotDummy.transform.position, dt * camData.speedPosFollow);
    }

    private void UpdateStepValues()
    {
        if (curveValues[0].x > camData.stepSoundPlay && !stepSoundPlayed)
        {
            stepSoundPlayed = true;
            //Debug.Log("Joue son de pas");
            //if (feedbackActivated) CustomSoundManager.Instance.PlaySound(renderingCam.gameObject, "Step_0" + UnityEngine.Random.Range(1, 5), false, 1f);
            if (feedbackActivated) CustomSoundManager.Instance.PlaySound("Step_0" + UnityEngine.Random.Range(1, 5), "Player", 1);
        }
        for (int i = 0; i < curveValues.Length; i++)
        {
            curveValues[i].x += dt * camData.CurvesAndValues[i].SpeedMultiplier * currentFrequency; // Purcentage
            if (curveValues[i].x > 1)
            {
                if (i == 0) stepSoundPlayed = false;
                curveValues[i].x -= curveValues[i].x - curveValues[i].x % 1; // Reset des pourcentages si il depassent 1
                if (camData.CurvesAndValues[i].IsInvertedAtEachLoop) curveValues[i].y *= -1;
            }
        }
    }

    private void UpdateCamValues(bool shortStep)
    {

        // Gestion du temps de la cam animé
        if (timerOnAnimatedCam > 0)
        {
            currentCamIsCine = false;
            timerOnAnimatedCam -= Time.deltaTime;
            if (timerOnAnimatedCam < 0)
            {
                currentCamIsCine = true;
                timerOnAnimatedCam = 0;
            }
        }

        // Fait le switch entre la caméra cinemachine et la caméra animée
        currentCamRef = !currentCamIsCine && animatedCam.transform.position != Vector3.zero && animatedCam != null ? animatedCam : cinemachineCam;

        // Init
        camRef.transform.position = currentCamRef.transform.position;
        camRef.transform.rotation = currentCamRef.transform.rotation;

        UpdateCamDummyValues();
        UpdateStepValues();
        HandleFBAtCharge();
        UpdateRecoilsValue();

        // Rotation en fonciton du dummy
        camRef.transform.rotation = Quaternion.LookRotation((camData.followRotDummy? camDelayRotDummy.transform.position : camDelayPosDummy.transform.position) - currentCamRef.transform.position, Vector3.up); // Regard de la cam

        float[] stepValues = GetStepValues();

        // Rotate en z en fonction de la vitesse horizontale
        rotZBySpeed = Mathf.Lerp (rotZBySpeed, -((currentCamRef.WorldToScreenPoint(camDelayPosDummy.transform.position).x - Screen.width / 2) / Screen.width) * camData.maxRotateWhileMoving, dt*camData.lerpOnLerpBecauseWhyTheFuckNot);
        //rotZBySpeed = -rotZBySpeed * camData.maxRotateWhileMoving / (Screen.width / 2);

        // Translate en arrière recoil
        camRef.transform.Translate(Vector3.back * recoilTranslationValue, Space.Self);
        if (shortStep)
        {
            // tanslate en fonction du pas actuel
            camRef.transform.Translate(Vector3.up * shortStepCurve.Evaluate(1 - (timerRemainingOnThisSequence / timerSequenceTotal)) * shortStepAmplitude, Space.World);
            // rotate en fonction de la vitesse horizontale et du pas actuel
            camRef.transform.Rotate(0, 0, rotZBySpeed, Space.Self);
        }
        else if (frequency < camData.minimumIdleTransition)
        {
            currentPurcentageIdle += dt / camData.idleTime;
            if (currentPurcentageIdle > 1) currentPurcentageIdle--;

            //Calcul des transition d'idle
            breathingIdlePurcentageActivated = Mathf.MoveTowards(breathingIdlePurcentageActivated, breathingEnabled ? 1 : 0, Time.deltaTime / timeFadeBreathing);

            // tanslate en fonction de la courbe d'idle
            float valueIdle = Mathf.Lerp(camData.idleCurve.Evaluate(currentPurcentageIdle) * camData.idleAmplitude, stepValues[0], frequency / camData.minimumIdleTransition);
            valueIdle = Mathf.Lerp(0, valueIdle, breathingIdlePurcentageActivated);
            camRef.transform.Translate(Vector3.up * valueIdle, Space.World);
            //camRef.transform.Translate(Vector3.right * Mathf.Lerp(0, stepValues[1], frequency/ camData.minimumIdleTransition), Space.Self);
            // rotate en fonction de la courbe d'idle
            camRef.transform.Rotate(0, 0, Mathf.Lerp(0, rotZBySpeed + stepValues[1], frequency / camData.minimumIdleTransition), Space.Self);
        }
        else
        {
            // tanslate en fonction du pas actuel
            camRef.transform.Translate(Vector3.up * stepValues[0], Space.World);
            //camRef.transform.Translate(Vector3.right * stepValues[2], Space.Self);
            // rotate en fonction de la vitesse horizontale et du pas actuel
            camRef.transform.Rotate(0, 0, rotZBySpeed + stepValues[1], Space.Self);
        }

        // Rotation en fonciton du cursor
        camRef.transform.Rotate(0, cursorRotateValue.y, 0, Space.World); // Rotation de la cam selon le placement du curseur (en Y)
        camRef.transform.Rotate(cursorRotateValue.x, 0, 0, Space.Self); // Rotation de la cam selon le placement du curseur (en X)

        fovModifViaSpeed = Mathf.Lerp(fovModifViaSpeed, currentFrequency * camData.fovMultiplier, dt * camData.fovSpeed);
        float fovAddedByChargeFeedback = weaponData != null ? feedbackChargedStarted ? weaponData.AnimValue.Evaluate(currentPurcentageFBCharged) * weaponData.fovModifier : 0 : 0;
        fovAddedByTimeScale = Mathf.Lerp(fovAddedByTimeScale, camData.timeScaleFovImpact - Time.timeScale * camData.timeScaleFovImpact, Time.unscaledDeltaTime * camData.timeScaleFovSpeed);
        camRef.fieldOfView = camData.BaseFov + camData.maxFovDecal * chargevalue + fovAddedByChargeFeedback + fovModifViaSpeed + fovAddedByTimeScale + recoilFovValue;

    }

    private void UpdateRecoilsValue()
    {
        recoilTranslationRef -= Time.unscaledDeltaTime * camData.RecoilRecover;
        if (recoilTranslationRef < 0)
            recoilTranslationRef = 0;
        recoilTranslationValue = Mathf.Lerp(recoilTranslationValue, Mathf.Pow(recoilTranslationRef, camData.RecoilPow), Time.unscaledDeltaTime*camData.RecoilLerpSpeed);

        recoilFovRef -= Time.unscaledDeltaTime * camData.fovRecoilRecover;
        if (recoilFovRef < 0)
            recoilFovRef = 0;
        recoilFovValue = Mathf.Lerp(recoilFovValue, Mathf.Pow(recoilFovRef, camData.fovRecoilPow), Time.unscaledDeltaTime * camData.RecoilLerpSpeed);
    }

    private float[] GetStepValues()
    {
        float[] returnedValues = new float[camData.CurvesAndValues.Length];
        for (int i = 0; i < returnedValues.Length; i++)
        {
            returnedValues[i] = (camData.CurvesAndValues[i].Curve.Evaluate(curveValues[i].x) * curveValues[i].y * camData.CurvesAndValues[i].MultipyValue) + camData.CurvesAndValues[i].OffsetValue; // Calcul de la value
        }
        return returnedValues;
    }

    #endregion

    /// <summary>
    /// Permet de resynchroniser la caméra entierement
    /// </summary>
    public void ResyncCamera(bool hardResync = false)
    {
        CameraLookAt(null, 0,0,0, 0.001f, 0);
        renderingCam.fieldOfView = camData.BaseFov;

        camDelayRotDummyParent.transform.position = renderingCam.transform.position;
        if (hardResync) camDelayRotDummyParent.transform.rotation = renderingCam.transform.rotation;

        camDelayRotDummy.transform.position = camDelayRotDummyParent.transform.position + camDelayRotDummyParent.transform.forward * camData.distanceBetweenDummy;

        if (hardResync) camDelayPosDummy.transform.position = camDelayRotDummy.transform.position;

        // Fait le switch entre la caméra cinemachine et la caméra animée
        currentCamRef = !currentCamIsCine && animatedCam.transform.position != Vector3.zero && animatedCam != null ? animatedCam : cinemachineCam;

        renderingCam.transform.position = currentCamRef.transform.position;
        if (hardResync) renderingCam.transform.rotation = currentCamRef.transform.rotation;
    }


    #region CamEffectsFunctions

    public void AddRecoil(bool fovType, float value, bool both = false) 
    {
        if (!fovType || both)
        {
            recoilTranslationRef += value;
            if (recoilTranslationRef > camData.RecoilMaxValue) recoilTranslationRef = camData.RecoilMaxValue;
        }
        if (fovType || both)
        {
            recoilFovRef += value;
            if (recoilFovRef > camData.maxFovRecoilValue) recoilFovRef = camData.maxFovRecoilValue;
        }
    }
    public void AddShake (float value, float duration = 1)
    {
        ChangeShakeDuration(duration);
        shakeSource.GenerateImpulse(Vector3.up * value); 
    }

    void ChangeShakeDuration (float duration)
    {
        shakeSource.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime = duration * 0.1f;
        shakeSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = duration * 0.2f;
        shakeSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime = duration * 0.7f;
        
        //Debug.Log(shakeSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime + shakeSource.m_ImpulseDefinition.m_TimeEnvelope.m_AttackTime + shakeSource.m_ImpulseDefinition.m_TimeEnvelope.m_DecayTime);
    }

    public void AddShake (float value, Vector3 initPos, float duration = 1)
    {

        ChangeShakeDuration(duration);
        float distance = Vector3.Distance(initPos, renderingCam.transform.position);
        value *= 1 - (distance / camData.distanceShakeCancelled);
        if (value > 0)
            shakeSource.GenerateImpulse(Vector3.up * value);
    }
    public void RemoveShake()
    {
        CinemachineImpulseManager.Instance.Clear();
    }

    private void HandleFBAtCharge()
    {
        float fChargedValuePast = chargevalue;
        chargevalue = UpdateChargeValue();
        if (chargevalue == 1 && fChargedValuePast != 1)
        {
            feedbackChargedStarted = true;
            AddShake(camData.shakeAtCharged);
        }

        if (chargevalue == 1 && feedbackChargedStarted && currentPurcentageFBCharged < 1)
        {
            if (weaponData != null) currentPurcentageFBCharged += Time.unscaledDeltaTime / weaponData.animTime;
        }
        else if (feedbackChargedStarted)
        {
            feedbackChargedStarted = false;
            currentPurcentageFBCharged = 0;
        }
        if (currentPurcentageFBCharged > 1)
        {
            AddShake(camData.shakeAtEndOfAnimation);
        }

        if (chargevalue == 1)
        {
            AddShake(camData.shakeWhenCharged * Time.unscaledDeltaTime);
        }
        else if (chargevalue > fChargedValuePast)
        {
            AddShake(camData.shakeWhenCharging * Time.unscaledDeltaTime);
        }

    }
    float UpdateChargeValue()
    {
        float currentChargevalue = Weapon.Instance.GetChargeValue();
        float _chargevalue = 0;
        if (currentChargevalue > camData.transitionStartAt)
            _chargevalue = (currentChargevalue - camData.transitionStartAt) / (1 - camData.transitionStartAt);
        return _chargevalue;
    }

    #endregion

    public void DecalCamWithCursor (Vector2 Pos) { cursorRotateValue = new Vector2(-(Pos.y * (camData.camMoveWithAim * 2) / Screen.height - camData.camMoveWithAim), Pos.x * (camData.camMoveWithAim * 2) / Screen.width - camData.camMoveWithAim); }

    #region SequencerFunctions

    public void FeedbackTransition(bool enabled, bool transition, float time)
    {
        if (!feedbackActivated && enabled) ResyncCamera(true);
        feedbackActivated = enabled;
        feedbackTransition = transition;
        if (feedbackTransition) transitionTime = time;
    }
    public void SetCurrentAnimCurve(CinemachineBlendDefinition animBlend)
    {
        currentCinemachineStyle = animBlend.m_Style;
        currentCinemachineCurve = animBlend.BlendCurve;
    }
    public void SetCurrentAnimCurveModified(AnimationCurve customCurve)
    {
        currentCinemachineStyle = CinemachineBlendDefinition.Style.Custom;
        currentCinemachineCurve = customCurve;
    }
    public void CameraLookAt(Transform camFollow, float _weightLookAt, float _weightRemoveRotLookAt, float _timeTransitionTo, float _timeTransitionBack, float timerBeforeGoBack = -1)
    {
        cameraLookAt = camFollow;
        timeTransitionTo = _timeTransitionTo;
        timeTransitionBack = _timeTransitionBack;
        weightLookAt = _weightLookAt;
        weightRemoveRotLookAt = _weightRemoveRotLookAt;
        timeRemainingLookAt = timerBeforeGoBack > 0 ? timerBeforeGoBack : timeRemainingLookAt;
    }
    public void ReleaselookAt()
    {
        cameraLookAt = null;
    }
    public void UpdateCamSteps (float _frequency, float timeSequence)
    {

        onShortStep = false;
        frequency = _frequency;
        currentFrequency = _frequency;
        timerRemainingOnThisSequence = timeSequence;
        timerSequenceTotal = timeSequence;
    }
    public void UpdatePos(Vector3 posStart, Vector3 posEnd, bool startAtStart, bool stopAtEnd, float fadeDistanceStart, float fadeDistanceEnd)
    {
        posCurrentSequenceStart = posStart;
        posCurrentSequenceEnd = posEnd;
        fadeStepsAtStartOfSequence = startAtStart;
        fadeStepsAtEndOfSequence = stopAtEnd;
        distanceFadeStepsAtStart = fadeDistanceStart;
        distanceFadeStepsAtEnd = fadeDistanceEnd;
    }
    public void UpdateBreathing(bool _breathingEnabled, float _timeFadeBreathing)
    {
        breathingEnabled = _breathingEnabled;
        timeFadeBreathing = _timeFadeBreathing;
    }
    public void ShortStep (AnimationCurve curve, float amplitude, float time)
    {
        onShortStep = true;
        shortStepCurve = curve;
        shortStepAmplitude = amplitude;
        timerRemainingOnThisSequence = time;
        timerSequenceTotal = time;
    }
    public void ChangeNoiseSettings(float _noisePurcentageAimed, float _timeTransitionNoise, float _noiseAmplitudePos = 0.5f, float _noiseAmplitudeRot = 5f, float _noiseFrequency = 0.01f)
    {
        noisePurcentageAimed = _noisePurcentageAimed;
        timeTransitionNoise = _timeTransitionNoise;
        noiseAmplitudePos = _noiseAmplitudePos;
        noiseAmplitudeRot = _noiseAmplitudeRot;
        noiseFrequency = _noiseFrequency;
    }
    float GetFloat(float seed) { return (Mathf.PerlinNoise(seed, customTimeForNoise) - 0.5f) * 2f; }
    Vector3 GetPerlinVectorThree() { return new Vector3(GetFloat(1), GetFloat(10), GetFloat(100)); }

    public float GetDistanceWithCam(Vector3 pos)
    {
        return Vector3.Distance(pos, renderingCam.transform.position);
    }

    public void TriggerAnim (AnimationClip animName, float animDuration)
    {
        animatorOverrideController = new AnimatorOverrideController(animatorFromAnimatedCam.runtimeAnimatorController);
        animatorFromAnimatedCam.runtimeAnimatorController = animatorOverrideController;
        animatorOverrideController[animatedCam.GetComponent<Animator>().runtimeAnimatorController.animationClips[1].name] = animName;
        currentCamIsCine = false;
        animatedCam.GetComponent<Animator>().SetTrigger("trigger");
        timerOnAnimatedCam = animDuration;
    }
    public void SetupBalancing (float _distanceUpBalancingAnchor, float initSpeedBalancing, float _dampingBalancing, float initialRot, float _minSpeedValue, float _returnLerpSpeedFromBalance,float _minSpeedRot,float _balancingFrequency, float _timeToGoToRot = 0.001f)
    {
        rotateBalancePivot = cinemachineCam.transform.position + Vector3.up * _distanceUpBalancingAnchor;
        refPointBalance.position = cinemachineCam.transform.position;
        refPointBalance.rotation = cinemachineCam.transform.rotation;
        refPointBalance.Rotate(Vector3.up * initialRot, Space.World);
        savedRotGoTo = refPointBalance.rotation;
        refPointBalance.rotation = cinemachineCam.transform.rotation;
        speedBalancing = initSpeedBalancing;
        dampingBalancing = _dampingBalancing;
        timeToGoToRot = _timeToGoToRot;
        minSpeedValue = _minSpeedValue;
        returnLerpSpeedFromBalance = _returnLerpSpeedFromBalance;
        minSpeedRot = _minSpeedRot;
        balancingFrequency = _balancingFrequency;
        if (timeToGoToRot < 0.001f) timeToGoToRot = 0.001f;
    }

    #endregion

}
