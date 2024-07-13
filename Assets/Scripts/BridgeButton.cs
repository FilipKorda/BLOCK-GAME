using System.Collections;
using UnityEngine;

public class BridgeButton : MonoBehaviour
{
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;
    [SerializeField] private float transitionDuration = 0.1f;
    private bool isAtOriginalPosition = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleObject();
        }
    }

    private void ToggleObject()
    {
        if (bridge != null && resetCollider != null)
        {
            StartCoroutine(MoveBridge());
            resetCollider.SetActive(!resetCollider.activeSelf);
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
}
