using UnityEngine;

public class TwoCubesButton : VisableCollider
{
    [SerializeField] private Player player;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private GameObject twoCubeController;
    [SerializeField] private CubeMovement cube1Movement;
    public Collider twoCubeButtonCollider;

    private void Start()
    {
        twoCubeButtonCollider.enabled = false;
    }

    private void Update()
    {
        if (AreTransformsAtSamePosition(player.gameObject.transform, twoCubeButtonCollider.gameObject.transform))
        {
            twoCubeButtonCollider.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
        {
            SplitObject();
        }
    }

    bool AreTransformsAtSamePosition(Transform t1, Transform t2)
    {
        return t1.position == t2.position;
    }

    private void SplitObject()
    {
        SoundManager.Instance.PlaySound(SoundClip.ConnectTwoCubesSound);
        cube1Movement.enabled = true;
        player.gameObject.SetActive(false);
        twoCubeButtonCollider.isTrigger = false;
        twoCubeButtonCollider.enabled = false;
        twoCubeController.SetActive(true);
        Debug.Log("Create 2 cubes and swithc them on TAB to move around");
    }
}
