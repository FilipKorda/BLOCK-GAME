using UnityEngine;
using static SoundManager;

public class Meta : VisableCollider
{
    [SerializeField] private Player player;
    [SerializeField] private DisappearAnimation disappearAnimation;
    [SerializeField] private SpiralMovement spiralMovement;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider metaCollider;


    private void Start()
    {
        metaCollider.enabled = false;
    }

    private void Update()
    {
        if (AreTransformsAtSamePosition(player.transform, metaCollider.transform))
        {
            metaCollider.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            if (player.gameObject.transform.position == metaCollider.transform.position)
            {
                player.canMove = false;
                Debug.Log("Win");
                CheckForMatch();
            }
        }
    }

    bool AreTransformsAtSamePosition(Transform t1, Transform t2)
    {
        return t1.position == t2.position;
    }

    private void CheckForMatch()
    {
        SoundManager.Instance.PlaySound(SoundType.WinSound);
        spiralMovement.Play();
        player.canMove = false;
        disappearAnimation.PlayDisappearAnimation();
    }

}
