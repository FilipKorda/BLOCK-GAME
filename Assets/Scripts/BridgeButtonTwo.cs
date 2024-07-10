using UnityEngine;

public class BridgeButtonTwo : VisableCollider
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject bridge;
    [SerializeField] private GameObject restetCollider;
    private bool bridgeIsOpen = false;
    private readonly float delayBeforeMatch = 0.11f;
    private float timeSinceMatched = 0f;
    private readonly float positionMarginOfError = 0.001f;
    private readonly float rotationMarginOfError = 0.001f;

    void Update()
    {
        if (!bridgeIsOpen && IsPositionMatched() && IsRotationMatched())
        {
            timeSinceMatched += Time.deltaTime;

            if (timeSinceMatched >= delayBeforeMatch)
            {
                CheckForMatch();
            }
        }
        else if (!IsPositionMatched() || !IsRotationMatched())
        {
            bridgeIsOpen = false;
        }
    }

    bool IsPositionMatched()
    {
        return Vector3.Distance(player.transform.position, transform.position) < positionMarginOfError;
    }

    bool IsRotationMatched()
    {
        Vector3 playerEulerAngles = player.transform.rotation.eulerAngles;

        bool isXMatched = IsWithinMargin(playerEulerAngles.x, 0) || IsWithinMargin(playerEulerAngles.x, 180);

        bool isYMatched = IsWithinMargin(playerEulerAngles.y, 0) || IsWithinMargin(playerEulerAngles.y, 90) ||
                          IsWithinMargin(playerEulerAngles.y, 180) || IsWithinMargin(playerEulerAngles.y, 270) ||
                          IsWithinMargin(playerEulerAngles.y, 360);

        return isXMatched && isYMatched;
    }

    bool IsWithinMargin(float angle, float target)
    {
        return Mathf.Abs(Mathf.DeltaAngle(angle, target)) < rotationMarginOfError;
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
