using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoQueue : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip[] videoClips;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private LoadingSystem loadingSystem;
    private int currentVideoIndex = 0;
    private bool loadNextLevelCalled = false;


    [SerializeField] private TextMeshProUGUI tutorialExplanationText;
    [TextArea(3, 10)]
    [SerializeField] private string[] tutorialExplanation;

    void Start()
    {
        UpdateNumberText();

        if (videoClips.Length > 0)
        {
            videoPlayer.clip = videoClips[currentVideoIndex];
            videoPlayer.Play();
            tutorialExplanationText.text = tutorialExplanation[currentVideoIndex];
        }
        videoPlayer.loopPointReached += OnVideoEnd;     
    }

    void StringInTables()
    {
        tutorialExplanation = new string[]
         {
            "Use the WASD keys to move the player.",
            "To activate the button with the dashed circle, you need to stand on it vertically and use the TAB key on the keyboard to switch between blocks. You can move the blocks using the WASD keys.",
            "You can activate the button with the circle by standing on it vertically or horizontally. This button can open and close your bridge.",
            "The button with the cross can only be activated when you stand on it vertically. It also opens or closes your bridge.",
            "Watch out for the orange tiles; they are very dangerous.",
            "Be careful not to fall off the edge of the map.",
            "Look for the open field on the ground on the map; it will take you to the next level."
         };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayPreviousVideo();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            PlayNextVideo();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && !loadNextLevelCalled)
        {
            LoadNextLexelOnce();
        }
    }

    void PlayPreviousVideo()
    {
        currentVideoIndex--;
        if (currentVideoIndex < 0)
        {
            currentVideoIndex = videoClips.Length - 1;
        }
        PlayVideoAtIndex(currentVideoIndex);
    }

    void PlayNextVideo()
    {
        currentVideoIndex++;
        if (currentVideoIndex >= videoClips.Length)
        {
            currentVideoIndex = 0;
        }
        PlayVideoAtIndex(currentVideoIndex);
    }

    public void LoadNextLexelOnce()
    {
        if (!loadNextLevelCalled)
        {
            loadNextLevelCalled = true;
            if (loadingSystem != null)
            {
                loadingSystem.LoadNextLexel();
            }
        }
    }

    void PlayVideoAtIndex(int index)
    {
        videoPlayer.clip = videoClips[index];
        videoPlayer.Play();
        UpdateNumberText();
        tutorialExplanationText.text = tutorialExplanation[index];
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        PlayNextVideo();
    }

    void UpdateNumberText()
    {
        numberText.text = (currentVideoIndex + 1).ToString();
    }
}
