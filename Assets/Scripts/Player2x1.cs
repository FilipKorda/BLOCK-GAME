using System.Collections;
using UnityEngine;

public class Player2x1 : MonoBehaviour
{
    public float rollDuration = 1f;
    private bool isRolling;
    public Transform pivot;
    public Transform ghostPlayer;
    public LayerMask contactWallLayer;

    private void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        isRolling = false;
    }

    private void Update()
    {
        if (!isRolling)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Roll(Direction.Left);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Roll(Direction.Up);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Roll(Direction.Right);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Roll(Direction.Down);
            }
        }
    }

    private void Roll(Direction direction)
    {
        StartCoroutine(RollToDirection(direction));
    }

    private IEnumerator RollToDirection(Direction swipeDirection)
    {
        if (!isRolling)
        {
            isRolling = true;

            float angle = 90f;
            Vector3 axis = GetAxis(swipeDirection);
            Vector3 directionVector = GetDirectionVector(swipeDirection);
            Vector2 pivotOffset = GetPivotOffset(swipeDirection);

            pivot.position = transform.position + (directionVector * pivotOffset.x) + (Vector3.down * pivotOffset.y);

            CopyTransformData(transform, ghostPlayer);
            ghostPlayer.RotateAround(pivot.position, axis, angle);

            float elapsedTime = 0f;

            while (elapsedTime < rollDuration)
            {
                elapsedTime += Time.deltaTime;

                transform.RotateAround(pivot.position, axis, (angle * (Time.deltaTime / rollDuration)));
                yield return null;
            }

            CopyTransformData(ghostPlayer, transform);

            isRolling = false;
        }

    }

    public void CopyTransformData(Transform source, Transform target)
    {
        target.localPosition = source.localPosition;
        target.localEulerAngles = source.localEulerAngles;
    }

    private Vector3 GetAxis(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector3.forward;
            case Direction.Up:
                return Vector3.right;
            case Direction.Right:
                return Vector3.back;
            case Direction.Down:
                return Vector3.left;
            default:
                return Vector3.zero;
        }
    }

    private Vector3 GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                return Vector3.left;
            case Direction.Up:
                return Vector3.forward;
            case Direction.Right:
                return Vector3.right;
            case Direction.Down:
                return Vector3.back;
            default:
                return Vector3.zero;
        }
    }

    private Vector2 GetPivotOffset(Direction direction)
    {
        Vector2 pivotOffset = Vector2.zero;
        Vector2 center = transform.GetComponent<BoxCollider>().size / 2f;

        if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, 100f, contactWallLayer))
        {
            switch (hit.collider.name)
            {
                case "X":
                    if (direction == Direction.Left || direction == Direction.Right)
                        pivotOffset = new Vector2(center.y, center.x);
                    else
                        pivotOffset = Vector2.one * center.x;
                    break;
                case "Y":
                    pivotOffset = center;
                    break;
                case "Z":
                    if (direction == Direction.Up || direction == Direction.Down)
                        pivotOffset = new Vector2(center.y, center.x);
                    else
                        pivotOffset = Vector2.one * center.x;
                    break;
            }
        }
        return pivotOffset;
    }

    public enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }
}