using System.Collections.Generic;
using UnityEngine;

public class FallingDownManager : MonoBehaviour
{
    [SerializeField] private GameObject ground;
    public GameObject[] groundObjects;

    [ContextMenu("Add Objects To Diactivate")]
    private void PopulateObjectsToMove()
    {
        if (ground == null)
        {
            Debug.LogError("Ground object is not assigned!");
            return;
        }

        int childCount = ground.transform.childCount;
        List<GameObject> objectsToMoveList = new List<GameObject>();

        for (int i = 0; i < childCount; i++)
        {
            GameObject child = ground.transform.GetChild(i).gameObject;
            if (child.GetComponent<StarRotation>() == null)
            {
                objectsToMoveList.Add(child);
            }
        }

        groundObjects = objectsToMoveList.ToArray();

        Debug.Log("Objects to move populated with children of ground, excluding those with StarRotation.");
    }
}
