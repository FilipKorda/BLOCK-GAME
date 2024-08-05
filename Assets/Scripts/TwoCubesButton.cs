using System.Collections;
using UnityEngine;

public class TwoCubesButton : VisableCollider
{
    // ==== Instruction for SetUp Cubes Mechnics ====
    // - To this twoCubeController assigned a TwoCubeController GameObject

    [SerializeField] private Player player;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private GameObject twoCubeController;
    [SerializeField] private Player cube1Movement;
    public Collider twoCubeButtonCollider;

    [SerializeField] private float tolerance = 0.01f;
    public bool fixRotation = false;
    public bool youMatched = false;

    private void Start()
    {
        fixRotation = false;
        youMatched = false;
        twoCubeButtonCollider.enabled = false;
    }

    private void Update()
    {
        CheckPositionAndRotation();
    }

    void CheckPositionAndRotation()
    {
        if (Vector3.Distance(playerCollider.transform.position, twoCubeButtonCollider.transform.position) < tolerance)
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

            if (!youMatched && Quaternion.Angle(playerCollider.transform.rotation, twoCubeButtonCollider.transform.rotation) < tolerance)
            {
                twoCubeButtonCollider.enabled = true;
                SplitObject();
                Debug.Log("!!!!!");
                youMatched = true;
            
            }
        }
    }

    private void SplitObject()
    {
        SoundManager.Instance.PlaySound(SoundClip.ConnectTwoCubesSound);
        cube1Movement.enabled = true;
        player.gameObject.SetActive(false);
        twoCubeButtonCollider.isTrigger = false;
        twoCubeButtonCollider.enabled = false;     
        StartCoroutine(DelaySpawnTwoCubes());
        Debug.Log("Create 2 cubes and swithc them on TAB to move around");
    }

    private IEnumerator DelaySpawnTwoCubes()
    {
        if (twoCubeController != null && twoCubeController != null)
        {
            yield return new WaitForSeconds(0.15f);
            fixRotation = false;
            twoCubeButtonCollider.isTrigger = true;
            yield return new WaitForSeconds(0.35f);
            twoCubeController.SetActive(true);
        }
    }
}
