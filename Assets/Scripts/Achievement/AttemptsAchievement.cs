public class AttemptsAchievement : Achievement
{
    private int attemptsTarget;

    public AttemptsAchievement(int target) : base($"Attempt {target} Times", $"Attempt {target} to unlock this achievement.")
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
