using DG.Tweening;
using System.Collections;
using UnityEngine;

public class DisappearAnimation : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float fallDownSpeed = 0.04f;

    public void PlayDisappearAnimation()
    {
        StartCoroutine(DealyDisappearAnimation());
    }

    private IEnumerator DealyDisappearAnimation()
    {
        yield return new WaitForSeconds(0.3f);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(player.transform.DOMoveY(-2.2f, fallDownSpeed));
        mySequence.Join(player.transform.DOScaleY(0, speed));
        mySequence.Append(player.transform.DOMoveY(-1f, speed));
        mySequence.SetEase(Ease.InOutQuad);
        mySequence.Play();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && player.transform.position.y < -2.199f)
        {
            Debug.Log("Collided with DisableSetActiveCollider and position Y is less than -2f");
            player.SetActive(false);
        }
    }

}
