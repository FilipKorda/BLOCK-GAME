using System.Collections;
using UnityEngine;

public class BridgeButtonTwo : VisableCollider
{
    [SerializeField] private Player player;

    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;

    public bool shouldOpenTwoBridges;
    [SerializeField] private GameObject bridge2;
    [SerializeField] private GameObject resetCollider2;
    [SerializeField] private Vector3 originalPositionBridge2;
    [SerializeField] private Vector3 destinationPositionBridge2;

    public bool isBridgeOpen = false;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider crossPlateCollider;

    [SerializeField] private float transitionDuration = 0.1f;
    private bool isAtOriginalPosition = true;

    [SerializeField] private float tolerance = 0.01f;
    private bool fixRotation = false;
    private bool xxxxxxx = false;

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
            if (!fixRotation && player.scale.x == 0.5 && player.scale.y == 1 && player.scale.z == 0.5 && !player.isRotating && player.totalRotation == 90)
            {
                rotation.w = 1;
                rotation.x = 0;
                rotation.y = 0;
                rotation.z = 0;

                player.transform.rotation = rotation;

                fixRotation = true;
            }

            if (!isBridgeOpen && Quaternion.Angle(playerCollider.transform.rotation, crossPlateCollider.transform.rotation) < tolerance)
            {
                crossPlateCollider.enabled = true;
                isBridgeOpen = true;
                xxxxxxx = false;
                Debug.Log("Open Bridge");
                ToggleObject();
            }         
        }

        if(!xxxxxxx && Vector3.Distance(playerCollider.transform.position, crossPlateCollider.transform.position) > tolerance)
        {
            Debug.Log("Close Bridge");
            xxxxxxx = true;
            isBridgeOpen = false;
            crossPlateCollider.enabled = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == playerCollider)
        {
           
        }
    }

    private IEnumerator MoveBridge()
    {
        Vector3 startPosition = isAtOriginalPosition ? originalPosition : destinationPosition;
        Vector3 endPosition = isAtOriginalPosition ? destinationPosition : originalPosition;

        Vector3 startPositionBridge2 = isAtOriginalPosition ? originalPositionBridge2 : destinationPositionBridge2;
        Vector3 endPositionBridge2 = isAtOriginalPosition ? destinationPositionBridge2 : originalPositionBridge2;

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            bridge.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);

            if (shouldOpenTwoBridges && bridge2 != null)
            {
                bridge2.transform.localPosition = Vector3.Lerp(startPositionBridge2, endPositionBridge2, elapsedTime / transitionDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bridge.transform.localPosition = endPosition;

        if (shouldOpenTwoBridges && bridge2 != null)
        {
            bridge2.transform.localPosition = endPositionBridge2;
        }

        isAtOriginalPosition = !isAtOriginalPosition;
    }

    private void ToggleObject()
    {
        if (resetCollider != null)
        {
            StartCoroutine(MoveBridge());
            resetCollider.SetActive(!resetCollider.activeSelf);
            if (resetCollider2 != null)
                resetCollider2.SetActive(!resetCollider2.activeSelf);
        }
    }
}
