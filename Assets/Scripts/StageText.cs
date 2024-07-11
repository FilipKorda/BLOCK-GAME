using TMPro;
using UnityEngine;

public class StageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private string stageString;

    private void Start()
    {
        UpdateStageText();
    }
    public void UpdateStageText()
    {
        stageText.text = stageString;
    }
}
