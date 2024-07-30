using UnityEngine;

public class FallingForceApplier : MonoBehaviour
{
    [SerializeField] private GameObject[] players;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            foreach (var player in players)
            {
                if (player.activeInHierarchy && player.GetComponent<Player>().enabled)
                {
                    player.GetComponent<Player>().PreparPlayerToFallDown();
                }
            }

            SoundManager.Instance.PlaySound(SoundClip.LoseSound);
        }
    }
}
