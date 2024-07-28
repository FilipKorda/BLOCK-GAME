using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioClipInfo[] audioClips;

    private Dictionary<string, AudioClipInfo> audioClipDictionary;
    private List<GameObject> audioSourcePool;
    private int poolSize = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioClipDictionary = new Dictionary<string, AudioClipInfo>();
            foreach (var clipInfo in audioClips)
            {
                audioClipDictionary.Add(clipInfo.name, clipInfo);
            }

            audioSourcePool = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject audioObject = new("AudioSource_" + i);
                audioObject.AddComponent<AudioSource>();
                audioObject.transform.parent = this.transform; 
                audioObject.SetActive(false);
                audioSourcePool.Add(audioObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(SoundClip clipName)
    {
        if (audioClipDictionary.TryGetValue(clipName.ToString(), out AudioClipInfo clipInfo))
        {
            GameObject soundObject = GetPooledAudioSource();
            if (soundObject != null)
            {
                AudioSource audioSource = soundObject.GetComponent<AudioSource>();
                audioSource.clip = clipInfo.clip;
                audioSource.volume = clipInfo.volume;
                audioSource.pitch = clipInfo.pitch;
                soundObject.SetActive(true);
                audioSource.Play();

                StartCoroutine(DisableSoundObjectAfterPlayback(soundObject, clipInfo.clip.length));
            }
            else
            {
                Debug.Log("SoundManager: No available audio sources in the pool.");
            }
        }
        else
        {
            Debug.Log($"SoundManager: Clip {clipName} <color=red>not found!</color>");
        }
    }

    private GameObject GetPooledAudioSource()
    {
        foreach (var audioObject in audioSourcePool)
        {
            if (audioObject != null && !audioObject.activeInHierarchy)
            {
                return audioObject;
            }
        }
        return null;
    }

    private IEnumerator DisableSoundObjectAfterPlayback(GameObject soundObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (soundObject != null)
        {
            soundObject.SetActive(false);
        }
    }
}
