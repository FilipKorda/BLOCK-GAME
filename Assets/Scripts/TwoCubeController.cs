using UnityEngine;
using UnityEngine.UIElements;

public class TwoCubeController : MonoBehaviour
{
    [SerializeField] private GameObject twoCubeController;
    [SerializeField] private Player player;
    [SerializeField] private GameObject cube1;
    [SerializeField] private CubeMovement cubeMovement1;
    [SerializeField] private GameObject cube2;
    [SerializeField] private CubeMovement cubeMovement2;

    private GameObject ActiveCube;
    private bool isCube1Active = true;
    private readonly float positionOffset = 0.001f;

    void Start()
    {
        ActiveCube = cube1;
        cubeMovement2.enabled = false;
        Debug.Log("Kontrola nad: Cube1");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchCube();
        }

        CheckProximity();
    }

    void SwitchCube()
    {
        isCube1Active = !isCube1Active;
        ActiveCube = isCube1Active ? cube1 : cube2;

        cubeMovement1.enabled = isCube1Active;
        cubeMovement2.enabled = !isCube1Active;

        string cubeName = isCube1Active ? "Cube1" : "Cube2";
        Debug.Log("Kontrola nad: " + cubeName);
    }
    void CheckProximity()
    {
        Vector3 positionCube1 = cube1.transform.position;
        Vector3 positionCube2 = cube2.transform.position;

        float dx = Mathf.Abs(positionCube1.x - positionCube2.x);
        float dy = Mathf.Abs(positionCube1.y - positionCube2.y);
        float dz = Mathf.Abs(positionCube1.z - positionCube2.z);

        if ((Mathf.Abs(dx - 1) <= positionOffset && dy <= positionOffset && dz <= positionOffset) ||
            (dx <= positionOffset && Mathf.Abs(dy - 1) <= positionOffset && dz <= positionOffset) ||
            (dx <= positionOffset && dy <= positionOffset && Mathf.Abs(dz - 1) <= positionOffset))
        {
            twoCubeController.SetActive(false);

            Vector3 connectionPosition = CalculateConnectionPosition(positionCube1, positionCube2, dx, dy, dz);
            Debug.Log("Cube1 i Cube2 s¹ obok siebie! Pozycja po³¹czenia: " + connectionPosition);

            player.transform.position = connectionPosition;

            //rotacja czerwonego gracza po z³¹czeniu siê
            if (Mathf.Abs(dx - 1) <= positionOffset)
            {
                // Poziome po³¹czenie (na osi X)
                player.transform.rotation = Quaternion.Euler(0, 0, 90);
                player.scale = new Vector3(1f, 0.5f, 0.5f);
            }        
            else if (Mathf.Abs(dz - 1) <= positionOffset)
            {
                // Poziome po³¹czenie (na osi Z)
                player.transform.rotation = Quaternion.Euler(0, 90, 90);
                player.scale = new Vector3(0.5f, 0.5f, 1f);
            }

            player.gameObject.SetActive(true);
            player.pivot = new Vector3(0, 0, 0);
            player.axis = new Vector3(0, 0, 0);

        }
    }
    Vector3 CalculateConnectionPosition(Vector3 pos1, Vector3 pos2, float dx, float dy, float dz)
    {
        if (Mathf.Abs(dx - 1) <= positionOffset)
        {
            return new Vector3((pos1.x + pos2.x) / 2, pos1.y, pos1.z);
        }
        else if (Mathf.Abs(dy - 1) <= positionOffset)
        {
            return new Vector3(pos1.x, (pos1.y + pos2.y) / 2, pos1.z);
        }
        else if (Mathf.Abs(dz - 1) <= positionOffset)
        {
            return new Vector3(pos1.x, pos1.y, (pos1.z + pos2.z) / 2);
        }
        return Vector3.zero;
    }
}
