using System.Collections;
using UnityEngine;

public class TwoCubeController : MonoBehaviour
{
    [SerializeField] private GameObject twoCubeController;
    [SerializeField] private Player player;
    [SerializeField] private TwoCubesButton twoCubesButton;
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
    private Vector3 initialPositionCube1;
    private Vector3 initialPositionCube2;

    private void Awake()
    {
        mainCamera = Camera.main;
        initialPositionCube1 = cube1.transform.position;
        initialPositionCube2 = cube2.transform.position;
        ActiveCube = cube1;
    }

    void Start()
    {            
        StartCoroutine(ActiveSelectorOnCube1(0.5f));     
    }

    private void OnEnable()
    {
        StartCoroutine(ActiveSelectorOnCube1(0.5f));
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


            //=============PO£¥CZENIE KOSTEK W JEDNEGO GRACZA=============//
            if (Mathf.Abs(dx - 1) <= positionOffset)
            {
                // Poziome po³¹czenie (na osi X)
                player.transform.rotation = Quaternion.Euler(0, 0, 90);
                player.scale = new Vector3(1f, 0.5f, 0.5f);
                ResetToInitialCubes();
            }
            else if (Mathf.Abs(dz - 1) <= positionOffset)
            {
                // Pionowe po³¹czenie (na osi Z)
                player.transform.rotation = Quaternion.Euler(0, 90, 90);
                player.scale = new Vector3(0.5f, 0.5f, 1f);
                ResetToInitialCubes();
            }

            player.gameObject.SetActive(true);
            ResetTwoCubesButtonCollider();
            player.pivot = new Vector3(0, 0, 0);
            player.axis = new Vector3(0, 0, 0);

            //=============PO£¥CZENIE KOSTEK W JEDNEGO GRACZA=============//
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

    private void ResetToInitialCubes()
    {
        cube1.transform.position = initialPositionCube1;
        cube2.transform.position = initialPositionCube2;

        cubeMovement1.enabled = false;
        cubeMovement1.canMove = true;

        cubeMovement2.enabled = false;
        cubeMovement2.canMove = true;

        ActiveCube = cube1;
        isCube1Active = true;
    }

    private void ResetTwoCubesButtonCollider()
    {
        twoCubesButton.twoCubeButtonCollider.isTrigger = true;
        twoCubesButton.twoCubeButtonCollider.enabled = false;
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