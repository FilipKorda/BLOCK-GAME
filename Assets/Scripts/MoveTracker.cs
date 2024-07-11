using TMPro;
using UnityEngine;

public class MoveTracker : MonoBehaviour
{
    [Header("TextMeshPro Field")]
    public TextMeshProUGUI textMeshPro;

    private void Start()
    {
        UpdateMoveCountText();
    }

    public void AddMove()
    {
        GameManager.Instance.moveCount++;
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

    private void UpdateMoveCountText()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = $"{GameManager.Instance.moveCount}";
        }
    }
}
