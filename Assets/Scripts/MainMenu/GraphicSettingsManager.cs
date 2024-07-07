using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GraphicSettingsManager : MonoBehaviour
{
    public Button toggleQualityButton;
    public TextMeshProUGUI qualityButtonText;

    private int currentQualityLevel = 0;
    private string[] qualityLevels = { "Low", "Medium", "High" };

    public Button toggleScreenModeButton;
    public TextMeshProUGUI screenButtonText;
    private bool isFullscreen;

    void Start()
    {
        currentQualityLevel = QualitySettings.GetQualityLevel();
        UpdateQualityButtonLabel();
        toggleQualityButton.onClick.AddListener(ToggleQuality);

        isFullscreen = Screen.fullScreen;
        UpdateScreenButtonLabel();
        toggleScreenModeButton.onClick.AddListener(ToggleScreenMode);
    }

    public void ToggleQuality()
    {
        currentQualityLevel = (currentQualityLevel + 1) % qualityLevels.Length;
        QualitySettings.SetQualityLevel(currentQualityLevel, true);
        UpdateQualityButtonLabel();
    }

    private void UpdateQualityButtonLabel()
    {
        qualityButtonText.text = qualityLevels[currentQualityLevel];
    }

    public void ToggleScreenMode()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
        UpdateScreenButtonLabel();
    }

    private void UpdateScreenButtonLabel()
    {
        screenButtonText.text = isFullscreen ? "Fullscreen" : "Windowed";
    }
}
