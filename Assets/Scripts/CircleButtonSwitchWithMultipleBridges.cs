using System.Collections;
using UnityEngine;

public class CircleButtonSwitchWithMultipleBridges : MonoBehaviour
{
    [SerializeField] private CircleButtonSwitchWithMultipleBridges otherCircleButton;
    [SerializeField] private CircleButtonSwitchWithMultipleBridges otherCircleButton1;
    [SerializeField] private CircleButtonSwitchWithMultipleBridges otherCircleButton2;

    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject resetCollider;
    [SerializeField] private Vector3 originalPosition;
    [SerializeField] private Vector3 destinationPosition;

    [SerializeField] private GameObject bridge2;
    [SerializeField] private GameObject resetCollider2;
    [SerializeField] private Vector3 originalPositionBridge2;
    [SerializeField] private Vector3 destinationPositionBridge2;

    [SerializeField] private float transitionDuration = 0.1f;

    public bool isBridgeOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (!isBridgeOpen && other.CompareTag("Player"))
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

            if(otherCircleButton.isBridgeOpen)
            {
                otherCircleButton.isBridgeOpen = false;
                otherCircleButton1.isBridgeOpen = true;
                otherCircleButton2.isBridgeOpen = true;
            }
            else
            {
                otherCircleButton.isBridgeOpen = true;
                otherCircleButton1.isBridgeOpen = false;
                otherCircleButton2.isBridgeOpen = false;
            }
        }
    }

    private IEnumerator MoveBridge()
    {
        Vector3 startPosition =  originalPosition;
        Vector3 endPosition =  destinationPosition;

        Vector3 startPositionBridge2 =  originalPositionBridge2 ;
        Vector3 endPositionBridge2 =  destinationPositionBridge2;

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
    }
}
