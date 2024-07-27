using TMPro;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{    
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
        currentStageNumberText.text = currentStageNumber.stageNumber.ToString();
    }

    public void AddAttemptsNumber()
    {
        GameManager.Instance.attemptsNumber++;
    }

    public void UpdateAttempts()
    {
        attemptsNumberText.text = GameManager.Instance.attemptsNumber.ToString();        
    }

    public void UpdateMoves()
    {
        movesNumberText.text = GameManager.Instance.moveCount.ToString();
    }

    public void UpdateStars()
    {
        attemptsNumberText.text = GameManager.Instance.starCount.ToString();
    }
}
