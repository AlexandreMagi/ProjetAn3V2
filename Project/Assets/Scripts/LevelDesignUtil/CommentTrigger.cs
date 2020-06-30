using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentTrigger : MonoBehaviour
{

    public enum typeOfComment { entreeFinalBattle, introAtterissage,introCouloir,minigun,salleSecrete,tourDeVerre,trap,win,introAvion,yourMama,doubleShooter}
    [SerializeField] typeOfComment comment = typeOfComment.entreeFinalBattle;
    [SerializeField] bool canPlay = true;
    [SerializeField] float delay = 0;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Main.Instance.PlayCommentWithDelay(1, "PresA_Borne_Gravitorb", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
            Main.Instance.PlayCommentWithDelay(0, "PresB_Borne_Gravitorb", "Comment", Main.Instance.CommentBVolume, delay + 4);
            SubtitleManager.Instance.SetSubtitle("Finally, the ultimate carnage tool !", 1, 10f, delay + 0);
            SubtitleManager.Instance.SetSubtitle("Indeed, the ability to pack hordes of enemies is extremely powerful!", 0, 5.3f, delay + 4);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (canPlay && Main.Instance.EnableComments)
        {
            canPlay = false;
            switch (comment)
            {
                case typeOfComment.entreeFinalBattle:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Entree_Final_Battle", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Entree_Final_Battle", "Comment", Main.Instance.CommentBVolume, delay + 2.0f);
                    SubtitleManager.Instance.SetSubtitle("Our Gladiator has reached the main stage !", 0, 4, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Now the real fun begins !", 1, 3, delay + 2.2f);
                    break;
                case typeOfComment.introAtterissage:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Intro_Atterrissage", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Intro_Atterrissage", "Comment", Main.Instance.CommentBVolume, delay + 4.5f);
                    SubtitleManager.Instance.SetSubtitle("Please give a warm welcome to our new gladiator in DEATH LIVE!", 0, 5f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Welcome, Good luck and.. Have fun !", 1, 5, delay + 4.8f);
                    break;
                case typeOfComment.introCouloir:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Intro_Couloirs", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Intro_Couloirs", "Comment", Main.Instance.CommentBVolume, delay + 3.2f);
                    SubtitleManager.Instance.SetSubtitle("Is this challenger ready to face the dangers of the arena ?", 0, 5f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Let's see how they hold up against swarmers !", 1, 5, delay + 3.2f);
                    break;
                case typeOfComment.minigun:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Minigun", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Minigun", "Comment", Main.Instance.CommentBVolume, delay + 2.3f);
                    SubtitleManager.Instance.SetSubtitle("Oh now he's packing the big gun !", 0, 5f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Let's see some bloood !", 1, 3, delay + 2.4f);
                    break;
                case typeOfComment.salleSecrete:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Salle_Secrete", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Salle_Secrete", "Comment", Main.Instance.CommentBVolume, delay + 0.6f);
                    SubtitleManager.Instance.SetSubtitle("Amazing ! A secret room!", 0, 5f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Jackpot !", 1, 1.5f, delay + 1.7f);
                    break;
                case typeOfComment.tourDeVerre:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Tour_De_Verre", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Tour_De_Verre", "Comment", Main.Instance.CommentBVolume, delay + 2f);
                    SubtitleManager.Instance.SetSubtitle("And here comes a new adversary !", 0, 5f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("This bad boy will scatter you anytime !", 1, 3.8f, delay + 2.3f);
                    break;
                case typeOfComment.trap:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Trap", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Trap", "Comment", Main.Instance.CommentBVolume, delay + 2.2f);
                    SubtitleManager.Instance.SetSubtitle("Watch out ! This is going to be deadly !", 0, 5f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Come on ! Squish'im like a bug !", 1, 3.3f, delay + 2.6f);
                    break;
                case typeOfComment.win:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Win", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Win", "Comment", Main.Instance.CommentBVolume, delay + 3.5f);
                    SubtitleManager.Instance.SetSubtitle("Dear viewers, Death Live has a NEW CHAMPION!!!", 0, 5f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Now THAT'S what I call sport !", 1, 3f, delay + 3.5f);
                    break;
                case typeOfComment.introAvion:
                    Main.Instance.PlayCommentWithDelay(0,"PresA_Intro_Avion", "Comment", Main.Instance.CommentBVolume, delay + 0);
                    Main.Instance.PlayCommentWithDelay(1,"PresB_Intro_Avion", "Comment", Main.Instance.CommentAVolume, delay + 14.5f);
                    SubtitleManager.Instance.SetSubtitle("And we are live ! Hello and welcome to this 821rst edition of your favourite extreme entertainement show !", 0, 10f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("I am your host, Abe and here with me today is TOX, the famous Veteran of the arena !", 0, 10f, delay + 8.1f);
                    SubtitleManager.Instance.SetSubtitle("I am glad to be back here tonight with you, dear viewers !", 1, 8f, delay + 14.8f);
                    SubtitleManager.Instance.SetSubtitle("I hope this competitor will at least have a fun death to watch, unlike the usual weaklings here!", 1, 11.3f, delay + 21.5f);
                    break;
                case typeOfComment.yourMama:
                    Main.Instance.PlayCommentWithDelay(1,"PresA_Your_Mama", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    Main.Instance.PlayCommentWithDelay(0,"PresB_Your_Mama", "Comment", Main.Instance.CommentBVolume, delay + 6);
                    SubtitleManager.Instance.SetSubtitle("Abe ! Look ! Look ! It's my autograph !", 1, 4.2f, delay + 0);
                    SubtitleManager.Instance.SetSubtitle("Your immaturity is matched only by your might", 0, 4.2f, delay + 6);
                    break;
                case typeOfComment.doubleShooter:
                    Main.Instance.PlayCommentWithDelay(1,"PresA_Double_Shooter", "Comment", Main.Instance.CommentAVolume, delay + 0.0f);
                    SubtitleManager.Instance.SetSubtitle(" Watch out for the crossfire !", 1, 3.3f, delay + 0);
                    break;
                default:
                    break;
            }
        }
    }



}
