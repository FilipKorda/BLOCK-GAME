using UnityEngine;

public class Meta : VisableCollider
{
    [SerializeField] private GameObject targetPlayer;
    [SerializeField] private Player player;

    private Collider thisCollider;
    private Collider targetCollider;
    private DisappearAnimation disappearAnimation;
    private SpiralMovement spiralMovement;
    private LevelConector levelConector;
    private bool isMatched = false;

    void Start()
    {
        thisCollider = GetComponent<Collider>();
        targetCollider = targetPlayer.GetComponent<Collider>();
        if (transform.childCount > 0)
        {
            Transform childTransform = transform.GetChild(0);
            disappearAnimation = childTransform.GetComponent<DisappearAnimation>();
        }
        spiralMovement = GetComponent<SpiralMovement>();
        levelConector = GetComponent<LevelConector>();
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
            levelConector.LoadNextLexel();
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
