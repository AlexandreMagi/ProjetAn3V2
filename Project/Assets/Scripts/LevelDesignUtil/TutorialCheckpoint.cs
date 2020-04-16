using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCheckpoint : MonoBehaviour
{

    // --- Acces depuis les scripts extérieurs (singleton)
    public static TutorialCheckpoint Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    DataTutorialCheckpoint checkpointData = null; // Data du tuto
    float purcentageAccomplission = 0; // Quand cette valeur arrive à 1, la/les actions sont considérées comme finies
    float endCheckpointTimer = 0; // Timer entre la réussite et le next séquence
    float timerBeforeNextForcedAction = 0; // Temps restant avant la prochaine action forcée
    bool mustForceAction = true; // Dit si le checkpoint doit forcer une action
    float dt = 0; // Delta time actuel du checkpoint

    #region Init and End

    /// <summary>
    /// Lance un checkpoint tutoriel
    /// </summary>
    /// <param name="_checkpointData"></param>
    public void InitTutorialCheckpoint (DataTutorialCheckpoint _checkpointData)
    {
        Debug.Log("Init Checkpoint");
        checkpointData = _checkpointData;
        purcentageAccomplission = 0;
        endCheckpointTimer = checkpointData.timerBetweenSuccesAndNextSequence;

        foreach (DataTutorialCheckpoint.playerActions desactivation in checkpointData.actionsPlayerCantDo)
        {
            switch (desactivation)
            {
                case DataTutorialCheckpoint.playerActions.shoot:
                    Main.Instance.SetControlState(TriggerSender.Activable.BaseWeapon, false);
                    break;
                case DataTutorialCheckpoint.playerActions.reload:
                    Main.Instance.SetControlState(TriggerSender.Activable.Reload, false);
                    break;
                case DataTutorialCheckpoint.playerActions.orb:
                    Main.Instance.SetControlState(TriggerSender.Activable.Orb, false);
                    break;
                case DataTutorialCheckpoint.playerActions.autoReload:
                    Main.Instance.SetControlState(TriggerSender.Activable.AutoReload, false);
                    break;
                case DataTutorialCheckpoint.playerActions.perfectReload:
                    Main.Instance.SetControlState(TriggerSender.Activable.PerfectReload, false);
                    break;
                case DataTutorialCheckpoint.playerActions.shotgun:
                    Main.Instance.SetControlState(TriggerSender.Activable.Shotgun, false);
                    break;
                case DataTutorialCheckpoint.playerActions.zeroG:
                    Main.Instance.SetControlState(TriggerSender.Activable.ZeroG, false);
                    break;
            }
        }
        foreach (DataTutorialCheckpoint.playerActions activations in checkpointData.actionsPlayerCanDoWhile)
        {
            switch (activations)
            {
                case DataTutorialCheckpoint.playerActions.shoot:
                    Main.Instance.SetControlState(TriggerSender.Activable.BaseWeapon, true);
                    break;
                case DataTutorialCheckpoint.playerActions.reload:
                    Main.Instance.SetControlState(TriggerSender.Activable.Reload, true);
                    break;
                case DataTutorialCheckpoint.playerActions.orb:
                    Main.Instance.SetControlState(TriggerSender.Activable.Orb, true);
                    break;
                case DataTutorialCheckpoint.playerActions.autoReload:
                    Main.Instance.SetControlState(TriggerSender.Activable.AutoReload, true);
                    break;
                case DataTutorialCheckpoint.playerActions.perfectReload:
                    Main.Instance.SetControlState(TriggerSender.Activable.PerfectReload, true);
                    break;
                case DataTutorialCheckpoint.playerActions.shotgun:
                    Main.Instance.SetControlState(TriggerSender.Activable.Shotgun, true);
                    break;
                case DataTutorialCheckpoint.playerActions.zeroG:
                    Main.Instance.SetControlState(TriggerSender.Activable.ZeroG, true);
                    break;
            }
        }

        if (checkpointData == null || checkpointData.CheckpointForceTo == DataTutorialCheckpoint.actionToDo.nothing)
        {
            mustForceAction = false;
            timerBeforeNextForcedAction = 0;
        }
        else
        {
            timerBeforeNextForcedAction = checkpointData.timeBeforeFirstAction;
            mustForceAction = (checkpointData.timeBeforeFirstAction == 0);
        }

    }

    /// <summary>
    /// Fin du checkpoint tutoriel
    /// </summary>
    public void EndTutorialCheckpoint()
    {
        Debug.Log("End Checkpoint");
        foreach (DataTutorialCheckpoint.playerActions activation in checkpointData.actionsPlayerCanDoAfter)
        {
            switch (activation)
            {
                case DataTutorialCheckpoint.playerActions.shoot:
                    Main.Instance.SetControlState(TriggerSender.Activable.BaseWeapon, true);
                    break;
                case DataTutorialCheckpoint.playerActions.reload:
                    Main.Instance.SetControlState(TriggerSender.Activable.Reload, true);
                    break;
                case DataTutorialCheckpoint.playerActions.orb:
                    Main.Instance.SetControlState(TriggerSender.Activable.Orb, true);
                    break;
                case DataTutorialCheckpoint.playerActions.autoReload:
                    Main.Instance.SetControlState(TriggerSender.Activable.AutoReload, true);
                    break;
                case DataTutorialCheckpoint.playerActions.perfectReload:
                    Main.Instance.SetControlState(TriggerSender.Activable.PerfectReload, true);
                    break;
                case DataTutorialCheckpoint.playerActions.shotgun:
                    Main.Instance.SetControlState(TriggerSender.Activable.Shotgun, true);
                    break;
                case DataTutorialCheckpoint.playerActions.zeroG:
                    Main.Instance.SetControlState(TriggerSender.Activable.ZeroG, true);
                    break;
            }
        }
        checkpointData = null;
        SequenceHandler.Instance.NextSequence();
    }

    #endregion

    void Update()
    {
        if (checkpointData != null)
        {
            dt = checkpointData.independentFromTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            HandleForcedAction();
            HandleNextSequence();
        }
    }

    void HandleForcedAction()
    {
        // --- Timer
        if (timerBeforeNextForcedAction > 0)
        {
            timerBeforeNextForcedAction -= dt;
            if (timerBeforeNextForcedAction < 0)
            {
                mustForceAction = true;
                timerBeforeNextForcedAction = 0;
            }
        }

        // --- Execution de l'action forcée
        if (mustForceAction && purcentageAccomplission < 1)
        {
            //Debug.Log("Action forced");
            mustForceAction = false;
            // --- Action forcée
            switch (checkpointData.CheckpointForceTo)
            {
                case DataTutorialCheckpoint.actionToDo.nothing:
                    break;
                case DataTutorialCheckpoint.actionToDo.reload:
                    Weapon.Instance.ReloadingInput();
                    break;
                case DataTutorialCheckpoint.actionToDo.changeNbBullet:
                    Weapon.Instance.SetBulletAmmount(checkpointData.nbBullet, false);
                    break;
            }
            // --- Dit si l'action doit être rejouée
            if (checkpointData.CheckpointForceTo != DataTutorialCheckpoint.actionToDo.nothing && checkpointData.forcedActionLoops)
                timerBeforeNextForcedAction = checkpointData.timeBetweenActions;

        }
    }

    void HandleNextSequence()
    {
        if (checkpointData.endSequenceBy == DataTutorialCheckpoint.howToEnd.timer) purcentageAccomplission += dt / checkpointData.timerToEndSequence / (float)checkpointData.nbActionsNecessary;

        if (purcentageAccomplission >= 1)
        {
            if (endCheckpointTimer > 0)
            {
                endCheckpointTimer -= dt;
                if (endCheckpointTimer < 0)
                {
                    endCheckpointTimer = 0;
                    EndTutorialCheckpoint();
                }
            }
        }
    }

    public void PlayerReloaded (bool perfectReload)
    {
        if (checkpointData != null)
        {
            Debug.Log("Player reload / " + perfectReload + " /  Needed is " + checkpointData.endSequenceBy);
            if (checkpointData.endSequenceBy == DataTutorialCheckpoint.howToEnd.reload || checkpointData.endSequenceBy == DataTutorialCheckpoint.howToEnd.perfectReload && perfectReload)
                purcentageAccomplission += 1 / (float)checkpointData.nbActionsNecessary;
        }
        Debug.Log(purcentageAccomplission);
    }

    public void PlayerUsedOrb()
    {
        if (checkpointData != null)
        {
            Debug.Log("Player used orb /  Needed is " + checkpointData.endSequenceBy);
            if (checkpointData.endSequenceBy == DataTutorialCheckpoint.howToEnd.orbLaunched)
                purcentageAccomplission += 1 / (float)checkpointData.nbActionsNecessary;
        }
        Debug.Log(purcentageAccomplission);
    }

    public void PlayerUsedZeroG()
    {
        if (checkpointData != null)
        {
            Debug.Log("Player used zero g /  Needed is " + checkpointData.endSequenceBy);
            if (checkpointData.endSequenceBy == DataTutorialCheckpoint.howToEnd.orbReactivated)
                purcentageAccomplission += 1 / (float)checkpointData.nbActionsNecessary;
        }
        Debug.Log(purcentageAccomplission);
    }

    public void PlayerUsedShotGun()
    {
        if (checkpointData != null)
        {
            Debug.Log("Player used shotgun /  Needed is " + checkpointData.endSequenceBy);
            if (checkpointData.endSequenceBy == DataTutorialCheckpoint.howToEnd.shotgun)
                purcentageAccomplission += 1 / (float)checkpointData.nbActionsNecessary;
        }
        Debug.Log(purcentageAccomplission);
    }
}
