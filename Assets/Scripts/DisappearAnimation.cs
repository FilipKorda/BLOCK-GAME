using UnityEngine;
using System.Collections;

public class DisappearAnimation : MonoBehaviour
{
    public float speed = 1.0f; // Now using speed instead of duration
    private Vector3 initialScale;
    private Vector3 targetScale;
    private Vector3 initialPosition;
    private Vector3 targetAnimPosition;
    [SerializeField] private Transform endPosition;

    private bool isAnimating = false; // Flag to control the animation

    public void PlayDisappearAnimation()
    {
        isAnimating = true;
        StartCoroutine(AnimateDisappear());

        if (isAnimating)
        {
            initialScale = transform.localScale;
            targetScale = new Vector3(transform.localScale.x, 0.0f, transform.localScale.z);
            initialPosition = transform.localPosition;
            targetAnimPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 1.0f, transform.localPosition.z);
        }        
    }

    private IEnumerator AnimateDisappear()
    {
        while (isAnimating)
        {
            float step = speed * Time.deltaTime;

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetAnimPosition, step);
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, step);

            if (Vector3.Distance(transform.localPosition, targetAnimPosition) < 0.001f || transform.position == endPosition.position)
            {
                isAnimating = false;
                Debug.Log("!!!!!!!!");
            }

            yield return null;
        }

        transform.localPosition = targetAnimPosition;
        transform.localScale = targetScale;
    }
}
