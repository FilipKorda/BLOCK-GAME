using UnityEngine;

public class ButtonUnlocker : MonoBehaviour
{
    [SerializeField] private GameObject chooseLevelButton;
    [SerializeField] private GameObject skindButton;

    public void UnlockChooseLevelButton()
    {
        if (GameManager.Instance.currentUnlockedAvailableCodesIndex >= 0)
        {
            chooseLevelButton.SetActive(true);
        }
    }

    public void UnlockSkins()
    {
        if (GameManager.Instance.currentUnlockedAvailableCodesIndex >= 9)
        {
            skindButton.SetActive(true);
        }
    }

}

