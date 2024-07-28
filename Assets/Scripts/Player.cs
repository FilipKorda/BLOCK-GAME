using UnityEngine;

public class Player : MonoBehaviour
{
    public FallingForceApplier.DirectionForceApplier rotationDirection;
    private readonly float rotationSpeed = 750f;
    public float totalRotation;
    public bool isRotating;

    public Vector3 pivot, axis, scale;
    [SerializeField] private Rigidbody rb;

    [Header("Player Management")]
    public bool canMove;

    [Header("Layer Mask of Invisible Wall")]
    [SerializeField] private LayerMask collisionLayerMask;

    [SerializeField] private MoveTracker moveTracker;
    [SerializeField] private StepSoundManager stepSoundManager;

    void Start()
    {
        isRotating = false;
        canMove = true;
        scale = transform.localScale / 2f;
        rb = GetComponent<Rigidbody>();
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
                if ((rotationDirection == FallingForceApplier.DirectionForceApplier.A) || (rotationDirection == FallingForceApplier.DirectionForceApplier.W))
                    transform.RotateAround(pivot, axis, deltaRotation);
                else transform.RotateAround(pivot, axis, -deltaRotation);
                totalRotation += deltaRotation;
            }
            else if (Input.GetKeyDown(KeyCode.W)) Rotate(FallingForceApplier.DirectionForceApplier.W);
            else if (Input.GetKeyDown(KeyCode.A)) Rotate(FallingForceApplier.DirectionForceApplier.A);
            else if (Input.GetKeyDown(KeyCode.S)) Rotate(FallingForceApplier.DirectionForceApplier.S);
            else if (Input.GetKeyDown(KeyCode.D)) Rotate(FallingForceApplier.DirectionForceApplier.D);
        }
        
    }

    void Rotate(FallingForceApplier.DirectionForceApplier direction)
    {
        rotationDirection = direction;
        isRotating = true;
        totalRotation = 0f;
      
        switch (rotationDirection)
        {
            case FallingForceApplier.DirectionForceApplier.D:
                pivot = transform.position + new Vector3(scale.x, -scale.y, 0);
                break;
            case FallingForceApplier.DirectionForceApplier.A:
                pivot = transform.position + new Vector3(-scale.x, -scale.y, 0);
                break;
            case FallingForceApplier.DirectionForceApplier.W:
                pivot = transform.position + new Vector3(0, -scale.y, scale.z);
                break;
            case FallingForceApplier.DirectionForceApplier.S:
                pivot = transform.position + new Vector3(0, -scale.y, -scale.z);
                break;
        }
        if ((rotationDirection == FallingForceApplier.DirectionForceApplier.D) || (rotationDirection == FallingForceApplier.DirectionForceApplier.A))
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
        rb.useGravity = true;
        canMove = false;
    }
}

