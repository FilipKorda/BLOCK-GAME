using UnityEngine;

public class BridgeButton : MonoBehaviour
{
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject restetCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleObject();
        }
    }

    private void ToggleObject()
    {
        if (bridge != null && restetCollider != null)
        {
            bridge.SetActive(!bridge.activeSelf);
            restetCollider.SetActive(!bridge.activeSelf);
        }
    }
}
