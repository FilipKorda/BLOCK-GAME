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

    private void UpdateMoveCountText()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = $"{GameManager.Instance.moveCount}";
        }
    }
}
