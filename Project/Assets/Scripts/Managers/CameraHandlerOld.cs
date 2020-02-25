using Cinemachine;
using UnityEngine;

public class CameraHandlerOld : MonoBehaviour
{

    private static CameraHandlerOld _instance;
    public static CameraHandlerOld Instance
    {
        get
        {
            return _instance;
        }
    }

    #region VAR


    [Header("Cameras")]
    [Tooltip("Dummy qui bouge sur lequel va se fixer la cam")]
    public GameObject CamDummy = null;
    [Tooltip("Dummy qui bouge sur lequel va se fixer la cam")]
    public GameObject AnimatedCam = null;
    [Tooltip("Camera render")]
    public GameObject RenderingCam = null;
    GameObject currentCamDummy = null;
    bool onCinemachineCamera = true;
    float timerOnCinemachineCam = 0;

    [SerializeField]
    DataCameraBasic camBasicData = null;

    DataWeapon weaponData = null;

    Vector3 vLookAtPos = Vector3.zero; // Vector3 qui va avoir un offset sur là où la camera devrait regarder
    Vector2 vRotateValue = Vector2.zero; // Valeur de rotation selon la position du curseur dans l'écran

    float currentRecoilValue = 0; // Recul valeur ref
    float currentRecoil = 0; // Recul actuel -> ref pow

    float currentFovRecoilValue = 0; // Recul valeur ref
    float currentFovRecoil = 0; // Recul actuel -> ref pow

    float currentFovModif = 0;

    [HideInInspector]
    public bool bFeedbckActivated = true; // Permet de desactiver les feedbacks de la caméra
    bool bFeedbackTransition = false;
    float fCurrentSpeedTransition = 0;

    GameObject camDummyValueFeedback;
    float CamDummyFov = 70;

    float fovAddedByTimeScale = 0;

    #region HeadBobingVar

    float fFrequency = 0; // Vitesse actuelle du headbobing
    float fAimedFrenquency = 0; // Vitesse visée
    float fFrequencyAccel = 1; // Acceleration
    float[] CurrentPurcentageStock; // Stock des pourcentages de chaque curve
    float[] CurrentInvertedStock; // Stock des inversions de chaque curve

    #endregion

    float chargevalue = 0;
    float currentPurcentageFBCharged = 0;
    bool feedbackChargedStarted = false;
    bool stepSoundPlayed = false;

    CinemachineImpulseSource CineMachImpulse;

    Transform cameraLookAt = null;
    float timeTransitionTo = 0;
    float timeTransitionBack = 0;
    float timeRemainingLookAt = 0;
    float currentPurcentageLookAt = 0;
    Vector3 savePosLookAt = Vector3.zero;

    #region ZeroGEffect

    [Header("Zero G effect")]
    [SerializeField]
    DataZeroGOnPlayer zeroGData = null;

    float zeroGanimPurcentage = 0;
    bool onZeroG = false;
    float currentZeroGRot = 0;
    int rotDir;

    #endregion


    #endregion


    private void Awake()
    {
        _instance = this;
        if (CamDummy.GetComponent<Camera>()) CamDummy.GetComponent<Camera>().enabled = false;
        if (RenderingCam.GetComponent<Camera>()) RenderingCam.GetComponent<Camera>().enabled = true;
        if (AnimatedCam != null && AnimatedCam.GetComponent<Camera>()) AnimatedCam.GetComponent<Camera>().enabled = false;
    }

    private void Start()
    {
        CineMachImpulse = GameObject.FindObjectOfType<CinemachineImpulseSource>();
        // Changement des tailles des tableaux
        CurrentPurcentageStock = new float[camBasicData.CurvesAndValues.Length];
        CurrentInvertedStock = new float[camBasicData.CurvesAndValues.Length];
        // Stockage des valeurs de base
        for (int i = 0; i < CurrentPurcentageStock.Length; i++)
        {
            CurrentPurcentageStock[i] = camBasicData.CurvesAndValues[i].Decal;
            CurrentInvertedStock[i] = 1;
        }
        camDummyValueFeedback = new GameObject();
        currentCamDummy = !onCinemachineCamera && AnimatedCam.transform.position != Vector3.zero && AnimatedCam != null ? AnimatedCam : CamDummy;
        vLookAtPos = currentCamDummy.transform.position + currentCamDummy.transform.forward * 5;
    }

    public void FeedbackTransition(bool bValue, float fSpeedTransi)
    {
        bFeedbackTransition = bValue;
        if (bFeedbackTransition)
        {
            fCurrentSpeedTransition = fSpeedTransi;
        }
    }

    public void AddRecoil(float _RecoilValue)
    {
        currentRecoilValue += _RecoilValue;
        if (currentRecoilValue > camBasicData.RecoilMaxValue)
            currentRecoilValue = camBasicData.RecoilMaxValue;
    }

    public void AddFovRecoil(float value)
    {
        currentFovRecoilValue += value;
        if (currentFovRecoilValue > camBasicData.maxFovRecoilValue)
            currentFovRecoilValue = camBasicData.maxFovRecoilValue;
    }


    /// <summary>
    /// Fonction lancé à chaque frame
    /// </summary>
    public void Update()
    {
        HandleFBAtCharge();
        currentRecoilValue -= Time.unscaledDeltaTime * camBasicData.RecoilRecover;
        if (currentRecoilValue < 0)
            currentRecoilValue = 0;

        currentFovRecoilValue -= Time.unscaledDeltaTime * camBasicData.RecoilRecover;
        if (currentFovRecoilValue < 0)
            currentFovRecoilValue = 0;

        //currentRecoil = Mathf.Pow(currentRecoilValue, camBasicData.RecoilPow);
        currentRecoil = Mathf.Lerp(currentRecoil, Mathf.Pow(currentRecoilValue, camBasicData.RecoilPow), Time.unscaledDeltaTime * camBasicData.RecoilLerpSpeed);
        currentFovRecoil = Mathf.Pow(currentFovRecoilValue, camBasicData.fovRecoilPow);

        if (fFrequency > camBasicData.frenquencyGoBackToZero)
            fFrequency = Mathf.MoveTowards(fFrequency, fAimedFrenquency, Time.deltaTime * fFrequencyAccel * fFrequency); // Changement de la frequence en fonction de la valeur "GoTo"
        else
            fFrequency = Mathf.MoveTowards(fFrequency, fAimedFrenquency, Time.deltaTime * fFrequencyAccel * camBasicData.frequencyDeccel); // Changement de la frequence en fonction de la valeur "GoTo"

        if (CurrentPurcentageStock[0] > camBasicData.stepSoundPlay && !stepSoundPlayed && bFeedbckActivated)
        {
            stepSoundPlayed = true;
            CustomSoundManager.Instance.PlaySound(RenderingCam, "Step_0" + Random.Range(1, 5), false, 1f);
        }

        for (int i = 0; i < CurrentPurcentageStock.Length; i++)
        {
            CurrentPurcentageStock[i] += Time.deltaTime * camBasicData.CurvesAndValues[i].SpeedMultiplier * fFrequency; // Augmentation des pourcentages
            if (CurrentPurcentageStock[i] > 1)
            {
                if (i == 0)
                    stepSoundPlayed = false;
                CurrentPurcentageStock[i] -= CurrentPurcentageStock[i] - CurrentPurcentageStock[i] % 1; // Reset des pourcentages si il depassent 1
                if (camBasicData.CurvesAndValues[i].IsInvertedAtEachLoop) // Inversion de certaines curves si le pourcentage depasse 1
                {
                    CurrentInvertedStock[i] *= -1;
                }
            }
        }

        if (timerOnCinemachineCam > 0)
        {
            timerOnCinemachineCam -= Time.deltaTime;
            if (timerOnCinemachineCam < 0)
                onCinemachineCamera = true;
        }

        currentCamDummy = !onCinemachineCamera && AnimatedCam.transform.position != Vector3.zero && AnimatedCam != null? AnimatedCam : CamDummy;

        // Reset de la position et de la rotation
        camDummyValueFeedback.transform.position = currentCamDummy.transform.position;
        camDummyValueFeedback.transform.rotation = currentCamDummy.transform.rotation;
        /// ----- GET VALUES ----- ///
        float[] camBasicDataValues = GetValue(); // Get les values des steps
        vLookAtPos = Vector3.Lerp(vLookAtPos, currentCamDummy.transform.position + currentCamDummy.transform.forward * 5, Time.deltaTime * camBasicData.camFollowSpeed); // Lerp du v3 vers la direction de la cam
        Quaternion targetRotation = Quaternion.LookRotation(vLookAtPos - currentCamDummy.transform.position, Vector3.up); // Regard de la cam

        /// ----- SET ROTATIONS ----- ///
        camDummyValueFeedback.transform.rotation = targetRotation; // Oriente la cam
        camDummyValueFeedback.transform.Rotate(0, vRotateValue.y, 0, Space.World); // Rotation de la cam selon le placement du curseur (en Y)
        camDummyValueFeedback.transform.Rotate(vRotateValue.x, 0, 0, Space.Self); // Rotation de la cam selon le placement du curseur (en X)

        // Setup position selon recul
        camDummyValueFeedback.transform.Translate(Vector3.back * currentRecoil, Space.Self);
        // Get de la valeur de rotation selon les mouvements horizontaux
        float vRotYByPosition = currentCamDummy.GetComponent<Camera>().WorldToScreenPoint(vLookAtPos).x - Screen.width / 2;
        vRotYByPosition = -vRotYByPosition * camBasicData.maxRotateWhileMoving / (Screen.width / 2);

        if (Input.GetKeyDown(KeyCode.Space)) StartZeroG();

        float dt = (zeroGData.animIndependentFromTimescale ? Time.unscaledDeltaTime : Time.deltaTime);
        if (onZeroG)
        {
            zeroGanimPurcentage += dt / zeroGData.animTime;
            if (zeroGanimPurcentage > 1) onZeroG = false;
            camDummyValueFeedback.transform.Translate(0, zeroGData.upAxisAnim.Evaluate(zeroGanimPurcentage) * zeroGData.upAxisValue, 0, Space.World);
            currentZeroGRot += dt * zeroGData.rollSpeedController.Evaluate(zeroGanimPurcentage) * zeroGData.rollSpeed * rotDir;
            if (Mathf.Abs(currentZeroGRot) > 360) currentZeroGRot -= 360 * rotDir;
        }
        else 
            currentZeroGRot = Mathf.MoveTowards(currentZeroGRot, Mathf.Abs(currentZeroGRot - 360 * rotDir) < currentZeroGRot ? 360 * rotDir : 0, dt * zeroGData.rotRecoverSpeed);
        UiDamageHandler.Instance.UpdateZeroGScreen(zeroGData, zeroGanimPurcentage, onZeroG);
        camDummyValueFeedback.transform.Rotate(0, 0, currentZeroGRot, Space.Self);

        /// ----- STEP ----- ///
        // Rotations de la cam selon les mouvements horizontaux + rotate en Z des steps
        camDummyValueFeedback.transform.Rotate(0, 0, -vRotYByPosition + camBasicDataValues[1], Space.Self);
        // Setup position selon steps
        camDummyValueFeedback.transform.Translate(Vector3.up * camBasicDataValues[0], Space.World);

        currentFovModif = Mathf.Lerp(currentFovModif, fFrequency * camBasicData.fovMultiplier, Time.deltaTime * camBasicData.fovSpeed);
        // Change le FOV
        float fovAddedByChargeFeedback = weaponData != null ? feedbackChargedStarted ? weaponData.AnimValue.Evaluate(currentPurcentageFBCharged) * weaponData.fovModifier : 0 : 0;
        fovAddedByTimeScale = Mathf.Lerp(fovAddedByTimeScale, camBasicData.timeScaleFovImpact - Time.timeScale * camBasicData.timeScaleFovImpact, Time.unscaledDeltaTime * camBasicData.timeScaleFovSpeed);
        CamDummyFov = camBasicData.BaseFov + camBasicData.maxFovDecal * chargevalue + fovAddedByChargeFeedback + currentFovModif + fovAddedByTimeScale + currentFovRecoilValue;

        if (cameraLookAt != null && (cameraLookAt.gameObject ? cameraLookAt.gameObject.activeSelf : true))// && !(cameraLookAt.gameObject != null && cameraLookAt.gameObject.activeSelf != false))
        {


            RenderingCam.transform.position = camDummyValueFeedback.transform.position;
            RenderingCam.transform.rotation = camDummyValueFeedback.transform.rotation;
            camDummyValueFeedback.transform.LookAt(cameraLookAt, Vector3.up);

            RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, camDummyValueFeedback.transform.rotation, currentPurcentageLookAt);

            if (!bFeedbckActivated)
            {
                if (bFeedbackTransition) RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(RenderingCam.GetComponent<Camera>().fieldOfView, currentCamDummy.GetComponent<Camera>().fieldOfView, Time.deltaTime * fCurrentSpeedTransition);
                else RenderingCam.GetComponent<Camera>().fieldOfView = currentCamDummy.GetComponent<Camera>().fieldOfView;
            }
            else
            {
                if (bFeedbackTransition) RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(RenderingCam.GetComponent<Camera>().fieldOfView, CamDummyFov, Time.deltaTime * fCurrentSpeedTransition);
                else RenderingCam.GetComponent<Camera>().fieldOfView = CamDummyFov;
            }
            if (currentPurcentageLookAt < 1) currentPurcentageLookAt += Time.deltaTime / timeTransitionTo;
            if (currentPurcentageLookAt > 1) currentPurcentageLookAt = 1;
            savePosLookAt = cameraLookAt.position;

        }
        else
        {
            cameraLookAt = null;
            if (currentPurcentageLookAt > 0) currentPurcentageLookAt -= Time.deltaTime / timeTransitionBack;
            if (currentPurcentageLookAt < 0) currentPurcentageLookAt = 0;

            if (!bFeedbckActivated)
            {
                if (bFeedbackTransition)
                {
                    RenderingCam.transform.position = Vector3.Lerp(RenderingCam.transform.position, currentCamDummy.transform.position, Time.deltaTime * fCurrentSpeedTransition);
                    RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, currentCamDummy.transform.rotation, Time.deltaTime * fCurrentSpeedTransition);
                    RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(RenderingCam.GetComponent<Camera>().fieldOfView, currentCamDummy.GetComponent<Camera>().fieldOfView, Time.deltaTime * fCurrentSpeedTransition);
                }
                else
                {
                    RenderingCam.transform.position = currentCamDummy.transform.position;
                    RenderingCam.transform.rotation = currentCamDummy.transform.rotation;
                    RenderingCam.GetComponent<Camera>().fieldOfView = currentCamDummy.GetComponent<Camera>().fieldOfView;
                }
                RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, currentCamDummy.transform.rotation, currentPurcentageLookAt);
            }
            else
            {
                if (bFeedbackTransition)
                {
                    RenderingCam.transform.position = Vector3.Lerp(RenderingCam.transform.position, camDummyValueFeedback.transform.position, Time.deltaTime * fCurrentSpeedTransition);
                    RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, camDummyValueFeedback.transform.rotation, Time.deltaTime * fCurrentSpeedTransition);
                    RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(RenderingCam.GetComponent<Camera>().fieldOfView, CamDummyFov, Time.deltaTime * fCurrentSpeedTransition);
                }
                else
                {
                    RenderingCam.transform.position = camDummyValueFeedback.transform.position;
                    RenderingCam.transform.rotation = camDummyValueFeedback.transform.rotation;
                    RenderingCam.GetComponent<Camera>().fieldOfView = CamDummyFov;
                }
                camDummyValueFeedback.transform.LookAt(savePosLookAt, Vector3.up);
                RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, camDummyValueFeedback.transform.rotation, currentPurcentageLookAt);
            }
        }
        if (timeRemainingLookAt > 0) timeRemainingLookAt -= Time.deltaTime;
        if (timeRemainingLookAt < 0)
        {

        }

    }

    public void StartZeroG()
    {
        onZeroG = true;
        zeroGanimPurcentage = 0;
        currentZeroGRot = 0;
        rotDir = Mathf.RoundToInt(1 * Mathf.Sign(Random.Range(-1f, 1f)));
    }

    public void DecalCurrentCamRotation(Vector2 Pos)
    {
        vRotateValue = new Vector2(-(Pos.y * (camBasicData.camMoveWithAim * 2) / Screen.height - camBasicData.camMoveWithAim), Pos.x * (camBasicData.camMoveWithAim * 2) / Screen.width - camBasicData.camMoveWithAim); // Get values
    }

    public void CameraLookAt(Transform camFollow, float _timeTransitionTo, float _timeTransitionBack, float timerBeforeGoBack = -1)
    {
        cameraLookAt = camFollow;
        timeTransitionTo = _timeTransitionTo;
        timeTransitionBack = _timeTransitionBack;
        timeRemainingLookAt = timerBeforeGoBack > 0 ? timerBeforeGoBack : timeRemainingLookAt;
    }

    private void HandleFBAtCharge()
    {
        float fChargedValuePast = chargevalue;
        chargevalue = UpdateChargeValue();
        if (chargevalue == 1 && fChargedValuePast != 1)
        {
            feedbackChargedStarted = true;
            AddShake(camBasicData.shakeAtCharged);
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
            AddShake(camBasicData.shakeAtEndOfAnimation);
        }

        if (chargevalue == 1)
        {
            AddShake(camBasicData.shakeWhenCharged * Time.unscaledDeltaTime);
        }
        else if (chargevalue != fChargedValuePast)
        {
            AddShake(camBasicData.shakeWhenCharging * Time.unscaledDeltaTime);
        }

    }

    float UpdateChargeValue()
    {
        float currentChargevalue = Weapon.Instance.GetChargeValue();
        float _chargevalue = 0;
        if (currentChargevalue > camBasicData.transitionStartAt)
            _chargevalue = (currentChargevalue - camBasicData.transitionStartAt) / (1 - camBasicData.transitionStartAt);
        return _chargevalue;
    }

    public void AddShake(float value)
    {
        CineMachImpulse.GenerateImpulse(Vector3.up * value);
    }
    public void AddShake(float value, Vector3 initPos)
    {
        float distance = Vector3.Distance(initPos, RenderingCam.transform.position);
        value *= 1 - (distance / camBasicData.distanceShakeCancelled);
        if (value > 0)
            CineMachImpulse.GenerateImpulse(Vector3.up * value);
    }

    public void SetWeapon(DataWeapon _weaponData)
    {
        weaponData = _weaponData;
    }


    /// <summary>
    /// Setup les values de rotations de la cam selon le placement du curseur sur l'écran
    /// </summary>
    /// <param name="fMaxValue"></param>
    /// <param name="Pos"></param>
    public void DecalCurrentCamRotation(float fMaxValue, Vector2 Pos)
    {
        vRotateValue = new Vector2(-(Pos.y * (fMaxValue * 2) / Screen.height - fMaxValue), Pos.x * (fMaxValue * 2) / Screen.width - fMaxValue); // Get values
    }

    /// <summary>
    /// Permet de get les valeurs crées par le système
    /// </summary>
    /// <returns></returns>
    public float[] GetValue()
    {
        float[] tReturnedValues = new float[camBasicData.CurvesAndValues.Length];
        for (int i = 0; i < tReturnedValues.Length; i++)
        {
            tReturnedValues[i] = (camBasicData.CurvesAndValues[i].Curve.Evaluate(CurrentPurcentageStock[i]) * CurrentInvertedStock[i] * camBasicData.CurvesAndValues[i].MultipyValue) + camBasicData.CurvesAndValues[i].OffsetValue; // Calcul de la value
            if (fFrequency < camBasicData.frenquencyGoBackToZero)
            {
                tReturnedValues[i] = Mathf.Lerp(tReturnedValues[i], 0, (camBasicData.frenquencyGoBackToZero - fFrequency) / camBasicData.frenquencyGoBackToZero) + camBasicData.CurvesAndValues[i].OffsetValue;
            }
        }
        return tReturnedValues;
    }

    /// <summary>
    /// Permet de changer la frequence du headbobing
    /// </summary>
    /// <param name="Frequency"></param>
    /// <param name="Acceleration"></param>
    public void ChangeSpeedMoving(float Frequency, float Acceleration = -1)
    {
        fAimedFrenquency = Frequency;
        if (Acceleration > 0)
            fFrequencyAccel = Acceleration;
    }

    public void TriggerAnim(string animString, float animDuration)
    {
        onCinemachineCamera = false;
        AnimatedCam.GetComponent<Animator>().SetTrigger(animString);
        timerOnCinemachineCam = animDuration;
    }

}
