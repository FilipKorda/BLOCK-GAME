using System.Collections;
using UnityEngine;

public class Meta : VisableCollider
{
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private Player player;
    [SerializeField] private SpiralMovement spiralMovement;
    [SerializeField] private DisappearAnimation disappearAnimation;

    private Collider thisCollider;
    private Collider targetCollider;
    private bool isMatched = false;

    void Start()
    {
        thisCollider = GetComponent<Collider>();
        targetCollider = targetPlayer.GetComponent<Collider>();
    }
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
        if (IsColliderMatched(thisCollider, targetCollider))
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
