using TMPro;
using UnityEngine;

public class MoveTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        UpdateMoveCountText();
    }

    public void AddMove()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.moveCount++;
        }
        UpdateMoveCountText();
    }
    
    public void ShowMoves()
    {
        textMeshPro.gameObject.SetActive(true);
    }

    public void HideMoves()
    {
        textMeshPro.gameObject.SetActive(false);
    }

    public void UpdateMoveCountText()
    {
        if (textMeshPro != null && GameManager.Instance != null)
        {
            textMeshPro.text = $"{GameManager.Instance.moveCount}";
        }
    }
}
