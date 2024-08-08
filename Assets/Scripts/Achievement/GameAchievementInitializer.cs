using UnityEngine;

public class GameAchievementInitializer : MonoBehaviour
{
    [SerializeField] private GameManager playerStats;
    private AchievementManager achievementManager;

    [SerializeField] private AchievementUI achievementUI;
    [SerializeField] private AchievementData moveAchievementData;
    [SerializeField] private AchievementData starAchievementData;
    [SerializeField] private AchievementData attemptAchievementData;
    [SerializeField] private AchievementData levelTenAchievementData;

    private void Start()
    {
        playerStats = GetComponent<GameManager>();
        achievementManager = new AchievementManager(playerStats);

        achievementManager.AddAchievement(new MovesAchievement(moveAchievementData, 100));
        achievementManager.AddAchievement(new StarAchievement(starAchievementData, 16));
        achievementManager.AddAchievement(new AttemptsAchievement(attemptAchievementData, 6));
        achievementManager.AddAchievement(new LevelTenAchievement(levelTenAchievementData, 9));

        foreach (var achievement in achievementManager.Achievements)
        {
            achievement.OnUnlock += AchievementUnlocked;
        }
    }

    private void AchievementUnlocked(Achievement achievement)
    {
        Debug.Log($"Achievement Unlocked: {achievement.AchievementData.achievementTitle}");
        achievementUI.ShowAchievement(achievement.AchievementData);
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
