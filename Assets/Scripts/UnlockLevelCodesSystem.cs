using UnityEngine;

public class UnlockLevelCodesSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] elementsToUnlock;
  
    public void UpdateUnlockedElements()
    {
        for (int i = 0; i < elementsToUnlock.Length; i++)
        {
            if (GameManager.Instance.completedLevels.Contains(i))
            {
                elementsToUnlock[i].SetActive(true);
            }
            else
            {
                elementsToUnlock[i].SetActive(false);
            }
        }
    }
}
