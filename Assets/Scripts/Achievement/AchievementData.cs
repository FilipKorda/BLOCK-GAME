using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "ScriptableObjects/Achievement")]
public class AchievementData : ScriptableObject
{
    public string achievementTitle;
    public Sprite achievementImage;
}
