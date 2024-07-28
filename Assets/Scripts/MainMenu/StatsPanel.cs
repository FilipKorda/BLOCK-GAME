using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanel : MonoBehaviour
{
    [SerializeField] private Image statsPanelImage;
    [SerializeField] private GameObject currentStage;
    [SerializeField] private GameObject attempts;
    [SerializeField] private GameObject moves;
    [SerializeField] private GameObject stars;
    [SerializeField] private TextMeshProUGUI currentStageNumberText;
    [SerializeField] private TextMeshProUGUI attemptsNumberText;
    [SerializeField] private TextMeshProUGUI movesNumberText;
    [SerializeField] private TextMeshProUGUI starsNumberText;
    [SerializeField] private SceneData currentStageNumber;

    private void Start()
    {
        UpdateAttempts();
        UpdateMoves();
        UpdateStars();
        if (currentStageNumberText != null)
            currentStageNumberText.text = currentStageNumber.stageNumber.ToString();
    }

    public void AddAttemptsNumber()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.attemptsNumber++;
    }

    public void UpdateAttempts()
    {
        if (attemptsNumberText != null && GameManager.Instance != null)
        {
            attemptsNumberText.text = $"{GameManager.Instance.attemptsNumber}";
        }
    }

    public void UpdateMoves()
    {
        if (movesNumberText != null && GameManager.Instance != null)
            movesNumberText.text = $"{GameManager.Instance.moveCount}";
    }

    public void UpdateStars()
    {
        if (starsNumberText && GameManager.Instance != null)
            starsNumberText.text = $"{GameManager.Instance.starCount}";
    }

    public void ActiveStatsPanel()
    {
        statsPanelImage.enabled = true;
        currentStage.SetActive(true);
        attempts.SetActive(true);
        moves.SetActive(true);
        stars.SetActive(true);
    }

    public void DeactiveStatsPanel()
    {
        statsPanelImage.enabled = false;
        currentStage.SetActive(false);
        attempts.SetActive(false);
        moves.SetActive(false);
        stars.SetActive(false);
    }
}
