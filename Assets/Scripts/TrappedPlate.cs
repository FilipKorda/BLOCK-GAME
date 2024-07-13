using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class TrappedPlate : VisableCollider
{
    [SerializeField] private float moveDistance = 20f;
    [SerializeField] private float moveDuration = 2f;
    [SerializeField] private GameObject targetPlate;
    [SerializeField] private float movePlateDistance = 21f;
    [SerializeField] private float movePlateDuration = 2f;
    [SerializeField] private float rotateDuration = 10f;

    [SerializeField] private Player player;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider trappedPlateCollider;
    private bool colliderMatech;

    private void Start()
    {
        trappedPlateCollider.enabled = false;
    }

    private void Update()
    {
        if (!colliderMatech && AreTransformsAtSamePosition(player.transform, trappedPlateCollider.transform))
        {
            trappedPlateCollider.enabled = true;
            colliderMatech = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            if (player.gameObject.transform.position == trappedPlateCollider.transform.position)
            {
                player.canMove = false;
                Debug.Log("spadasz");
                MovePlateDown();
                MovePlayerDown();
            }
        }
    }

    bool AreTransformsAtSamePosition(Transform t1, Transform t2)
    {
        return t1.position == t2.position;
    }

    void MovePlateDown()
    {
        Vector3 endPosition = targetPlate.transform.position - Vector3.up * movePlateDistance;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(targetPlate.transform.DOMove(endPosition, movePlateDuration));
        sequence.Join(targetPlate.transform.DORotate(new Vector3(180f, 180f, -180f), rotateDuration, RotateMode.LocalAxisAdd));
        sequence.Play();
    }
    void MovePlayerDown()
    {
        Vector3 endPosition = player.transform.position - Vector3.up * moveDistance;
        player.transform.DOMove(endPosition, moveDuration);
    }
}
