using UnityEngine;

public class Meta : MonoBehaviour
{
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private Player player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private Transform targetPosition;
    private Collider thisCollider;
    private Collider targetCollider;
    private bool isMatched = false;

    void Start()
    {
        thisCollider = GetComponent<Collider>();
        targetCollider = targetPlayer.GetComponent<Collider>();
    }

    void Update()
    {
        if (!isMatched)
        {
            CheckForMatch();
        }
        else
        {
            MovePlayerDown();
        }
    }

    void CheckForMatch()
    {
        if (IsColliderMatched(thisCollider, targetCollider))
        {
            Debug.Log("Go to next level!");
            player.canMove = false;
            isMatched = true;
        }
        else
        {
            isMatched = false;
        }
    }

    bool IsColliderMatched(Collider col1, Collider col2)
    {
        return col1.bounds.center == col2.bounds.center && col1.bounds.size == col2.bounds.size;
    }

    void MovePlayerDown()
    {
        float step = moveSpeed * Time.deltaTime;
        targetPlayer.transform.position = Vector3.Lerp(targetPlayer.transform.position, targetPosition.position, step);
    }

    //Draw meta gizmos
    private void OnDrawGizmos()
    {
        Transform tr = transform;

        Vector3[] vertices = new Vector3[]
        {
            tr.TransformPoint(new Vector3(-0.5f, 0.5f, -0.5f)),   // 0
            tr.TransformPoint(new Vector3(0.5f, 0.5f, -0.5f)),    // 1
            tr.TransformPoint(new Vector3(0.5f, -0.5f, -0.5f)),   // 2
            tr.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f)),  // 3
            tr.TransformPoint(new Vector3(-0.5f, 0.5f, 0.5f)),    // 4
            tr.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f)),     // 5
            tr.TransformPoint(new Vector3(0.5f, -0.5f, 0.5f)),    // 6
            tr.TransformPoint(new Vector3(-0.5f, -0.5f, 0.5f))    // 7
        };

        DrawEdge(vertices[0], vertices[1]);
        DrawEdge(vertices[1], vertices[2]);
        DrawEdge(vertices[2], vertices[3]);
        DrawEdge(vertices[3], vertices[0]);

        DrawEdge(vertices[4], vertices[5]);
        DrawEdge(vertices[5], vertices[6]);
        DrawEdge(vertices[6], vertices[7]);
        DrawEdge(vertices[7], vertices[4]);

        DrawEdge(vertices[0], vertices[4]);
        DrawEdge(vertices[1], vertices[5]);
        DrawEdge(vertices[2], vertices[6]);
        DrawEdge(vertices[3], vertices[7]);
    }
    void DrawEdge(Vector3 startPoint, Vector3 endPoint)
    {
        Debug.DrawLine(startPoint, endPoint, Color.blue, 0f, false);
    }

}
