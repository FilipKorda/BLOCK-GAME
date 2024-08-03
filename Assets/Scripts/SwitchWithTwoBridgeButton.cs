using System.Collections;
using UnityEngine;

public class SwitchWithTwoBridgeButton : MonoBehaviour
{
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;

    [SerializeField] private GameObject bridge2;
    [SerializeField] private GameObject resetCollider2;
    [SerializeField] private Vector3 originalPositionBridge2;
    [SerializeField] private Vector3 destinationPositionBridge2;

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
        if (resetCollider != null)
        {
            StartCoroutine(MoveBridge());
            resetCollider.SetActive(!resetCollider.activeSelf);
            if (resetCollider2 != null)
                resetCollider2.SetActive(!resetCollider2.activeSelf);
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

            if (bridge2 != null)
            {
                bridge2.transform.localPosition = Vector3.Lerp(startPositionBridge2, endPositionBridge2, elapsedTime / transitionDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bridge.transform.localPosition = endPosition;

        if (bridge2 != null)
        {
            bridge2.transform.localPosition = endPositionBridge2;
        }

        isAtOriginalPosition = !isAtOriginalPosition;
    }
}
