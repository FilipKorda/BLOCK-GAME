using UnityEngine;

public class BridgeButtonTwo : VisableCollider
{
    [SerializeField] private Transform targetPlayer;
    [SerializeField] private Player player;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject restetCollider;
    private Collider thisCollider;
    private Collider targetCollider;
    private bool isBridgeOpened = false;  // Flaga do kontrolowania stanu mostu

    void Start()
    {
        thisCollider = GetComponent<Collider>();
        targetCollider = targetPlayer.GetComponent<Collider>();
    }

    void Update()
    {
        CheckForMatch();
    }

    void CheckForMatch()
    {
        if (IsColliderMatched(thisCollider, targetCollider))
        {
            if (!isBridgeOpened)
            {
                Debug.Log("You open bridge");
                ToggleObject();
                isBridgeOpened = true; 
            }
        }
        else
        {
            isBridgeOpened = false; 
        }
    }

    bool IsColliderMatched(Collider col1, Collider col2)
    {
        return col1.bounds.center == col2.bounds.center && col1.bounds.size == col2.bounds.size;
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
