using UnityEngine;

public class StepSoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip stoneNormalSound;
    [SerializeField] private AudioClip stoneBridgeSound;
    [SerializeField] private AudioClip trappedPlateSound;
    [SerializeField] private AudioSource audioSource;

    public void PlaySound(string surfaceTag)
    {
        switch (surfaceTag)
        {
            case "StoneNormalSound":
                audioSource.clip = stoneNormalSound;
                break;
            case "StoneBridgeSound":
                audioSource.clip = stoneBridgeSound;
                break;
            case "TrappedPlateSound":
                audioSource.clip = trappedPlateSound;
                break;
        }
        audioSource.Play();
    }
}
