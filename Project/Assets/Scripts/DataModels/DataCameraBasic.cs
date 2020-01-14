using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataCameraBasic")]
public class DataCameraBasic : ScriptableObject
{
    [Header ("Recul caméra")]
    public float RecoilMaxValue = 1.5f;
    public float RecoilRecover = 5;
    public float RecoilPow = 2;

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

    [Header("Smooth transitions")]
    [Tooltip("Plus la valeur est haute, moins elle sera smooth")]
    public float camFollowSpeed = 5;
    [Tooltip("Plus la valeur est haute, plus la caméra va rotate lors des mouvements latéraux")]
    public float maxRotateWhileMoving = 5;

    [Header("Step options")]
    public DataStep[] CurvesAndValues;
    [Tooltip("Valeur de detection d'arret de mouvement")]
    public float frenquencyGoBackToZero = 1;
    [Tooltip("Valeur de decceleration")]
    public float frequencyDeccel = 0.02f;
    [PropertyRange(0.1f, 0.9f)]
    public float stepSoundPlay = 0.7f;
}
