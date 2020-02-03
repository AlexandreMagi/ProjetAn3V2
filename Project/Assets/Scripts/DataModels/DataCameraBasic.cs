using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataCameraBasic")]
public class DataCameraBasic : ScriptableObject
{
    [PropertyRange(0f, 10f)]
    public float camMoveWithAim;
    [Header ("Recul caméra")]
    public float RecoilMaxValue = 1.5f;
    public float RecoilRecover = 5;
    public float RecoilPow = 2;
    public float RecoilLerpSpeed = 8;

    public float maxFovRecoilValue = 10;
    public float fovRecoilRecover = 5;
    public float fovRecoilPow = 2;

    [Header("Camera Shakes")]
    public float distanceShakeCancelled = 30;
    public float shakeAtCharged = 1;
    public float shakeAtEndOfAnimation = 1;
    public float shakeWhenCharged = 10;
    public float shakeWhenCharging = 8;

    [Header("Fov caméra")]
    [Tooltip("Valeur de FOV de base")]
    public float BaseFov = 70;
    [Tooltip("Valeur de FOV en fonction de la vitesse")]
    public float fovMultiplier = 5f;
    [Tooltip("Valeur de transition entre FOVs")]
    public float fovSpeed = 1;
    public float maxFovDecal = 10;
    [Tooltip("Valeur de sécurité pour transition")]
    [PropertyRange(0, 1)]
    public float transitionStartAt = 0;
    public float timeScaleFov = 5;
    public float timeScaleFovSpeed = 5;

    [Header("Smooth transitions")]
    [Tooltip("Plus la valeur est haute, moins elle sera smooth")]
    public float camFollowSpeed = 5;
    [Tooltip("Plus la valeur est haute, plus la caméra va rotate lors des mouvements latéraux")]
    public float maxRotateWhileMoving = 5;

    public float camTransitionSpeedAnimatedCine = 3;
    public float camSafeDistanceTransition = 1;

    [Header("Step options")]
    public DataStep[] CurvesAndValues;
    [Tooltip("Valeur de detection d'arret de mouvement")]
    public float frenquencyGoBackToZero = 1;
    [Tooltip("Valeur de decceleration")]
    public float frequencyDeccel = 0.02f;
    [PropertyRange(0.1f, 0.9f)]
    public float stepSoundPlay = 0.7f;

    [Header("Cam Dummy Parameters")]
    public float distanceBetweenDummy = 5;
    public float speedRotFollow = 5;
    public float speedPosFollow = 5;

    [Header("Others")]
    public bool independentFromTimeScale = true;
}
