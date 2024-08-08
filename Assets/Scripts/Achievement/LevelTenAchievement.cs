public class LevelTenAchievement : Achievement
{
    private int levelTarget;

    public LevelTenAchievement(AchievementData data, int target) : base(data)
    {
        levelTarget = target;
    }

    public override void CheckCriteria(GameManager playerStats)
    {
        if (playerStats.currentUnlockedAvailableCodesIndex >= levelTarget)
        {
            Unlock();

        }
    }
}
