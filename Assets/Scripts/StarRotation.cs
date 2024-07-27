using UnityEngine;

public class StarRotation : MonoBehaviour
{
    private readonly float rotationSpeed = 10f;

    void Update()
    {
        if (transform.gameObject.activeInHierarchy)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
