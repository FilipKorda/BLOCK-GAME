using System.Collections.Generic;

public class AchievementManager
{
    private readonly List<Achievement> achievements = new();
    private readonly GameManager playerStats;

    public AchievementManager(GameManager stats)
    {
        playerStats = stats;
        playerStats.OnStatsChanged += CheckAchievements;
    }

    public void AddAchievement(Achievement achievement)
    {
        achievements.Add(achievement);
    }

    private void CheckAchievements()
    {
        foreach (var achievement in achievements)
        {
            achievement.CheckCriteria(playerStats);
        }
    }
}
