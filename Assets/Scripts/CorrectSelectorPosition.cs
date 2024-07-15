using UnityEngine;

public class CorrectSelectorPosition : MonoBehaviour
{
    [SerializeField] private Transform selector;

    private void Update()
    {
        selector.localPosition = new Vector3(0, 0.25f, 0);
    }
}
