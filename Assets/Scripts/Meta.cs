using UnityEngine;

public class Meta : VisableCollider
{
    [SerializeField] private Player player;
    [SerializeField] private Collider thisCollider;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private DisappearAnimation disappearAnimation;
    [SerializeField] private SpiralMovement spiralMovement;
    private bool isMatched = false;

    void Update()
    {
        if (!isMatched)
        {
            CheckForMatch();
        }
        else
        {
            disappearAnimation.PlayDisappearAnimation();

        }
    }
    void CheckForMatch()
    {
        if (IsColliderMatched(thisCollider, playerCollider))
        {
            Debug.Log("Go to the Next Level");
            spiralMovement.Play();
            player.canMove = false;
            isMatched = true;
        }
        else
        {
            isMatched = false;
        }
    }
    bool IsColliderMatched(Collider col1, Collider col2)
    {
        return col1.bounds.center == col2.bounds.center && col1.bounds.size == col2.bounds.size;
    }
}
