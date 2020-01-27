using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataLifeBarUi")]
public class DataLifeBarUi : ScriptableObject
{

    [Header("Bullet sprite display")]
    public Sprite capsuleSprite;
    public float purcentageUsedY = 0.8f;
    public Vector2 decalSprites = new Vector2 (0.9f, 0.1f);

    public float holaFeedbackTime = 1;
    public float holaRange = 1;
    public float holaScaleMultiplier = 0.2f;
    public AnimationCurve holaEffectOnBullet = AnimationCurve.Linear(0, 0, 1, 1);

    public float baseSize = 100;

    [Header("Anim quand prend dégats")]
    public AnimationCurve takeDamageScaleAnim = AnimationCurve.Linear(0, 0, 1, 1);
    public float scaleAnimValue = 1f;
    public float scaleAnimTime = 0.5f;

    [Header("Colors")]
    public Color lifeCapsuleColor = Color.red;
    public Color lifeEmptyCapsuleColor = Color.red;

    public float idleSpeed = 7;
    public float idleMagnitude = 0.3f;
}

