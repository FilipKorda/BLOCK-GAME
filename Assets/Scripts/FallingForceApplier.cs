using UnityEngine;

public class FallingForceApplier : MonoBehaviour
{
    [SerializeField] private GameObject[] players;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var playerScript))
        {
            foreach (var player in players)
            {
                if (player.activeInHierarchy && playerScript.enabled)
                {
                    playerScript.PreparPlayerToFallDown();
                }
            }
            SoundManager.Instance.PlaySound(SoundClip.LoseSound);
        }
    }
}
