
public class StarAchievement : Achievement
{
    private int starTarget;

    public StarAchievement(AchievementData data, int target) : base(data)
    {
        starTarget = target;
    }

    public override void CheckCriteria(GameManager playerStats)
    {
        if (playerStats.StarCount >= starTarget)
        {
            Unlock();
        }
    }
}
