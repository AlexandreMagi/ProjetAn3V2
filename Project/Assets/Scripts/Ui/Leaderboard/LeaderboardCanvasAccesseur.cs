using UnityEngine;
using UnityEngine.UI;

public class LeaderboardCanvasAccesseur : MonoBehaviour
{
    public Transform root_FirstLeaderboard = null;
    public Transform root_NameAndTitleChoice = null;
    public Transform root_FinalLeaderboard = null;
    public Transform root_SingleScores = null;
    public CharSelect[] charSelectors = null; // Stock des gérant de character
    public Text scoreAtCharSelect = null;
    public Text scoreCharGoTo = null;
    public UiCrossHair crossHairHandler = null;
    public Text titleChoiceText = null;
    public TitleSelecHandler titleHandler = null;
    public BonusHandler bonusHandler = null;
    public Transform parentForScoreAfterTransition = null;
    public Animator charSelectAnimator = null;
    public LeaerboardMultiIdle idleAnimatorInCharSelect = null;
}
