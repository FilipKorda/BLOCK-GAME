using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementPanel;
    [SerializeField] private TextMeshProUGUI achievementTitle;
    [SerializeField] private Image achievementImage;
    [SerializeField] private float displayTime = 3.0f;

    private void Start()
    {
        achievementPanel.SetActive(false);
    }

    public void ShowAchievement(AchievementData data)
    {
        achievementTitle.text = data.achievementTitle;
        achievementImage.sprite = data.achievementImage;
        achievementPanel.SetActive(true);
        StartCoroutine(HideAchievementAfterTime(displayTime));
    }

    private IEnumerator HideAchievementAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        achievementPanel.SetActive(false);
    }

}
