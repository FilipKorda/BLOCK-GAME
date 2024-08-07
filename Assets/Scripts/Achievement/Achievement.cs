using System;
using UnityEngine;

public abstract class Achievement
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public bool IsUnlocked { get; protected set; }

    public event Action<Achievement> OnUnlock;

    public Achievement(string name, string description)
    {
        Name = name;
        Description = description;
        IsUnlocked = false;
    }

    public abstract void CheckCriteria(GameManager playerStats);

    protected void Unlock()
    {
        if (!IsUnlocked)
        {
            IsUnlocked = true;
            OnUnlock?.Invoke(this);
            Debug.Log($"Achievement Unlocked: {Name} - {Description}");
        }
    }
}
