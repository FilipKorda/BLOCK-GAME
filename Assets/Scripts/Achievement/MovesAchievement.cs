public class MovesAchievement : Achievement
{
    private int movesTarget;

    public MovesAchievement(AchievementData data, int target) : base(data)
    {
        movesTarget = target;
    }

    public override void CheckCriteria(GameManager playerStats)
    {
        if (playerStats.MoveCount >= movesTarget)
        {
            Unlock();
        }
    }
}
