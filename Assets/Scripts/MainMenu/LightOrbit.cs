using UnityEngine;

public class LightOrbit : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 10.0f;
    [SerializeField] private float orbitSpeed = 10.0f;
    [SerializeField] private Vector3 orbitAxis = Vector3.up;
    [SerializeField] private float initialAngle = 0.0f;  

    void Start()
    {
        if (target != null)
        {
            Vector3 offset = Quaternion.Euler(0, initialAngle, 0) * new Vector3(0, 0, distance);
            transform.position = target.position + offset;
        }
    }

    void Update()
    {
        if (target != null)
        {
            transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);
            transform.LookAt(target);
        }
    }
}
