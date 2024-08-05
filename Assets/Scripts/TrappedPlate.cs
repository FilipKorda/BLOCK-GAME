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

    [SerializeField] private float tolerance = 0.01f;
    private bool fixRotation = false;

    private void Start()
    {
        fixRotation = false;
        trappedPlateCollider.enabled = false;
    }

    private void Update()
    {
        CheckPositionAndRotation();
    }

    void CheckPositionAndRotation()
    {
        if (Vector3.Distance(playerCollider.transform.position, trappedPlateCollider.transform.position) < tolerance)
        {
            Quaternion rotation = player.transform.rotation;
            if (!fixRotation && player.scale.x == 0.5 && player.scale.y == 1 && player.scale.z == 0.5 && !player.isRotating && player.totalRotation == 90)
            {
                rotation.w = 1;
                rotation.x = 0;
                rotation.y = 0;
                rotation.z = 0;

                player.transform.rotation = rotation;

                fixRotation = true;
            }

            if (!colliderMatech && Quaternion.Angle(playerCollider.transform.rotation, trappedPlateCollider.transform.rotation) < tolerance)
            {
                trappedPlateCollider.enabled = true;
                colliderMatech = true;
                CheckForMatch();
            }
        }
    }

    private void CheckForMatch()
    {
        player.canMove = false;
        Debug.Log("spadasz");
        MovePlateDown();
        MovePlayerDown();
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
