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
    private string[] resolutionArray = { "1280x720", "1280x1024", "1600x1200", "1920x1080" };

    private int currentResolutionIndex;


    void Start()
    {
        currentQualityLevel = QualitySettings.GetQualityLevel();
        UpdateQualityButtonLabel();
        toggleQualityButton.onClick.AddListener(ToggleQuality);

        isFullscreen = Screen.fullScreen;
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
        Screen.fullScreen = isFullscreen;
        UpdateScreenButtonLabel();
    }

    private void UpdateScreenButtonLabel()
    {
        screenButtonText.text = isFullscreen ? "Fullscreen" : "Windowed";
    }

    private int FindCurrentResolutionIndex()
    {
        string currentResolution = Screen.width + "x" + Screen.height;
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
        string[] resolution = resolutionArray[currentResolutionIndex].Split('x');
        int width = int.Parse(resolution[0]);
        int height = int.Parse(resolution[1]);
        Screen.SetResolution(width, height, Screen.fullScreen);
    }

    private void UpdateResolutionLabel()
    {
        resolutionButtonText.text = resolutionArray[currentResolutionIndex];
    }
}
