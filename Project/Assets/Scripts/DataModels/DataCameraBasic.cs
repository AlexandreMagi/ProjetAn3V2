using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataCameraBasic")]
public class DataCameraBasic : ScriptableObject
{

    [Header("Camera Shakes")]
    public float distanceShakeCancelled = 30;
    public float shakeAtCharged = 1;
    public float shakeAtEndOfAnimation = 1;
    public float shakeWhenCharged = 10;
    public float shakeWhenCharging = 8;


    [Header("Smooth transitions")]
    [Tooltip("Plus la valeur est haute, moins elle sera smooth")]
    public float camFollowSpeed = 5;

    public float camTransitionSpeedAnimatedCine = 3;
    public float camSafeDistanceTransition = 1;





    // Variables

    [Header("Cam Dummy Parameters")]
    public float distanceBetweenDummy = 5;
    public float speedRotFollow = 5;
    public float speedPosFollow = 5;
    public bool followRotDummy = false;

    [Header("Rotations")]
    [PropertyRange(0f, 10f)]
    public float camMoveWithAim;
    [Tooltip("Plus la valeur est haute, plus la caméra va rotate lors des mouvements latéraux")]
    public float maxRotateWhileMoving = 5;
    public float lerpOnLerpBecauseWhyTheFuckNot = 5;

    [Header("Fov camera")]
    [Tooltip("Valeur de FOV de base")]
    public float BaseFov = 70;
    [Tooltip("Valeur de FOV en fonction de la vitesse")]
    public float fovMultiplier = 5f;
    [Tooltip("Valeur de transition entre FOVs")]
    public float fovSpeed = 1;
    public float maxFovDecal = 2;
    public float timeScaleFovImpact = 5;
    public float timeScaleFovSpeed = 5;
    [Tooltip("Anim fov Arme")]
    [PropertyRange(0, 1)]
    public float transitionStartAt = 0.4f;

    [Header("Step options")]
    public DataStep[] CurvesAndValues;
    [Tooltip("Valeur de detection d'arret de mouvement")]
    public float frenquencyGoBackToZero = 1;
    [Tooltip("Valeur de decceleration")]
    public float frequencyDeccel = 0.02f;
    [PropertyRange(0.1f, 0.9f)]
    public float stepSoundPlay = 0.7f;

    [Header("Recul caméra")]
    public float RecoilMaxValue = 1.5f;
    public float RecoilRecover = 5;
    public float RecoilPow = 2;
    public float RecoilLerpSpeed = 8;

    public float maxFovRecoilValue = 10;
    public float fovRecoilRecover = 5;
    public float fovRecoilPow = 2;
    public float fovRecoilLerpSpeed = 8;

    [Header("Others")]
    public bool independentFromTimeScale = true;

    [Header("Idle")]
    public AnimationCurve idleCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float idleAmplitude = 0.5f;
    public float idleTime = 0.5f;
    public float minimumIdleTransition = 1;

    [Header("Stop automatique des Steps et Idle")]
    public float distanceFadeStepsAtEnd = 1f;

}
