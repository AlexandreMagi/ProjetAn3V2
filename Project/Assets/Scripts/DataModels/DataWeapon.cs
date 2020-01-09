using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DataWeapon")]
public class DataWeapon : ScriptableObject
{

    [Header("Nom du preset (en majuscule)")]
    public string PresetName = "BASE WEAPON";

    [Header("Presets")]
    [Tooltip("Façon de tirer du joueur")]
    public DataWeaponMod mplayerMod = null;
    [Tooltip("Balle tirée par le joueur")]
    public DataBullet mbulletMod = null;
    //[Tooltip("Preset de curseur d'ui")]
    //public M_Ui muiMod = null;
    [Tooltip("Type d'orbe lancé (en lien avec l'arme)")]
    public DataGravityOrb morbeMod = null;

    [Header("Sprites UI liés / doit être en 100 par 100")]
    [Tooltip("Sprite Un, aussi appelé point")]
    public Sprite SpriteOne = null;
    [Tooltip("Sprite Two, aussi appelé circle")]
    public Sprite SpriteTwo = null;
    [Tooltip("Sprite Three, aussi appelé stright")]
    public Sprite SpriteThree = null;
    [Tooltip("Sprite affiché en tant que logo de l'arme")]
    public Sprite SpriteLogo = null;

    [Header("Sons de l'arme (en string) ps:samarchpa")]
    [Tooltip("Son de émit par l'arme lors d'un tir")]
    public string ShootSound = null;
    [Tooltip("Son de émit par l'arme lors du rechargement")]
    public string ReloadSound = null;
    [Tooltip("Son de émit par l'arme lorsque le joueur essaie de tirer lors qu'il n'a plus de balle")]
    public string EmptySound = null;

}
