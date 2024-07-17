using UnityEngine;

public class Star : MonoBehaviour
{
    [SerializeField] private StarTracker starTracker;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Collider cube1Collider;
    [SerializeField] private Collider cube2Collider;

    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private bool leftRotate;

    void Update()
    {
        if (leftRotate)
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        else
            transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (cube1Collider != null && cube2Collider != null)
        {
            if (other == playerCollider || other == cube1Collider || other == cube2Collider)
            {
                Collect();
            }

        }
    }

    public void Collect()
    {
        starTracker.AddStar();
        gameObject.SetActive(false);
    }
}
