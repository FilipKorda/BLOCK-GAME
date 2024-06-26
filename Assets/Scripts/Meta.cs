using UnityEngine;

public class Meta : VisableCollider
{
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private Player player;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Transform targetPosition;
    [SerializeField] private SpiralMovement spiralMovement;
    [SerializeField] private DisappearAnimation disappearAnimation;

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
            Debug.Log("Go to the Next Level");
            spiralMovement.Play();
            disappearAnimation.PlayDisappearAnimationart();
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

}
