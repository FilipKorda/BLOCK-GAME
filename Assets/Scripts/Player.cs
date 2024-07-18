using UnityEngine;

public class Player : MonoBehaviour
{
    private readonly float rotationSpeed = 750f;
    private float totalRotation;
    private bool isRotating;
    private Direction rotationDirection;
    public Vector3 pivot, axis, scale;
    private Rigidbody rb;

    [Header("Player Management")]
    public bool canMove;

    [Header("Force Directions")]
    [SerializeField] private Vector3 forceDirection_W = new(0, -10, 10);
    [SerializeField] private Vector3 forceDirection_A = new(-10, -10, 0);
    [SerializeField] private Vector3 forceDirection_S = new(0, -10, -10);
    [SerializeField] private Vector3 forceDirection_D = new(10, -10, 0);

    [Header("Layer Mask of Invisible Wall")]
    [SerializeField] private LayerMask collisionLayerMask;

    [SerializeField] private MoveTracker moveTracker;
    private StepSoundManager stepSoundManager;

    void Start()
    {
        isRotating = false;
        canMove = true;
        scale = transform.localScale / 2f;
        rb = GetComponent<Rigidbody>();
        stepSoundManager = GetComponent<StepSoundManager>();
        FreezeAllAxes();
    }

    void Update()
    {
        if (canMove)
        {
            if (isRotating)
            {
                float deltaRotation = rotationSpeed * Time.deltaTime;
                if (totalRotation + deltaRotation >= 90f)
                {
                    deltaRotation = 90f - totalRotation;
                    isRotating = false;
                    moveTracker.AddMove();
                    DetectSurface();
                }
                if ((rotationDirection == Direction.A) || (rotationDirection == Direction.W))
                    transform.RotateAround(pivot, axis, deltaRotation);
                else transform.RotateAround(pivot, axis, -deltaRotation);
                totalRotation += deltaRotation;
            }
            else if (Input.GetKeyDown(KeyCode.W)) Rotate(Direction.W);
            else if (Input.GetKeyDown(KeyCode.A)) Rotate(Direction.A);
            else if (Input.GetKeyDown(KeyCode.S)) Rotate(Direction.S);
            else if (Input.GetKeyDown(KeyCode.D)) Rotate(Direction.D);
        }

    }

    void Rotate(Direction direction)
    {
        rotationDirection = direction;
        isRotating = true;
        totalRotation = 0f;

        switch (rotationDirection)
        {
            case Direction.D:
                pivot = transform.position + new Vector3(scale.x, -scale.y, 0);
                break;
            case Direction.A:
                pivot = transform.position + new Vector3(-scale.x, -scale.y, 0);
                break;
            case Direction.W:
                pivot = transform.position + new Vector3(0, -scale.y, scale.z);
                break;
            case Direction.S:
                pivot = transform.position + new Vector3(0, -scale.y, -scale.z);
                break;
        }
        if ((rotationDirection == Direction.D) || (rotationDirection == Direction.A))
        {
            axis = Vector3.forward;
            (scale.y, scale.x) = (scale.x, scale.y);
        }
        else
        {
            axis = Vector3.right;
            (scale.y, scale.z) = (scale.z, scale.y);
        }
    }

    void DetectSurface()
    {
        Ray ray = new(transform.position, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            string surfaceTag = hit.collider.tag;
            stepSoundManager.PlaySound(surfaceTag);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if ((collisionLayerMask.value & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }

        Vector3 collisionDirection = (collision.transform.position - transform.position).normalized;
        Debug.Log("Collision Direction: " + collisionDirection);
        Debug.Log("Collision Direction: " + collision.gameObject.name);
        Destroy(collision.gameObject);

        if (IsInRange(collisionDirection, new Vector3(-1f, -1f, 0f), new Vector3(1f, 0f, 1f)))  // to dzia³a na przycisk W
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirection_W, ForceMode.Impulse);
        }
        if (IsInRange(collisionDirection, new Vector3(-1f, -1f, -1f), new Vector3(0f, 0f, 1f)))// to dzia³a na przycisk A
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirection_A, ForceMode.Impulse);
        }
        if (IsInRange(collisionDirection, new Vector3(-1f, -1f, -1f), new Vector3(1f, 0f, 0f))) // to dzia³a na przycisk S
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirection_S, ForceMode.Impulse);
        }
        if (IsInRange(collisionDirection, new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f))) // to dzia³a na przycisk D
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirection_D, ForceMode.Impulse);
        }
    }

    void FreezeAllAxes()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void UnfreezeRotation()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    bool IsInRange(Vector3 value, Vector3 min, Vector3 max)
    {
        return value.x >= min.x && value.x <= max.x &&
               value.y >= min.y && value.y <= max.y &&
               value.z >= min.z && value.z <= max.z;
    }

}

public enum Direction
{
    None,
    W,
    S,
    D,
    A
}
