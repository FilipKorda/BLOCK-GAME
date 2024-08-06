using UnityEngine;

public class UnlockLevelCodesSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] elementsToUnlock;
  
    public void UpdateUnlockedElements()
    {
        for (int i = 0; i < elementsToUnlock.Length; i++)
        {
            elementsToUnlock[i].SetActive(i <= GameManager.Instance.currentUnlockedAvailableCodesIndex);
        }
    }
}
