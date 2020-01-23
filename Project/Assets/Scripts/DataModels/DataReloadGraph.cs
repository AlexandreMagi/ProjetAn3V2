using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataReloadGraph")]
public class DataReloadGraph : ScriptableObject
{

    [Header("Bullet sprite display")]
    public Sprite bulletSprite;
    public float purcentageUsedY = 0.8f;
    public Vector2 decalBulletSprites = new Vector2 (0.9f, 0.1f);
    public float spaceEveryThreeBullet = 0.05f;

    public float holaFeedbackTime = 1;
    public float holaRange = 1;
    public float holaScaleMultiplier = 0.2f;
    public AnimationCurve holaEffectOnBullet = AnimationCurve.Linear(0, 0, 1, 1);

    public float baseSize = 100;

    public float bulletFallSpeep = 3;

    [Header("Bullet remainings feedback")]
    public AnimationCurve scaleAnimBulletTextShot = AnimationCurve.Linear(0, 0, 1, 1);
    public float scaleAnimBulletValue = 1f;
    public float scaleAnimBulletTime = 0.5f;
    public int shortNumberOfBullet = 5;
    public Color shortOnBulletColor = Color.red;
    public int midNumberOfBullet = 5;
    public Color midOnBulletColor = Color.red;
    public Color highOnBulletColor = Color.white;
    public Color noBulletColor = Color.black;
    public float scaleIfNoBullet = 0.2f;
    public float scaleEmptySpeed = 1;
    public float scaleRecoverSpeed = 3;

    [Header("Colors")]
    public Color barColor = Color.white;
    public Color extremityColor = Color.white;
    public Color checkBarColor = Color.green;
    public Color checkBarColorFailed = Color.red;
    public Color perfectSpotColor = Color.blue;

    [Header("Reload Anim")]
    public AnimationCurve verticalScaleAnimAatSpawn = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve horizontalScaleAnimAatSpawn = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve scaleAnimOnPerfectIndicator = AnimationCurve.Linear(0, 0, 1, 1);
    public float animDuration = 0.2f;

    public float idleSpeed = 7;
    public float idleMagnitude = 0.3f;
    public float reducingTime = 0.2f;
    public float perfectAnimScaleMultiplier = 1;
    public float perfectAnimtime = 0.2f;
}

