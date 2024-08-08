using UnityEngine;

public class LightOrbit : MonoBehaviour
{
    public Transform target;  // Obiekt, wokó³ którego œwiat³o ma siê obracaæ
    public float distance = 10.0f;  // Odleg³oœæ œwiat³a od obiektu
    public float orbitSpeed = 10.0f;  // Prêdkoœæ obrotu œwiat³a wokó³ obiektu
    public Vector3 orbitAxis = Vector3.up;  // Oœ obrotu œwiat³a (domyœlnie wokó³ osi Y)
    public float initialAngle = 0.0f;  // Pocz¹tkowy k¹t w stopniach

    void Start()
    {
        if (target != null)
        {
            // Oblicz pozycjê œwiat³a na orbicie na podstawie pocz¹tkowego k¹ta
            Vector3 offset = Quaternion.Euler(0, initialAngle, 0) * new Vector3(0, 0, distance);
            transform.position = target.position + offset;
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Obracaj œwiat³o wokó³ obiektu w czasie rzeczywistym
            transform.RotateAround(target.position, orbitAxis, orbitSpeed * Time.deltaTime);

            // Upewnij siê, ¿e œwiat³o jest skierowane na obiekt
            transform.LookAt(target);
        }
    }
}
