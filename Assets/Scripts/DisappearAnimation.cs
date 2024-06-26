using UnityEngine;
using System.Collections;

public class DisappearAnimation : MonoBehaviour
{
    public float duration = 1.0f;

    private Vector3 initialScale;
    private Vector3 targetScale;
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    public void PlayDisappearAnimationart()
    {
        initialScale = transform.localScale;
        targetScale = new Vector3(transform.localScale.x, 0.0f, transform.localScale.z);
        initialPosition = transform.localPosition;
        targetPosition = new Vector3(transform.localPosition.x, -1.0f, transform.localPosition.z);

        StartCoroutine(AnimateDisappear());
    }

    private IEnumerator AnimateDisappear()
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / duration);

            float newYScale = Mathf.Lerp(initialScale.y, targetScale.y, elapsedTime / duration);
            transform.localScale = new Vector3(initialScale.x, newYScale, initialScale.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
        transform.localScale = targetScale;
        gameObject.SetActive(false);
    }
}
