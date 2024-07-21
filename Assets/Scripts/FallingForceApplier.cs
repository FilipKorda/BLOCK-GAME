using UnityEngine;

public class FallingForceApplier : MonoBehaviour
{
    public enum DirectionForceApplier
    {
        None,
        W,
        S,
        A,
        D
    }

    [Header("Force Directions")]
    [SerializeField] private DirectionForceApplier[] forceDirections;
    [SerializeField] private float forceStrength = 35f;
    [SerializeField] private Player player;
    [SerializeField] private FallingDownManager fallingDownManager;


    private Vector3 forceDirection_W = new(0, -20, 5);
    private Vector3 forceDirection_A = new(-5, -20, 0);
    private Vector3 forceDirection_S = new(0, -20, -5);
    private Vector3 forceDirection_D = new(5, -20, 0);

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            player.PreparPlayerToFallDown();
            Vector3 force = Vector3.zero;

            foreach (var direction in forceDirections)
            {
                if (direction == player.rotationDirection)
                {
                    switch (direction)
                    {
                        case DirectionForceApplier.W:
                            force += new Vector3(0, -forceStrength, 5);
                            break;
                        case DirectionForceApplier.S:
                            force += new Vector3(0, -forceStrength, -5);
                            break;
                        case DirectionForceApplier.A:
                            force += new Vector3(-5, -forceStrength, 0);
                            break;
                        case DirectionForceApplier.D:
                            force += new Vector3(5, -forceStrength, 0);
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (var groundObject in fallingDownManager.groundObjects)
            {
                if (groundObject != null)
                {
                    Collider collider = groundObject.GetComponent<Collider>();
                    collider.isTrigger = true;
                }
            }

            rb.AddForce(force, ForceMode.Impulse);
        }
    }
}
