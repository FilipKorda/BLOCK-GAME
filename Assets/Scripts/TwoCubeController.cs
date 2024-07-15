using System.Collections;
using UnityEngine;

public class TwoCubeController : MonoBehaviour
{
    [SerializeField] private GameObject twoCubeController;
    [SerializeField] private Player player;
    [Header("Cube 1")]
    [SerializeField] private GameObject cube1;
    [SerializeField] private CubeMovement cubeMovement1;
    [SerializeField] private GameObject selector_Canvas1;
    [Header("Cube 2")]
    [SerializeField] private GameObject cube2;
    [SerializeField] private CubeMovement cubeMovement2;
    [SerializeField] private GameObject selector_Canvas2;

    private Camera mainCamera;
    private GameObject ActiveCube;
    private bool isCube1Active = true;
    private readonly float positionOffset = 0.001f;

    void Start()
    {
        mainCamera = Camera.main;
        ActiveCube = cube1;
        StartCoroutine(ActiveSelectorOnCube1(0.5f));
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

        if (isCube1Active)
        {
            StartCoroutine(ActiveSelectorOnCube1(0.5f));
        }
        else
        {
            StartCoroutine(ActiveSelectorOnCube2(0.5f));
        }

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

    #region LookAtCameraAndShowSelector
    public IEnumerator ActiveSelectorOnCube1(float duration)
    {
        SelectorLookAtCamera1();
        selector_Canvas1.SetActive(true);
        yield return new WaitForSeconds(duration);
        selector_Canvas1.SetActive(false);
    }

    public IEnumerator ActiveSelectorOnCube2(float duration)
    {
        SelectorLookAtCamera2();
        selector_Canvas2.SetActive(true);
        yield return new WaitForSeconds(duration);
        selector_Canvas2.SetActive(false);
    }

    public void SelectorLookAtCamera1()
    {
        Vector3 direction = mainCamera.transform.position - selector_Canvas1.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        selector_Canvas1.transform.rotation = lookRotation;
    }

    public void SelectorLookAtCamera2()
    {
        Vector3 direction = mainCamera.transform.position - selector_Canvas2.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        selector_Canvas2.transform.rotation = lookRotation;
    }

    public void DisableSelector()
    {
        if (isCube1Active)
        {
            selector_Canvas1.SetActive(false);
        }
        else
        {
            selector_Canvas2.SetActive(false);
        }
    }
    #endregion

}