using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies/DataSwarmer")]
public class DataSwarmer : DataEnemy
{
    [Header("Swarmer var")]
    [Tooltip("Il s'agit du \"pourcentage\" d'écart du chemin maximum que peut faire le swarmer")]
    public float varianceInPath;

    [Tooltip("Le temps qui doit s'écouler pour cibler le point de passage suivant après avoit atteint le point de passage actuel.")]
    public float timeBeforeNextPath;

    [Tooltip("La distance minimum entre le Swarmer et le Path pour que ce dernier soit \"validé\"")]
    public float distanceBeforeNextPath;
    [Tooltip("L'angle maximum que peut faire le Swarmer pour aller au Path suivant. Si l'angle est trop grand, le Swarmer va ignorer ce Path")]
    public float angleToIgnorePath = 120;

    [Tooltip("Vitesse du Swarmer")]
    public float speed;

    [Range(0f, 1f), Tooltip("Le pourcentage de la vitesse de base du swarmer quand il est dans les airs. Plus il est haut, plus la vitesse de base sera conservée.")]
    public float percentSpeedInTheAir = .7f;

    [Tooltip("Dégâts infligés au Joueur par le Swarmer")]
    public float damage;

    [Tooltip("Pourcentage de la vitesse initiale conservée à chaque frame. Plus il est haut, plus le swarmer va accélérer rapidement")]
    public float accelerationConversionRate = .1f;

    [Tooltip("Vélocité maximale pouvant être atteinte par le Swarmer")]
    public float maximumVelocity = 10f;

    [Tooltip("Distance minimum requise pour qu'une Entity puisse être considérée comme une cible")]
    public float distanceToTargetEnemy;

    [Tooltip("Distance à partir de laquelle le Swarmer peut infliger des dégâts")]
    public float distanceMelee;

    [Tooltip("Attente avant une attaque, une fois assez proche")]
    public float waitDuration = 0.3f;
    [Tooltip("Distance minimum requise pour attaquer")]
    public float distanceBeforeAttack = 6f;
    [Tooltip("Eloignement maximum de la distance minimum d'attaque pour continuer l'attaque")]
    public float distanceApproximation = 1f;
    [Tooltip("Force avec laquelle le Swarmer se propulse pour attaquer")]
    public float jumpForce = 80f;
    [Tooltip("Multiplicateur de vitesse lors d'une attaque du Swarmer")]
    public float speedMultiplierWhenAttacking = 4;

    //[Tooltip("Fait apparaître des morceaux à la mort")]
    //public bool spawnsPartsOnDeath = true;

    //public GameObject deadBody = null;

    public bool targetsPlayerAtEndOfPath = true;

    [Header("AI variables")]
    [Tooltip("Esquive des obstacles")]
    public bool hasDodgeIntelligence = true;

    [Tooltip("Portée de détection frontale des obstacles")]
    public float frontalDetectionSight = 2;
    [Tooltip("Hauteur du saut d'esquive d'obstacles")]
    public float jumpHeight = 3;
    [Tooltip("Force du saut d'esquive d'obstacles")]
    public float jumpDodgeForce = 2500;
    [Tooltip("Cooldown entre 2 sauts d'esquive")]
    public float jumpCooldownInitial = .5f;

    public float numberOfSideTries = 4;
    public float tryStep = 1;
    public float distanceDodgeStep = 1;
    public float extraLengthByStep = .2f;
    public float sideDetectionSight = 1;
    public float dodgeSlideForce = 350;

    public float timeForUpwardsTransition = .2f;
    public float maxBlockedRetryPathTime = 2f;
    public float maxBlockedSuicideTime = 4f;
    public float considerStuckThreshhold = 1f;
    public float initialTimeToConsiderCheck = .5f;

    public float maxHeightToChaseWaypoint = .3f;

    public float rayCastRangeToConsiderAirbone = .7f;

    public float pushForce = 150;
    public float upwardsPushForce = 50;

    [Header("Material")]
    public Material mat;

    public string vfxToPlayWhenPulledByGrav = "VFX_Orbe";
    public string vfxToPlayWhenHoldByGrav = "VFX_Orbe";
    public string vfxToPlayWhenReleaseByGrav = "VFX_Orbe";
    public float distanceMinWithCamToPlayVFX = 5;
    public float distanceWithCamToFadeVFX = 8;
    public float timeToChangeColorWhileAttacking = 0.5f;


    [Header("Material")]
    public float distWithPlayerToPlaySound = 12;

    [Header("Opti")]
    public float distanceRequiredToAnimate = 8;
}
