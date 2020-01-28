using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataLifeBarUi")]
public class DataLifeBarUi : ScriptableObject
{

    [Header("Capsule sprite display")]
    public Sprite capsuleSprite;
    public float purcentageUsedY = 0.8f;
    public Vector2 decalSprites = new Vector2 (0.9f, 0.1f);

    public float startBps = 2f;
    public float maxBps = 4f;
    public float addedBpsShield = 1.5f;
    public float addedBps = 2;
    public float recoverBps = 0.2f;

    public float holaFeedbackTime = 1;
    public float holaScaleMultiplier = 0.2f;
    public AnimationCurve holaEffectOnBullet = AnimationCurve.Linear(0, 0, 1, 1);

    public Vector2 baseSize = new Vector2(70, 50);
    public float sizeDesac = 0.3f;
    public float outlineSize = 5;

    [Header("Anim quand prend dégats")]
    public AnimationCurve takeDamageScaleAnim = AnimationCurve.Linear(0, 0, 1, 1);
    public float scaleAnimValue = 1f;
    public float scaleAnimTime = 0.5f;
    public float speedDesactivate = 5;
    public float sizeWhenJustHited = 2;

    [Header("Colors")]
    public Color lifeCapsuleColor = Color.red;
    public Color lifeEmptyCapsuleColor = Color.red;
    public Color hitedColor = Color.white;

    public float idleSpeed = 7;
    public float idleMagnitude = 0.3f;
    public float idleDecal = 0;
}

