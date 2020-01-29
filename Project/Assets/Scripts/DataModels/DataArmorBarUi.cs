using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataArmorBarUi")]
public class DataArmorBarUi : ScriptableObject
{

    [Header("Capsule sprite display")]
    public Sprite capsuleSprite;

    public Vector2 baseSize = new Vector2(14, 300);
    public float outlineSize = 5;
    public float deadSize = 15;

    [Header("Anim quand prend dégats")]
    public AnimationCurve takeDamageScaleAnim = AnimationCurve.Linear(0, 0, 1, 1);
    public float scaleAnimValue = 1f;
    public float scaleAnimTime = 0.5f;
    public float sizeWhenJustHited = 2;

    public float speedDieAnim = 8;

    [Header("Colors")]
    public Color baseColor = Color.cyan;
    public Color lowLifeColor = new Color(1, 1, 1, .8f);
    public Color hitedColor = Color.white;
    public Color deadColor = Color.white;

    public float idleSpeed = 7;
    public float idleMagnitude = 0.3f;
    public float idleDecal = 0;
}

