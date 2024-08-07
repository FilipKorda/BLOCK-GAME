using UnityEngine;

public class GameAchievementInitializer : MonoBehaviour
{
    [SerializeField] private GameManager playerStats;
    private AchievementManager achievementManager;

    private void Start()
    {
        playerStats = GetComponent<GameManager>();
        achievementManager = new AchievementManager(playerStats);

        achievementManager.AddAchievement(new MovesAchievement(100));
        achievementManager.AddAchievement(new StarAchievement(16));
        achievementManager.AddAchievement(new AttemptsAchievement(6));
    }


    //TEST Buttons
    [ContextMenu("Add Moves")]
    private void ButtonAddMoves()
    {
        playerStats.AddTESTAchievementMove(100);
    }
    [ContextMenu("Add Stars")]
    private void ButtonAddStarts()
    {
        playerStats.AddTESTAchievementStar(100);
    }
    [ContextMenu("Add Attempts")]
    private void ButtonAddAttempts()
    {
        playerStats.AddTESTAchievementAttempts(100);
    }
}
