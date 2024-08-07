public class MovesAchievement : Achievement
{
    private int movesTarget;

    public MovesAchievement(int target) : base($"Make move {target} times", $"Make move {target} times to unlock this achievement.")
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
