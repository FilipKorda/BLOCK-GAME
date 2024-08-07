public class AttemptsAchievement : Achievement
{
    private int attemptsTarget;

    public AttemptsAchievement(AchievementData data, int target) : base(data)
    {
        attemptsTarget = target;
    }

    public override void CheckCriteria(GameManager playerStats)
    {
        if (playerStats.AttemptsNumber >= attemptsTarget)
        {
            Unlock();
        }
    }
}
