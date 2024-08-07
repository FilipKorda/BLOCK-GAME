using System;
public abstract class Achievement
{
    public AchievementData AchievementData { get; private set; }
    public bool IsUnlocked { get; protected set; }

    public event Action<Achievement> OnUnlock;

    public Achievement(AchievementData data)
    {
        AchievementData = data;
        IsUnlocked = false;
    }

    public abstract void CheckCriteria(GameManager playerStats);

    protected void Unlock()
    {
        if (!IsUnlocked)
        {
            IsUnlocked = true;
            OnUnlock?.Invoke(this);
        }
    }

}
