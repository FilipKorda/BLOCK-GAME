using System.Collections;
using System.Linq;
using UnityEngine;

public class ResetCollider : MonoBehaviour
{
    [SerializeField] private GameObject ground;
    [SerializeField] private PlateMoveDown[] objectsToMove;
    [SerializeField] private LoadingSystem loadingSystem;
    [SerializeField] private Player player;
    private int previousStarCount = 0;


    private void Start()
    {
        if (GameManager.Instance != null)
        {
            previousStarCount = GameManager.Instance.starCount;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ResetDealy());

        player.gameObject.SetActive(false);

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(TriggerMovePlatesWithDelay());
        }

        if (GameManager.Instance != null)
        {
            int starDifference = GameManager.Instance.starCount - previousStarCount;

            if (starDifference > 0)
            {
                GameManager.Instance.starCount = Mathf.Max(0, GameManager.Instance.starCount - starDifference); 
            }

            previousStarCount = GameManager.Instance.starCount;
        }

    }

    private IEnumerator TriggerMovePlatesWithDelay()
    {
        PlateMoveDown[] shuffledObjects = objectsToMove.OrderBy(x => Random.value).ToArray();

        foreach (var obj in shuffledObjects)
        {
            obj.MovePlateDown();
            yield return new WaitForSeconds(0.01f);
        }
    }

    private IEnumerator ResetDealy()
    {
        yield return new WaitForSeconds(2f);
        loadingSystem.ResetThisLevel();
    }

    [ContextMenu("Add PlateMoveDown To Move Objects")]
    private void AddObjectsToMove()
    {
        if (ground == null)
        {
            Debug.LogError("Ground object is not assigned!");
            return;
        }

        // Pobierz wszystkie dzieci obiektu ground
        int childCount = ground.transform.childCount;
        objectsToMove = new PlateMoveDown[childCount];
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = ground.transform.GetChild(i).gameObject;

            if (child.TryGetComponent<PlateMoveDown>(out var plateMoveDown))
            {
                objectsToMove[i] = plateMoveDown;
            }
            else
            {
                Debug.LogWarning($"Child {child.name} does not have a PlateMoveDown component.");
            }
        }

        Debug.Log("Objects to move populated with PlateMoveDown components from children of ground.");
    }
}
