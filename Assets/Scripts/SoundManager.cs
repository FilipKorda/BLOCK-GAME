using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SoundType
    {
        GetStarSound = 0,
        LoseSound = 1,
        WinSound = 2,
        ConnectTwoCube = 3,
        StartGame = 4,
    }

    public static SoundManager Instance;

    [SerializeField] private AudioClip getStarSound;
    [SerializeField] private AudioClip loseSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip connectTwoCube;
    [SerializeField] private AudioClip startGame;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.GetStarSound:
                audioSource.clip = getStarSound;
                break;
            case SoundType.LoseSound:
                audioSource.clip = loseSound;
                break;
            case SoundType.WinSound:
                audioSource.clip = winSound;
                break;
            case SoundType.ConnectTwoCube:
                audioSource.clip = connectTwoCube;
                break;
            case SoundType.StartGame:
                audioSource.clip = startGame;
                break;
        }
        audioSource.Play();
    }
}
