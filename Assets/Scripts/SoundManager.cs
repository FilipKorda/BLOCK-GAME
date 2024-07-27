using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Clips")]
    [SerializeField] private AudioClipInfo[] audioClipInfos;
    [SerializeField] private GameObject parent;

    private Dictionary<string, AudioClipInfo> audioClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeAudioClips();
        }
    }

    private void InitializeAudioClips()
    {
        audioClips = new Dictionary<string, AudioClipInfo>();

        foreach (var clipInfo in audioClipInfos)
        {
            if (clipInfo != null)
            {
                audioClips[clipInfo.clipName.ToString()] = clipInfo;
            }
            else
            {
                Debug.Log("Found a null AudioClipInfo!");
            }
        }
        Debug.Log($"<color=green>Total clips loaded:</color> {audioClips.Count}");
    }

    public void PlaySound(SoundClip clipName)
    {
        if (audioClips.ContainsKey(clipName.ToString()))
        {
            AudioClipInfo clipInfo = audioClips[clipName.ToString()];
            GameObject soundObject = new(clipInfo.clipName.ToString());

            if (parent != null)
            {
                soundObject.transform.SetParent(parent.transform);
            }

            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = clipInfo.clip;
            audioSource.volume = clipInfo.volume;
            audioSource.pitch = clipInfo.pitch;
            audioSource.Play();

            Destroy(soundObject, clipInfo.clip.length);
        }
        else
        {
            Debug.Log($"SoundManager: Clip {clipName} <color=red>not found!</color>");
        }
    }

}
