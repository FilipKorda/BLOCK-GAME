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

    public Button resolutionButton;
    public TextMeshProUGUI resolutionButtonText;
    private string[] resolutionArray = { "1280x720", "1366x768", "1600x900", "1920x1080", "2560x1440" };

    private int currentResolutionIndex;


    void Start()
    {
        currentQualityLevel = QualitySettings.GetQualityLevel();
        UpdateQualityButtonLabel();
        toggleQualityButton.onClick.AddListener(ToggleQuality);

        isFullscreen = ScreenSettings.Fullscreen;
        UpdateScreenButtonLabel();
        toggleScreenModeButton.onClick.AddListener(ToggleScreenMode);

        currentResolutionIndex = FindCurrentResolutionIndex();
        UpdateResolutionLabel();
        resolutionButton.onClick.AddListener(ResolutionMode);
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
        ScreenSettings.Apply(ParseResolution(resolutionArray[currentResolutionIndex]), isFullscreen);
        UpdateScreenButtonLabel();
    }

    private void UpdateScreenButtonLabel()
    {
        screenButtonText.text = isFullscreen ? "Fullscreen" : "Windowed";
    }

    private int FindCurrentResolutionIndex()
    {
        // Na pełnym ekranie okno ma zawsze natywną rozdzielczość, więc liczy się wybrana (renderowania).
        Vector2Int chosen = ScreenSettings.ChosenResolution;
        string currentResolution = chosen.x + "x" + chosen.y;
        for (int i = 0; i < resolutionArray.Length; i++)
        {
            if (resolutionArray[i] == currentResolution)
            {
                return i;
            }
        }
        return 0;
    }

    public void ResolutionMode()
    {
        currentResolutionIndex = (currentResolutionIndex + 1) % resolutionArray.Length;
        ApplyResolution();
        UpdateResolutionLabel();
    }

    private void ApplyResolution()
    {
        ScreenSettings.Apply(ParseResolution(resolutionArray[currentResolutionIndex]), isFullscreen);
    }

    private static Vector2Int ParseResolution(string resolution)
    {
        string[] parts = resolution.Split('x');
        return new Vector2Int(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private void UpdateResolutionLabel()
    {
        resolutionButtonText.text = resolutionArray[currentResolutionIndex];
    }
}
