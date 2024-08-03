using UnityEngine;

public class Player : MonoBehaviour
{
    private DirectionForceApplier rotationDirection;
    private readonly float rotationSpeed = 800f;
    public float totalRotation;
    public bool isRotating;

    public Vector3 pivot, axis, scale;
    [SerializeField] private Rigidbody rb;

    [Header("Player Management")]
    public bool canMove;
    private bool shouldCorrectRotation;
    private bool isFalling;

    [Header("Layer Mask of Invisible Wall")]
    [SerializeField] private LayerMask collisionLayerMask;

    [SerializeField] private MoveTracker moveTracker;
    [SerializeField] private StepSoundManager stepSoundManager;

    [SerializeField] private Transform targetWhenFall;
    [SerializeField] private float attractionForce = 10f;



    void Start()
    {
        isRotating = false;
        canMove = true;
        isFalling = false;
        shouldCorrectRotation = true;
        scale = transform.localScale / 2f;
        rb = GetComponent<Rigidbody>();
        FreezeAllAxes();
    }

    void Update()
    {
        if (isFalling && targetWhenFall != null)
        {
            Vector3 direction = targetWhenFall.position - transform.position;
            direction.Normalize();
            rb.AddForce(direction * attractionForce);
        }

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
                if ((rotationDirection == DirectionForceApplier.A) || (rotationDirection == DirectionForceApplier.W))
                {
                    transform.RotateAround(pivot, axis, deltaRotation);
                }
                else
                {
                    transform.RotateAround(pivot, axis, -deltaRotation);
                }

                totalRotation += deltaRotation;
            }
            else if (Input.GetKeyDown(KeyCode.W)) Rotate(DirectionForceApplier.W);
            else if (Input.GetKeyDown(KeyCode.A)) Rotate(DirectionForceApplier.A);
            else if (Input.GetKeyDown(KeyCode.S)) Rotate(DirectionForceApplier.S);
            else if (Input.GetKeyDown(KeyCode.D)) Rotate(DirectionForceApplier.D);
        }
        if (!isRotating)
        {
            CorrectRotationIfUpsideDown();
        }

    }

    private void CorrectRotationIfUpsideDown()
    {
        if (shouldCorrectRotation)
        {
            Vector3 currentRotation = transform.eulerAngles;
            if (Mathf.Abs(currentRotation.x - 180) < 1f || Mathf.Abs(currentRotation.z - 180) < 1f || Mathf.Abs(currentRotation.x - 360) < 1f || Mathf.Abs(currentRotation.z - 360) < 1f)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    void Rotate(DirectionForceApplier direction)
    {
        rotationDirection = direction;
        isRotating = true;
        totalRotation = 0f;

        switch (rotationDirection)
        {
            case DirectionForceApplier.D:
                pivot = transform.position + new Vector3(scale.x, -scale.y, 0);
                break;
            case DirectionForceApplier.A:
                pivot = transform.position + new Vector3(-scale.x, -scale.y, 0);
                break;
            case DirectionForceApplier.W:
                pivot = transform.position + new Vector3(0, -scale.y, scale.z);
                break;
            case DirectionForceApplier.S:
                pivot = transform.position + new Vector3(0, -scale.y, -scale.z);
                break;
        }
        if ((rotationDirection == DirectionForceApplier.D) || (rotationDirection == DirectionForceApplier.A))
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

    public void FreezeAllAxes()
    {
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void PreparPlayerToFallDown()
    {
        rb.constraints = RigidbodyConstraints.None;
        shouldCorrectRotation = false;
        rb.useGravity = true;
        canMove = false;
        isFalling = true;

        Vector3 newCenterOfMass = rb.centerOfMass;
        newCenterOfMass.y += 1;
        rb.centerOfMass = newCenterOfMass;

        if (targetWhenFall != null)
        {
            Vector3 targetPosition = transform.position;
            targetPosition.y -= 10;
            targetWhenFall.position = targetPosition;
        }
    }

}

public enum DirectionForceApplier
{
    None,
    W,
    S,
    A,
    D
}
