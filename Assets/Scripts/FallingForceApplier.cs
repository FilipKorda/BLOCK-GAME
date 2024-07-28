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
    [SerializeField] private float downFallForce = 5f;
    [SerializeField] private Player player;
    [SerializeField] private FallingDownManager fallingDownManager;
    [SerializeField] private float rotationSpeed = 100f;

    private bool isRotating;
    private float rotationX = 0;
    private float rotationY = 0;
    private float rotationZ = 0;

    private Vector3 forceDirection_W = new(0, -20, 5);
    private Vector3 forceDirection_A = new(-5, -20, 0);
    private Vector3 forceDirection_S = new(0, -20, -5);
    private Vector3 forceDirection_D = new(5, -20, 0);

    private bool soundPlay = false;

    private void Start()
    {
        isRotating = false;
    }

    private void Update()
    {
        if (isRotating && player.gameObject.activeInHierarchy)
        {
            Vector3 rotation = rotationSpeed * Time.deltaTime * new Vector3(rotationX, rotationY, rotationZ);

            player.gameObject.transform.Rotate(rotation);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out var rb))
        {
            player.PreparPlayerToFallDown();

            if(!soundPlay)
            {
                SoundManager.Instance.PlaySound(SoundClip.LoseSound);
                soundPlay = true;
            }
          

            Vector3 force = Vector3.zero;

            isRotating = true;

            foreach (var direction in forceDirections)
            {
                if (direction == player.rotationDirection)
                {
                    switch (direction)
                    {
                        case DirectionForceApplier.W:
                            rotationX = 1;
                            rotationZ = 1;

                            force += new Vector3(0, -downFallForce, 5);
                            break;
                        case DirectionForceApplier.S:
                            rotationX = -1;
                            rotationZ = -1;

                            force += new Vector3(0, -downFallForce, -5);
                            break;
                        case DirectionForceApplier.A:
                            rotationX = -1;
                            rotationZ = -1;

                            force += new Vector3(-5, -downFallForce, 0);
                            break;
                        case DirectionForceApplier.D:
                            rotationX = 1;
                            rotationZ = 1;

                            force += new Vector3(5, -downFallForce, 0);
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
