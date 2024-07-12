using UnityEngine;

public class BridgeButtonTwo : VisableCollider
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject restetCollider;
    private bool bridgeIsOpen = false;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider crossPlateCollider;

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            if (player.transform.position == crossPlateCollider.transform.position)
            {
                CheckForMatch();
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

    bool AreTransformsAtSamePosition(Transform t1, Transform t2)
    {
        return t1.position == t2.position;
    }

    private void CheckForMatch()
    {
        ToggleObject();
        bridgeIsOpen = true;
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
