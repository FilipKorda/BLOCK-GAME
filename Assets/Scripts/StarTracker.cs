using TMPro;
using UnityEngine;

public class StarTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI starCountText;

    private void Start()
    {
        UpdateStarCountUI();
    }

    public void AddStar()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.starCount++;
        }
        UpdateStarCountUI();
    }

    public void ShowStars()
    {
        starCountText.gameObject.SetActive(true);
    }

    public void HideStars()
    {
        starCountText.gameObject.SetActive(false);
    }

    public void UpdateStarCountUI()
    {
        if (starCountText != null && GameManager.Instance != null)
        {
            starCountText.text = $"{GameManager.Instance.starCount}";
        }
    }
}
