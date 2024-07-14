using UnityEngine;

public class TwoCubesButton : VisableCollider
{
    [SerializeField] private Player player;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider twoCubeButtonCollider;
    [SerializeField] private TwoCubeController twoCubeController;

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
        player.gameObject.SetActive(false);
        twoCubeButtonCollider.enabled = true;
        twoCubeButtonCollider.isTrigger = false;
        twoCubeController.gameObject.SetActive(true);        
        Debug.Log("Create 2 cubes and swithc them on TAB to move around");
    }
}
