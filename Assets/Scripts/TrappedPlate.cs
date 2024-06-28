using UnityEngine;
using DG.Tweening;
public class TrappedPlate : VisableCollider
{
   
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private float moveDistance = 5f; 
    [SerializeField] private float moveDuration = 1f;

    [SerializeField] private Transform targetPlate;
    [SerializeField] private float movePlateDistance = 2f;
    [SerializeField] private float movePlateDuration = 1f;
    [SerializeField] private float rotateDuration = 1f;

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
        Vector3 endPosition = targetPlate.position - Vector3.up * movePlateDistance;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(targetPlate.DOMove(endPosition, movePlateDuration)); 
        sequence.Join(targetPlate.DORotate(new Vector3(180f, 180f, -180f), rotateDuration, RotateMode.LocalAxisAdd));
        sequence.Play();
    }
    void MovePlayerDown()
    {
        Vector3 endPosition = targetPlayer.position - Vector3.up * moveDistance;
        targetPlayer.DOMove(endPosition, moveDuration);
    }
}
