using DG.Tweening;
using UnityEngine;

public class PlateMoveDown : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private float moveDownDistance = 15f;
    [SerializeField] private float moveDownDuration = 10f;

    public void MovePlateDown()
    {
        Vector3 endPosition = targetObject.position - Vector3.up * moveDownDistance;
        targetObject.DOMove(endPosition, moveDownDuration);
    }
}
