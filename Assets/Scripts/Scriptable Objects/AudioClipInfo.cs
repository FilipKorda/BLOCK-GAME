using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipInfo", menuName = "Audio/Clip Info", order = 1)]
public class AudioClipInfo : ScriptableObject
{
    public SoundClip clipName;
    public AudioClip clip;
    public float volume = 1.0f;
    public float pitch = 1.0f;
}
