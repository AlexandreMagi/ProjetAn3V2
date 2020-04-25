using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DataTutorialCheckpoint", menuName = "ScriptableObjects/DataTutorialCheckpoint")]
public class DataTutorialCheckpoint : ScriptableObject
{


    //###################################################################################################################################//


    [Header("Action forcée")]

    [Tooltip("L'action que le checkpoint va forcer sur le joueur")] public actionToDo CheckpointForceTo = actionToDo.nothing;
    public enum actionToDo { nothing, reload, changeNbBullet }
    [HideIf("CheckpointForceTo", actionToDo.nothing), Tooltip("Dis si l'action se fait en boucle")] public bool forcedActionLoops = false;
    [HideIf("CheckpointForceTo", actionToDo.nothing), Tooltip("Temps avant la première action")] public float timeBeforeFirstAction = 3;
    [HideIf("CheckpointForceTo", actionToDo.nothing), ShowIf("forcedActionLoops"), Tooltip("Temps entre chaque action")] public float timeBetweenActions = 3;
    [ShowIf("CheckpointForceTo", actionToDo.changeNbBullet), Tooltip("Nombre de balle fixé")] public int nbBullet = 0;



    //###################################################################################################################################//


    [Header("Comment finir la séquence")]

    [Tooltip("Action qui permet de passer à la prochaine séquence")] public howToEnd endSequenceBy = howToEnd.timer;
    public enum howToEnd { timer, reload, perfectReload, orbLaunched, orbReactivated, shotgun }
    [Tooltip("Nombre de répétitions de l'action nécessaire")] public int nbActionsNecessary = 1;
    [Tooltip("Temps apres l'action pour changer de séquence")] public float timerBetweenSuccesAndNextSequence = 0;

    [ShowIf("endSequenceBy", howToEnd.timer), Tooltip("Temps pour une action Timer")] public float timerToEndSequence = 5;
    [Tooltip("Indique si le checkpoint fait passer à la prochaine séquence si accomplis")] public bool nextSequenceOnEnd = true;



    //###################################################################################################################################//


    [Header("Empeche et permet le joueur de faire des trucs")]

    [Tooltip("Actions que le joueur ne peut pas faire pendant le tuto")] public List<playerActions> actionsPlayerCantDo = new List<playerActions>();
    [Tooltip("Actions que le joueur peut faire pendant le tuto")] public List<playerActions> actionsPlayerCanDoWhile = new List<playerActions>();
    [Tooltip("Actions que le joueur peut faire après le tuto")] public List<playerActions> actionsPlayerCanDoAfter = new List<playerActions>();
    public enum playerActions { shoot, orb, reload, autoReload, perfectReload, shotgun, zeroG }



    //###################################################################################################################################//


    [Header("Pop de Hint")]

    public bool popHintAfterTimer = false;
    [ShowIf("popHintAfterTimer")]
    public float timeBeforePopHint = 5;
    [ShowIf("popHintAfterTimer")]
    public bool hintTimerTimeScaled = false;
    [ShowIf("popHintAfterTimer")]
    public string hintText = "Enter Hint Text";



    //###################################################################################################################################//


    [Header("Autres")]

    public bool independentFromTimeScale = false;

}
