﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataPostEffect", menuName = "ScriptableObjects/DataPostEffect")]
public class DataPostEffect : ScriptableObject
{
    [Header("Chroma")]
    public bool chromaChangeDependentFromTimeScale = true;
    public float chromaTimeTransition = 1;
    public float chromaMax = 1;
    public float chromaMin = 0;

    [Header("Depth Of Field")]
    public bool activateDof = false;
    public bool dofDependentFromTimeScale = true;
    public LayerMask lmask = 0;
    public float transitionSpeed = 5;

    [Header("Lens Distortion")]
    public bool distortionDependentFromTimeScale = true;
    public AnimationCurve animDistortionAtOrb = AnimationCurve.Linear(0, 0, 1, 0);
    public float animDistortionDuration = 1;
    public float animDistortionMultiplier = 1;
    public float maxDistToFade = 15;
}
