using UnityEngine;

public class Meta : VisableCollider
{
    [SerializeField] private Player player;
    [SerializeField] private DisappearAnimation disappearAnimation;
    [SerializeField] private SpiralMovement spiralMovement;
    private readonly float positionMarginOfError = 0.001f;
    private readonly float rotationMarginOfError = 0.001f;
    private readonly float delayBeforeMatch = 0.11f;
    private float timeSinceMatched = 0f;
    private bool isMatched;

    void Update()
    {
        if (IsPositionMatched() && IsRotationMatched())
        {
            timeSinceMatched += Time.deltaTime;
            if (!isMatched && timeSinceMatched >= delayBeforeMatch)
            {
                CheckForMatch();
            }
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
        Debug.Log("Go to the Next Level");
        spiralMovement.Play();
        player.canMove = false;
        disappearAnimation.PlayDisappearAnimation();
        isMatched = true;
    }

}
