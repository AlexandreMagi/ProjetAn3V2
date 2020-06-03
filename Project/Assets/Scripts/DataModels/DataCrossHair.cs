using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataCrossHair")]
public class DataCrossHair : ScriptableObject
{
    [Header("Base parameters")]
    public bool affectedByTimeScale = false;
    public float baseSize = 100;
    public float amplitudeIdle;
    public float speedIdle;
    public float offsetIdle;
    public Color baseColor = Color.white;
    public Color outlineBaseColor = Color.black;
    public float rotateSpeed;
    [PropertyRange(-1, 1)]
    public int rotateDir = 1;
    public float startRotation;
    public Vector2 randomRotateAtStart;
    public Vector2 offset;
    public bool resetRotationOnRelease = false;

    [Header("Anim quand Tir")]
    public float reculRecoverRate;
    public float reculAddedPerShot;
    [PropertyRange(0.01f, 100f)]
    public float reculMax = 0.01f;
    public float reculSizeMultiplier;

    [Header("Anim quand Hit")]
    public float hitRecoverRate;
    public float hitAddedPerShot;
    [PropertyRange(0.01f, 100f)]
    public float hitMax = 0.01f;
    public float hitSizeMultiplier;
    public Color hitMaxColor = Color.white;
    public Color outlineHitMaxColor = Color.black;

    [Header("Transition de normal à chargé")]
    [PropertyRange(0, 1)]
    public float transitionStartAt = 0;
    public float chargingSize;
    public float chargingAmplitudeIdle;
    public float chargingSpeed;
    public float chargingRotation;
    public Color chargingColor = Color.white;
    public Color outlineChargingColor = Color.black;
    public float chargingRotateSpeed;
    public Vector2 chargingOffset;

    [Header("Quand pas de balle")]
    public float noBulletSize;
    public float noBulletAmplitudeIdle;
    public float noBulletIdleSpeed;
    public Color noBulletColor = Color.white;
    public Color noBulletOutlineColor = Color.black;
    public float noBulletAnimTime = 2;

    [Header("État quand chargé")]
    public Color chargedColor = Color.white;
    public Color outlineChargedColor = Color.black;
    public float chargedRotateSpeed;

    [Header("Changement quand orb chargée")]
    public bool reactToOrb = false;
    public Color orbChargedColor = Color.white;
    public Color outlineOrbChargedColor = Color.black;
    public float orbBonusSize;
    public float orbGrowTime = 0.01f;
    public float orbShrinkTime = 0.01f;
    public float orbIdleSpeed;
    public float orbIdleAmplitude;

    [Header("Animation quand chargé")]
    public float timeToDoChargedFeedback = 1;
    public AnimationCurve sizeAnim = AnimationCurve.Linear(0, 0, 1, 0);
    public float sizeMultiplier = 1;

    [Header("Changement quand overlap bullet affect")]
    public bool reactAtOverlap = false;
    [ShowIf("reactAtOverlap")] public float sizeMultiplierByOverlap = 1;
    [ShowIf("reactAtOverlap")] public Color colorAtOverlap = Color.red;
    [ShowIf("reactAtOverlap")] public Color outlineColorAtOverlap = Color.red;
    [ShowIf("reactAtOverlap")] public float overlapLerpSpeed = 12;

    [Header("Pop")]
    public activatedIf crosshairPopsWhen = activatedIf.start;
    public enum activatedIf { start, reload, reloadPerfect, shotgun, orb, zeroG }

    public float popTime = 1f;

    [Header("Changement minigun")]
    public bool isAlwaysActivated = false;
    [HideIf("isAlwaysActivated")] public bool isActivatedAtMinigun = false;
    public float minigunTimeTransition = 1;
    [ShowIf("isActivatedAtMinigun")] public float rotationMultiplier = 0;
    [ShowIf("isActivatedAtMinigun")] public float sizeAddedMultiplier = 0;

}


