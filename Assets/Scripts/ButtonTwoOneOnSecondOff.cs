using System.Collections;
using UnityEngine;

public class ButtonTwoOneOnSecondOff : VisableCollider
{
    [SerializeField] private ButtonTwoOneOnSecondOff twinThisButton;
    [SerializeField] private Player player;

    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;

    public bool bridgeIsClosed = false;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider crossPlateCollider;

    [SerializeField] private float transitionDuration = 0.1f;
    [SerializeField] private float tolerance = 0.01f;

    private bool fixRotation = false;

    private void Start()
    {
        fixRotation = false;
        crossPlateCollider.enabled = false;
    }

    private void Update()
    {
        CheckPositionAndRotation();
    }

    void CheckPositionAndRotation()
    {
        if (Vector3.Distance(playerCollider.transform.position, crossPlateCollider.transform.position) < tolerance)
        {
            Quaternion rotation = player.transform.rotation;
            if (!bridgeIsClosed && !fixRotation && player.scale.x == 0.5 && player.scale.y == 1 && player.scale.z == 0.5 && !player.isRotating && player.totalRotation == 90)
            {
                rotation.w = 1;
                rotation.x = 0;
                rotation.y = 0;
                rotation.z = 0;

                player.transform.rotation = rotation;

                fixRotation = true;
            }

            if (!bridgeIsClosed && Quaternion.Angle(playerCollider.transform.rotation, crossPlateCollider.transform.rotation) < tolerance)
            {
                ToggleObject();
                bridgeIsClosed = true;
            }
        }
    }

    private IEnumerator MoveBridge()
    {
        Vector3 startPosition = originalPosition;
        Vector3 endPosition = destinationPosition;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            bridge.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

    private void ToggleObject()
    {
        if (resetCollider != null)
        {
            StartCoroutine(MoveBridge());
            resetCollider.SetActive(!resetCollider.activeSelf);
            twinThisButton.bridgeIsClosed = false;
            fixRotation = false;
        }
    }
}
