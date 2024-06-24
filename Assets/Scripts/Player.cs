using UnityEngine;

public class Player : MonoBehaviour
{
    private readonly float rotationSpeed = 750f;
    private float totalRotation;
    private bool isMoving;
    private bool canMove;
    private Direction rotationDirection;
    private Vector3 pivot, axis, scale;
    private Rigidbody rb;

    private Vector3 forceDirectionNorth = new Vector3(0, -5, 5);
    private Vector3 forceDirectionSouth = new Vector3(0, -5, -5);
    private Vector3 forceDirectionEast = new Vector3(5, -5, 0);
    private Vector3 forceDirectionWest = new Vector3(-5, -5, 0);

    void Start()
    {
        isMoving = false;
        canMove = true;
        scale = transform.localScale / 2f;
        FreezeAllAxes();
    }

    void Update()
    {
        if(canMove)
        {
            if (isMoving)
            {
                float deltaRotation = rotationSpeed * Time.deltaTime;
                if (totalRotation + deltaRotation >= 90f)
                {
                    deltaRotation = 90f - totalRotation;
                    isMoving = false;
                }
                if ((rotationDirection == Direction.West) || (rotationDirection == Direction.North))
                    transform.RotateAround(pivot, axis, deltaRotation);
                else transform.RotateAround(pivot, axis, -deltaRotation);
                totalRotation += deltaRotation;
            }
            else if (Input.GetKeyDown(KeyCode.W)) Rotate(Direction.North);
            else if (Input.GetKeyDown(KeyCode.A)) Rotate(Direction.West);
            else if (Input.GetKeyDown(KeyCode.S)) Rotate(Direction.South);
            else if (Input.GetKeyDown(KeyCode.D)) Rotate(Direction.East);
            if (!isMoving)
            {
              //  SnapPosition();
            }
        }
      
    }

    void Rotate(Direction direction)
    {
        rotationDirection = direction;
        isMoving = true;
        totalRotation = 0f;
        switch (rotationDirection)
        {
            case Direction.East:
                pivot = transform.position + new Vector3(scale.x, -scale.y, 0);
                break;
            case Direction.West:
                pivot = transform.position + new Vector3(-scale.x, -scale.y, 0);
                break;
            case Direction.North:
                pivot = transform.position + new Vector3(0, -scale.y, scale.z);
                break;
            case Direction.South:
                pivot = transform.position + new Vector3(0, -scale.y, -scale.z);
                break;


        }
        if ((rotationDirection == Direction.East) || (rotationDirection == Direction.West))
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
    void SnapPosition()
    {
        Vector3 newPosition = transform.position;

        newPosition.x = Mathf.Round(newPosition.x);
        newPosition.z = Mathf.Round(newPosition.z);

        newPosition.y = -scale.y;

        transform.position = newPosition;
    }


    void OnCollisionEnter(Collision collision)
    {
        Vector3 collisionDirection = rb.velocity.normalized;
        Debug.Log("Collision Direction: " + collisionDirection);

        if (collisionDirection == Direction.North)
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirectionNorth * 10, ForceMode.Impulse);
        }
        else if (collisionDirection == Direction.South)
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirectionSouth * 10, ForceMode.Impulse);
        }
        else if (collisionDirection == Direction.West)
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirectionWest * 10, ForceMode.Impulse);
        }
        else if (collisionDirection == Direction.East)
        {
            canMove = false;
            UnfreezeRotation();
            rb.AddForce(forceDirectionEast * 10, ForceMode.Impulse);
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

}

public enum Direction
{
    North,
    South,
    East,
    West
}
