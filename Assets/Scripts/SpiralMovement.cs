using System.Collections;
using UnityEngine;

public class SpiralMovement : MonoBehaviour
{
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject[] objectsToMove;
    [SerializeField] private float initialSpiralSpeed = 0.05f;
    [SerializeField] private float initialAngleSpeed = 4f;
    [SerializeField] private float initialUpwardSpeed = 1f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private Vector3 centerPoint = Vector3.zero;
    [SerializeField] private float delayBeetweenPlates = 0.025f;
    [SerializeField] private float startDelay = 0.5f;
    private bool isWin = false;
    private float[] startAngles;
    private float[] startDistances;
    private float[] startHeights;
    private float[] startTimes;

    public void Play()
    {
        StartCoroutine(ActiveSpiralMovement());
    }

    private IEnumerator ActiveSpiralMovement()
    {
        yield return new WaitForSeconds(startDelay);
        isWin = true;
        startAngles = new float[objectsToMove.Length];
        startDistances = new float[objectsToMove.Length];
        startHeights = new float[objectsToMove.Length];
        startTimes = new float[objectsToMove.Length];
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            Vector3 direction = objectsToMove[i].transform.position - centerPoint;
            startAngles[i] = Mathf.Atan2(direction.z, direction.x);
            startDistances[i] = direction.magnitude;
            startHeights[i] = objectsToMove[i].transform.position.y;
            startTimes[i] = Time.time + i * delayBeetweenPlates;
        }

    }

    void Update()
    {
        if (isWin)
        {
            for (int i = 0; i < objectsToMove.Length; i++)
            {
                if (Time.time >= startTimes[i])
                {
                    float elapsedTime = Time.time - startTimes[i];

                    float spiralSpeed = initialSpiralSpeed + elapsedTime * acceleration;
                    float angleSpeed = initialAngleSpeed + elapsedTime * acceleration;
                    float upwardSpeed = initialUpwardSpeed + elapsedTime * acceleration;

                    float newAngle = startAngles[i] + elapsedTime * angleSpeed;
                    float newDistance = startDistances[i] + elapsedTime * spiralSpeed;
                    float newHeight = startHeights[i] + elapsedTime * upwardSpeed;

                    float x = centerPoint.x + newDistance * Mathf.Cos(newAngle);
                    float z = centerPoint.z + newDistance * Mathf.Sin(newAngle);
                    Vector3 newPosition = new(x, newHeight, z);

                    objectsToMove[i].transform.position = newPosition;
                }
            }
        }

    }

    [ContextMenu("Add Objects To Move")]
    private void PopulateObjectsToMove()
    {
        if (ground == null)
        {
            Debug.LogError("Ground object is not assigned!");
            return;
        }

        int childCount = ground.transform.childCount;
        objectsToMove = new GameObject[childCount];
        for (int i = 0; i < childCount; i++)
        {
            objectsToMove[i] = ground.transform.GetChild(i).gameObject;
        }

        Debug.Log("Objects to move populated with children of ground.");
    }
}
