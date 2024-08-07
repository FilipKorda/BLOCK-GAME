
public class StarAchievement : Achievement
{
    private int starTarget;

    public StarAchievement(int target) : base($"Get Star {target} Times", $"Get Star {target} times to unlock this achievement.")
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
