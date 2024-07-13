using System.Collections;
using UnityEngine;

public class BridgeButtonTwo : VisableCollider
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    public bool bridgeIsOpen = false;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider crossPlateCollider;

    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;
    [SerializeField] private float transitionDuration = 0.1f;
    private bool isAtOriginalPosition = true;

    private void Start()
    {
        crossPlateCollider.enabled = false;
    }

    private void Update()
    {
        if (AreTransformsAtSamePosition(player, crossPlateCollider.transform))
        {
            crossPlateCollider.enabled = true;
            bridgeIsOpen = true;
        }
        if (bridgeIsOpen && AreTransformsAtSamePosition(player, crossPlateCollider.transform))
        {
            crossPlateCollider.enabled = true;
            bridgeIsOpen = false;
        }
        else
        {
            crossPlateCollider.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            if (player.transform.position == crossPlateCollider.transform.position)
            {
                ToggleObject();
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other == playerCollider)
        {
            Debug.Log("xd");
            crossPlateCollider.enabled = false;
        }
    }

    private IEnumerator MoveBridge()
    {
        Vector3 startPosition = isAtOriginalPosition ? originalPosition : destinationPosition;
        Vector3 endPosition = isAtOriginalPosition ? destinationPosition : originalPosition;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            bridge.transform.localPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bridge.transform.localPosition = endPosition;
        isAtOriginalPosition = !isAtOriginalPosition;
    }

    bool AreTransformsAtSamePosition(Transform t1, Transform t2)
    {
        return t1.position == t2.position;
    }

    private void ToggleObject()
    {
        if (bridge != null && resetCollider != null)
        {
            StartCoroutine(MoveBridge());
            resetCollider.SetActive(!resetCollider.activeSelf);
        }
    }
}
