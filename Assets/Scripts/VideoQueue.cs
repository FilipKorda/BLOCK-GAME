using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class VideoQueue : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public VideoClip[] videoClips;
    public TextMeshProUGUI numberText;
    public LevelConector levelConector;
    private int currentVideoIndex = 0;
    private bool loadNextLevelCalled = false;

    void Start()
    {
        UpdateNumberText();

        if (videoClips.Length > 0)
        {
            videoPlayer.clip = videoClips[currentVideoIndex];
            videoPlayer.Play();
        }
        videoPlayer.loopPointReached += OnVideoEnd;
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
            if (levelConector != null)
            {
                levelConector.LoadNextLexel();
            }
        }
    }

    void PlayVideoAtIndex(int index)
    {
        videoPlayer.clip = videoClips[index];
        videoPlayer.Play();
        UpdateNumberText();
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
