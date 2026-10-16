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

    // Wywoływane przez widok MAIN MENU zaraz po tym, jak schowa przycisk Skins.
    // Zmiana koloru gracza jest dostępna od początku gry, bez ukończonych poziomów.
    public void UnlockSkins()
    {
        skindButton.SetActive(true);
    }

}

