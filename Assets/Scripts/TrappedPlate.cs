using UnityEngine;

public class TrappedPlate : VisableCollider
{
    [SerializeField] private GameObject targetPlate;
    [SerializeField] private Transform targetPlatePosition;
    [SerializeField] private float moveSpeedTrappedPlate = 6f;

    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private float moveSpeed = 4f;

    [SerializeField] private Player player;
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
            MovePlateDown();
            MovePlayerDown();
        }
    }
    void CheckForMatch()
    {
        if (IsColliderMatched(thisCollider, targetCollider))
        {
            Debug.Log("You got trapped"); 
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
    void MovePlateDown()
    {
        float step = moveSpeedTrappedPlate * Time.deltaTime;
        targetPlate.transform.position = Vector3.Lerp(targetPlate.transform.position, targetPlatePosition.position, step);
    }
    void MovePlayerDown()
    {
        float step = moveSpeed * Time.deltaTime;
        targetPlayer.transform.position = Vector3.Lerp(targetPlayer.transform.position, targetPosition.position, step);
    }
}
