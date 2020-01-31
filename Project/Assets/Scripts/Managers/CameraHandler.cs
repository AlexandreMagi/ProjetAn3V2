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
    float transitionSpeed = 0; // Vitesse de transition

    Camera camRef = null; // Caméra ref pour les calculs

    float fovAddedByTimeScale = 0; // Fov actuel affecté par le time scale

    float frequency = 0; // Vitesse actuelle du headbobing
    float aimedFrequency = 0; // Vitesse visée
    float frequencyTransitionSpeed = 1; // Vitesse de transition vers la vitesse visée
    Vector2[] curveValues = new Vector2 [0]; // Stock des valeurs des curves

    float dt = 0;

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
        // Desactivation des caméra animated et cinemachine pour passer sur la render cam
        if (cinemachineCam) cinemachineCam.enabled = false;
        else Debug.LogWarning("BUG : Camera isn't setup in CameraHandler");
        if (animatedCam) animatedCam.enabled = false;
        else Debug.LogWarning("BUG : Camera isn't setup in CameraHandler");
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

        ResyncCamera(); // Place les dummies
    }

    public void SetWeapon(DataWeapon _weaponData) { weaponData = _weaponData; }

    #endregion

    #region UpdateFunctions

    private void Update()
    {
        dt = camData.independentFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        UpdateCamValues();
    }

    private void UpdateCamDummyValues()
    {
        camDelayRotDummyParent.transform.position = currentCamRef.transform.position;
        camDelayRotDummyParent.transform.rotation = Quaternion.Lerp(camDelayRotDummyParent.transform.rotation, currentCamRef.transform.rotation, dt * camData.speedRotFollow);

        camDelayPosDummy.transform.position = Vector3.Lerp(camDelayPosDummy.transform.position, camDelayRotDummy.transform.position, dt * camData.speedPosFollow);
    }

    private void UpdateCamValues()
    {
        // Fait le switch entre la caméra cinemachine et la caméra animée
        currentCamRef = !currentCamIsCine && animatedCam.transform.position != Vector3.zero && animatedCam != null ? animatedCam : cinemachineCam;

        // Init
        camRef.transform.position = currentCamRef.transform.position;
        camRef.transform.rotation = currentCamRef.transform.rotation;

        renderingCam.transform.position = camRef.transform.position;
        renderingCam.transform.rotation = camRef.transform.rotation;

        UpdateCamDummyValues();

        camRef.transform.rotation = Quaternion.LookRotation(camDelayPosDummy.transform.position - currentCamRef.transform.position, Vector3.up); // Regard de la cam


    }

    #endregion

    /// <summary>
    /// Permet de resynchroniser la caméra entierement
    /// </summary>
    public void ResyncCamera()
    {
        renderingCam.fieldOfView = camData.BaseFov;

        camDelayRotDummyParent.transform.position = renderingCam.transform.position;
        camDelayRotDummyParent.transform.rotation = renderingCam.transform.rotation;

        camDelayRotDummy.transform.position = camDelayRotDummyParent.transform.position + camDelayRotDummyParent.transform.forward * camData.distanceBetweenDummy;

        camDelayPosDummy.transform.position = camDelayRotDummy.transform.position;

        // Fait le switch entre la caméra cinemachine et la caméra animée
        currentCamRef = !currentCamIsCine && animatedCam.transform.position != Vector3.zero && animatedCam != null ? animatedCam : cinemachineCam;

        renderingCam.transform.position = currentCamRef.transform.position;
        renderingCam.transform.rotation = currentCamRef.transform.rotation;
    }


    #region CamEffectsFunctions

    public void AddRecoil (bool fovType, float value) { }
    public void AddShake (float value) { shakeSource.GenerateImpulse(Vector3.up * value); }
    public void AddShake (float value, Vector3 initPos) { }

    #endregion

    public void DecalCamWithCursor (Vector2 Pos) { cursorRotateValue = new Vector2(-(Pos.y * (camData.camMoveWithAim * 2) / Screen.height - camData.camMoveWithAim), Pos.x * (camData.camMoveWithAim * 2) / Screen.width - camData.camMoveWithAim); }

    #region SequencerFunctions

    public void FeedbackTransition(bool enabled, bool transition, float speed) { }
    public void CameraLookAt(Transform camFollow, float _timeTransitionTo, float _timeTransitionBack, float timerBeforeGoBack = -1) { }
    public void UpdateCamSteps (float frequency, float transitionSpeed = -1) { }
    public void TriggerAnim (string animName, float animDuration) { }

    #endregion

}
