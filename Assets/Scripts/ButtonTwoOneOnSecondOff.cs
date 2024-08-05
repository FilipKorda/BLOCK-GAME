using System.Collections;
using UnityEngine;

public class ButtonTwoOneOnSecondOff : VisableCollider
{
    [SerializeField] private ButtonTwoOneOnSecondOff twinThisButton;
    [SerializeField] private Transform player;

    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;

    public bool bridgeIsClosed = false;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider crossPlateCollider;

    [SerializeField] private float transitionDuration = 0.1f;

    private void Start()
    {
        crossPlateCollider.enabled = false;
    }

    private void Update()
    {
        if (AreTransformsAtSamePosition(player, crossPlateCollider.transform))
        {
            crossPlateCollider.enabled = true;
        }
        else
        {
            crossPlateCollider.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!bridgeIsClosed && other == playerCollider)
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
            crossPlateCollider.enabled = false;
        }
    }

    private IEnumerator MoveBridge()
    {
        bridgeIsClosed = true;
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

    bool AreTransformsAtSamePosition(Transform t1, Transform t2)
    {
        return t1.position == t2.position;
    }

    private void ToggleObject()
    {
        if (resetCollider != null)
        {
            StartCoroutine(MoveBridge());
            resetCollider.SetActive(!resetCollider.activeSelf);
            twinThisButton.bridgeIsClosed = false;
        }
    }
}
