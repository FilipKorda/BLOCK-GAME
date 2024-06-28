using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetCollider : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlateMoveDown[] objectsToMove;

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(ResetDealy());

        player.SetActive(false);

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(TriggerMovePlatesWithDelay());
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
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
