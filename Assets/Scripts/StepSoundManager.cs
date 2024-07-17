using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSoundManager : MonoBehaviour
{
    public AudioClip grassFootstep;
    public AudioClip concreteFootstep;
    public AudioClip woodFootstep;

    private AudioSource audioSource;
    private bool isMoving;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        isMoving = false;
    }

    void Update()
    {
        // Sprawdzanie czy gracz siê obraca
        if (isMoving)
        {
            Ray ray = new(transform.position, Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            if (Physics.Raycast(ray, out RaycastHit hit, 1.5f))
            {
                switch (hit.collider.tag)
                {
                    case "Grass":
                        PlayFootstep(grassFootstep);
                        break;
                    case "Concrete":
                        PlayFootstep(concreteFootstep);
                        break;
                    case "Wood":
                        PlayFootstep(woodFootstep);
                        break;
                    default:
                        // Mo¿esz dodaæ domyœlny dŸwiêk lub zostawiæ pust¹ akcjê
                        break;
                }
            }
        }
    }

    void PlayFootstep(AudioClip clip)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // Metody do ustawiania ruchu
    public void StartMoving()
    {
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }
}
