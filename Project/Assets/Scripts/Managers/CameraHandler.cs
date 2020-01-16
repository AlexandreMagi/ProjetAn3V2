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

    #region VAR

    [Header("Cameras")]
    [Tooltip("Dummy qui bouge sur lequel va se fixer la cam")]
    public GameObject CamDummy = null;
    [Tooltip("Camera render")]
    public GameObject RenderingCam = null;

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

    #endregion

    private void Awake()
    {
        _instance = this;
        if (CamDummy.GetComponent<Camera>()) CamDummy.GetComponent<Camera>().enabled = false;
        if (RenderingCam.GetComponent<Camera>()) RenderingCam.GetComponent<Camera>().enabled = true;
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
            //CustomSoundManager.Instance.PlaySound(RenderingCam, "Step_0" + Random.Range(1, 5), false, 1f);
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


        // Reset de la position et de la rotation
        camDummyValueFeedback.transform.position = CamDummy.transform.position;
        camDummyValueFeedback.transform.rotation = CamDummy.transform.rotation;
        /// ----- GET VALUES ----- ///
        float[] camBasicDataValues = GetValue(); // Get les values des steps
        vLookAtPos = Vector3.Lerp(vLookAtPos, CamDummy.transform.position + CamDummy.transform.forward * 5, Time.deltaTime * camBasicData.camFollowSpeed); // Lerp du v3 vers la direction de la cam
        Quaternion targetRotation = Quaternion.LookRotation(vLookAtPos - CamDummy.transform.position, Vector3.up); // Regard de la cam

        /// ----- SET ROTATIONS ----- ///
        camDummyValueFeedback.transform.rotation = targetRotation; // Oriente la cam
        camDummyValueFeedback.transform.Rotate(0, vRotateValue.y, 0, Space.World); // Rotation de la cam selon le placement du curseur (en Y)
        camDummyValueFeedback.transform.Rotate(vRotateValue.x, 0, 0, Space.Self); // Rotation de la cam selon le placement du curseur (en X)

        // Setup position selon recul
        camDummyValueFeedback.transform.Translate(Vector3.back * currentRecoil, Space.Self);
        // Get de la valeur de rotation selon les mouvements horizontaux
        float vRotYByPosition = CamDummy.GetComponent<Camera>().WorldToScreenPoint(vLookAtPos).x - Screen.width / 2;
        vRotYByPosition = -vRotYByPosition * camBasicData.maxRotateWhileMoving / (Screen.width / 2);
        // Rotations de la cam selon les mouvements horizontaux + rotate en Z des steps
        camDummyValueFeedback.transform.Rotate(0, 0, -vRotYByPosition + camBasicDataValues[1], Space.Self);
        // Setup position selon steps
        camDummyValueFeedback.transform.Translate(Vector3.up * camBasicDataValues[0], Space.World);

        currentFovModif = Mathf.Lerp(currentFovModif, fFrequency * camBasicData.fovMultiplier, Time.deltaTime * camBasicData.fovSpeed);
        // Change le FOV
        float fovAddedByChargeFeedback = weaponData != null ? feedbackChargedStarted ? weaponData.AnimValue.Evaluate(currentPurcentageFBCharged) * weaponData.fovModifier : 0 : 0;
        fovAddedByTimeScale = Mathf.Lerp(fovAddedByTimeScale, camBasicData.timeScaleFov - Time.timeScale * camBasicData.timeScaleFov, Time.unscaledDeltaTime * camBasicData.timeScaleFovSpeed);
        CamDummyFov = camBasicData.BaseFov + camBasicData.maxFovDecal * chargevalue + fovAddedByChargeFeedback + currentFovModif + fovAddedByTimeScale + currentFovRecoilValue;

        if (!bFeedbckActivated)
        {
            if (bFeedbackTransition)
            {
                RenderingCam.transform.position = Vector3.Lerp(RenderingCam.transform.position, CamDummy.transform.position, Time.deltaTime * fCurrentSpeedTransition);
                RenderingCam.transform.rotation = Quaternion.Lerp(RenderingCam.transform.rotation, CamDummy.transform.rotation, Time.deltaTime * fCurrentSpeedTransition);
                RenderingCam.GetComponent<Camera>().fieldOfView = Mathf.Lerp(RenderingCam.GetComponent<Camera>().fieldOfView, CamDummy.GetComponent<Camera>().fieldOfView, Time.deltaTime * fCurrentSpeedTransition);
            }
            else
            {
                RenderingCam.transform.position = CamDummy.transform.position;
                RenderingCam.transform.rotation = CamDummy.transform.rotation;
                RenderingCam.GetComponent<Camera>().fieldOfView = CamDummy.GetComponent<Camera>().fieldOfView;
            }
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
        }
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

}
