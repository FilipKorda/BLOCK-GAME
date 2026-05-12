using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoQueue : MonoBehaviour
{
    [Header("Video Managment")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip[] videoClips;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private LoadingSystem loadingSystem;
    private int currentVideoIndex = 0;
    private bool loadNextLevelCalled = false;

    [Header("Video Explanation Managment")]
    [SerializeField] private TextMeshProUGUI tutorialExplanationText;
    [TextArea(3, 10)]
    [SerializeField] private string[] tutorialExplanation;

    [Header("Video Play/Pause Managment")]
    [SerializeField] private Image playPauseImage;
    [SerializeField] private Image playPauseV2Image;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private float fadeDuration = 0.3f;

    void Start()
    {
        UpdateNumberText();

        playPauseImage.color = new Color(0f, 0f, 0f, 0f);

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

        if (Input.GetKeyDown(KeyCode.Return) && !loadNextLevelCalled)
        {
            LoadNextLexelOnce();
        }

        if (Input.GetMouseButtonDown(0))
        {
            PlayPauseVideo();
        }
    }

    void PlayPauseVideo()
    {
        StartCoroutine(ShowSign());

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
            playPauseImage.sprite = playSprite;
            playPauseV2Image.sprite = pauseSprite;
        }
        else
        {
            videoPlayer.Play();
            playPauseImage.sprite = pauseSprite;
            playPauseV2Image.sprite = playSprite;
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

    private IEnumerator ShowSign()
    {
        StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(FadeToClear());
    }

    private IEnumerator FadeToBlack()
    {
        if (playPauseImage != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
                playPauseImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
        }
    }

    private IEnumerator FadeToClear()
    {
        if (playPauseImage != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
                playPauseImage.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
        }
    }
}
