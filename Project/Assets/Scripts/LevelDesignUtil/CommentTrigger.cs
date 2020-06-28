using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentTrigger : MonoBehaviour
{

    public enum typeOfComment { entreeFinalBattle, introAtterissage,introCouloir,minigun,salleSecrete,tourDeVerre,trap,win}
    [SerializeField] typeOfComment comment = typeOfComment.entreeFinalBattle;
    [SerializeField] bool canPlay = true;
    [SerializeField] float delay = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (canPlay && Main.Instance.EnableComments)
        {
            switch (comment)
            {
                case typeOfComment.entreeFinalBattle:
                    Main.Instance.PlaySoundWithDelay("PresA_Entree_Final_Battle"    , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Entree_Final_Battle"    , "Comment", Main.Instance.CommentBVolume, delay + 2.0f);
                    break;
                case typeOfComment.introAtterissage:
                    Main.Instance.PlaySoundWithDelay("PresA_Intro_Atterrissage"     , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Intro_Atterrissage"     , "Comment", Main.Instance.CommentBVolume, delay + 4.5f);
                    break;
                case typeOfComment.introCouloir:
                    Main.Instance.PlaySoundWithDelay("PresA_Intro_Couloirs"         , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Intro_Couloirs"         , "Comment", Main.Instance.CommentBVolume, delay + 3.5f);
                    break;
                case typeOfComment.minigun:
                    Main.Instance.PlaySoundWithDelay("PresA_Minigun"                , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Minigun"                , "Comment", Main.Instance.CommentBVolume, delay + 2.3f);
                    break;
                case typeOfComment.salleSecrete:
                    Main.Instance.PlaySoundWithDelay("PresA_Salle_Secrete"          , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Salle_Secrete"          , "Comment", Main.Instance.CommentBVolume, delay + 1.5f);
                    break;
                case typeOfComment.tourDeVerre:
                    Main.Instance.PlaySoundWithDelay("PresA_Tour_De_Verre"          , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Tour_De_Verre"          , "Comment", Main.Instance.CommentBVolume, delay + 2.2f);
                    break;
                case typeOfComment.trap:
                    Main.Instance.PlaySoundWithDelay("PresA_Trap"                   , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Trap"                   , "Comment", Main.Instance.CommentBVolume, delay + 2.2f);
                    break;
                case typeOfComment.win:
                    Main.Instance.PlaySoundWithDelay("PresA_Trap"                   , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Trap"                   , "Comment", Main.Instance.CommentBVolume, delay + 3.5f);
                    break;
                default:
                    break;
            }
        }
    }



}
