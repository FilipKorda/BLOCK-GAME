using System.Collections;
using UnityEngine;

public class CircleButtonOnlyOn : MonoBehaviour
{
    [SerializeField] private CircleButtonOnlyOn twinCircleButtonOnlyOn;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;
    [SerializeField] private float transitionDuration = 0.1f;
    public bool bridgeIsClosed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleObject();
        }
    }

    private void ToggleObject()
    {
        if (!bridgeIsClosed && bridge != null && resetCollider != null)
        {
            StartCoroutine(MoveBridge());
            resetCollider.SetActive(!resetCollider.activeSelf);
            twinCircleButtonOnlyOn.bridgeIsClosed = false;
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
        bridge.transform.localPosition = endPosition;
    }
}
