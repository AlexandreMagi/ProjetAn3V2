using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentTrigger : MonoBehaviour
{

    public enum typeOfComment { entreeFinalBattle, introAtterissage,introCouloir,minigun,salleSecrete,tourDeVerre,trap,win}
    [SerializeField] typeOfComment comment = typeOfComment.entreeFinalBattle;
    [SerializeField] bool canPlay = true;
    [SerializeField] float delay = 0;
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Random.Range(0f, 100f) < 50)
            {
                Main.Instance.PlaySoundWithDelay("PresA_Belle_Action_A", "Comment", Main.Instance.CommentAVolume, 0);
                SubtitleManager.Instance.SetSubtitle("What a play !", 0, 4, 0);
            }
            else
            {
                Main.Instance.PlaySoundWithDelay("PresA_Belle_Action_B", "Comment", Main.Instance.CommentBVolume, 0);
                SubtitleManager.Instance.SetSubtitle("Wow ! Did you see that !", 0, 4, 0);
            }
            if (Random.Range(0f, 100f) < 50)
            {
                Main.Instance.PlaySoundWithDelay("PresB_Belle_Action_A", "Comment", Main.Instance.CommentAVolume, 1);
                SubtitleManager.Instance.SetSubtitle("That was impressive !", 1, 3, 1);
            }
            else
            {
                Main.Instance.PlaySoundWithDelay("PresB_Belle_Action_B", "Comment", Main.Instance.CommentBVolume, 1.5f);
                SubtitleManager.Instance.SetSubtitle("That was worhty of Death Live !", 1, 4, 1.5f);
            }
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (canPlay && Main.Instance.EnableComments)
        {
            canPlay = false;
            switch (comment)
            {
                case typeOfComment.entreeFinalBattle:
                    Main.Instance.PlaySoundWithDelay("PresA_Entree_Final_Battle"    , "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Entree_Final_Battle"    , "Comment", Main.Instance.CommentBVolume, delay + 2.0f);
                    SubtitleManager.Instance.SetSubtitle("Our Gladiator has reached the main stage !",0, 4, 0);
                    SubtitleManager.Instance.SetSubtitle("Now the real fun begins !",1, 3, 2.2f);
                    break;
                case typeOfComment.introAtterissage:
                    Main.Instance.PlaySoundWithDelay("PresA_Intro_Atterrissage", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Intro_Atterrissage", "Comment", Main.Instance.CommentBVolume, delay + 4.5f);
                    SubtitleManager.Instance.SetSubtitle("Please give a warm welcome to our new gladiator in DEATH LIVE!",0, 5f, 0);
                    SubtitleManager.Instance.SetSubtitle("Welcome, Good luck and.. Have fun !",1, 5, 4.8f);
                    break;
                case typeOfComment.introCouloir:
                    Main.Instance.PlaySoundWithDelay("PresA_Intro_Couloirs", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Intro_Couloirs", "Comment", Main.Instance.CommentBVolume, delay + 3.2f);
                    SubtitleManager.Instance.SetSubtitle("Is this challenger ready to face the dangers of the arena ?",0, 5f, 0);
                    SubtitleManager.Instance.SetSubtitle("Let's see how they hold up against swarmers !",1, 5, 3.2f);
                    break;
                case typeOfComment.minigun:
                    Main.Instance.PlaySoundWithDelay("PresA_Minigun", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Minigun", "Comment", Main.Instance.CommentBVolume, delay + 2.3f);
                    SubtitleManager.Instance.SetSubtitle("Oh now he's packing the big gun !", 0,5f, 0);
                    SubtitleManager.Instance.SetSubtitle("Let's see some bloood !", 1,3, 2.4f);
                    break;
                case typeOfComment.salleSecrete:
                    Main.Instance.PlaySoundWithDelay("PresA_Salle_Secrete", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Salle_Secrete", "Comment", Main.Instance.CommentBVolume, delay + 0.6f);
                    SubtitleManager.Instance.SetSubtitle("Amazing ! A secret room!",0, 5f, 0);
                    SubtitleManager.Instance.SetSubtitle("Jackpot !",1, 1.5f, 1.7f);
                    break;
                case typeOfComment.tourDeVerre:
                    Main.Instance.PlaySoundWithDelay("PresA_Tour_De_Verre", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Tour_De_Verre", "Comment", Main.Instance.CommentBVolume, delay + 2f);
                    SubtitleManager.Instance.SetSubtitle("And here comes a new adversary !", 0, 5f, 0);
                    SubtitleManager.Instance.SetSubtitle("This bad boy will scatter you anytime !", 1, 3.8f, 2.3f);
                    break;
                case typeOfComment.trap:
                    Main.Instance.PlaySoundWithDelay("PresA_Trap", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Trap", "Comment", Main.Instance.CommentBVolume, delay + 2.2f);
                    SubtitleManager.Instance.SetSubtitle("Watch out ! This is going to be deadly !", 0, 5f, 0);
                    SubtitleManager.Instance.SetSubtitle("Come on ! Squish'im like a bug !", 1, 3.3f, 2.6f);
                    break;
                case typeOfComment.win:
                    Main.Instance.PlaySoundWithDelay("PresA_Win", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlaySoundWithDelay("PresB_Win", "Comment", Main.Instance.CommentBVolume, delay + 3.5f);
                    SubtitleManager.Instance.SetSubtitle("Dear viewers, Death Live has a NEW CHAMPION!!!", 0, 5f, 0);
                    SubtitleManager.Instance.SetSubtitle("Now THAT'S what I call sport !", 1, 3f, 3.5f);
                    break;
                default:
                    break;
            }
        }
    }



}
