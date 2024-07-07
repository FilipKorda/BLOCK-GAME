using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsManager : MonoBehaviour
{
   // public AudioSource musicSource;
    public Button toggleSoundButton;
    public TextMeshProUGUI buttonText;
    private bool isMuted = false;

    void Start()
    {
        UpdateButtonLabel();
        toggleSoundButton.onClick.AddListener(ToggleSound);
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;
        //musicSource.mute = isMuted;
        UpdateButtonLabel();
    }

    private void UpdateButtonLabel()
    {
        buttonText.text = isMuted ? "Unmute" : "Mute";
    }
}
