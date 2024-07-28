using UnityEngine;

public class Meta : VisableCollider
{
    [SerializeField] private Player player;
    [SerializeField] private DisappearAnimation disappearAnimation;
    [SerializeField] private SpiralMovement spiralMovement;

    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider metaCollider;
    [SerializeField] private float tolerance = 0.01f;
    private bool fixRotation = false;
    private bool youWin = false;


    private void Awake()
    {
        fixRotation = false;
        metaCollider.enabled = false;
    }

    private void Update()
    {
        CheckPositionAndRotation();
    }

    void CheckPositionAndRotation()
    {
        if (Vector3.Distance(playerCollider.transform.position, metaCollider.transform.position) < tolerance)
        {
            Quaternion rotation = player.transform.rotation;
            if (!fixRotation && player.scale.x == 0.5 && player.scale.y == 1 && player.scale.z == 0.5 && !player.isRotating && player.totalRotation == 90)
            {
                rotation.w = 1;
                rotation.x = 0;
                rotation.y = 0;
                rotation.z = 0;

                player.transform.rotation = rotation;

                fixRotation = true;
            }

            if (!youWin && Quaternion.Angle(playerCollider.transform.rotation, metaCollider.transform.rotation) < tolerance)
            {
           
                CheckForMatch();
                youWin = true;

            }
        }
    }

    private void CheckForMatch()
    {
        SoundManager.Instance.PlaySound(SoundClip.WinGameSound);
        Debug.Log(SoundClip.WinGameSound);
        spiralMovement.Play();
        player.canMove = false;   
        disappearAnimation.PlayDisappearAnimation();
        Debug.Log("Win");
    }

}
