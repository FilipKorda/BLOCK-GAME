using UnityEngine;

public class BridgeButton : MonoBehaviour
{
    [SerializeField] private GameObject bridge;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ToggleObject();
        }
    }

    private void ToggleObject()
    {
        if (bridge != null)
        {
            bridge.SetActive(!bridge.activeSelf);
        }
    }
}
