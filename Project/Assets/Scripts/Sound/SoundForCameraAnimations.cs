using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundForCameraAnimations : MonoBehaviour
{
    public void PlayStepSound()
    {
        CustomSoundManager.Instance.PlaySound("Step_0" + Random.Range(1, 5), "Player", 1);
    }
    public void PlayEffortSound()
    {
        CustomSoundManager.Instance.PlaySound("SE_WallJump", "Player", transform, 2, false, 1, 0.2f);
    }
    public void PlayYourMama()
    {
        Main.Instance.PlayCommentWithDelay(1, "PresA_Your_Mama", "Comment", Main.Instance.CommentAVolume, 0.0f);
        Main.Instance.PlayCommentWithDelay(0, "PresB_Your_Mama", "Comment", Main.Instance.CommentBVolume,  6);
        SubtitleManager.Instance.SetSubtitle("Abe ! Look ! Look ! It's my autograph !", 1, 4.2f, 0);
        SubtitleManager.Instance.SetSubtitle("Your immaturity is matched only by your might", 0, 4.2f,  6);
    }
}
