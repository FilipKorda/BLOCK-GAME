using UnityEngine;

public class PlayerColorChanger : MonoBehaviour
{
    [SerializeField] private GameObject playersView;
    [SerializeField] private Material defaultRedMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material yellowMaterial;

    public void ActivePlayersView()
    {
        playersView.SetActive(true);
    }

    public void DeactivePlayersView()
    {
        playersView.SetActive(true);
    }

    public void DefaultRed()
    {
        PlayerColorManager.Instance.SelectedMaterial = defaultRedMaterial;
        Debug.Log("Changed color to <color=red>Red</color>");
    }

    public void SelectBlue()
    {
        PlayerColorManager.Instance.SelectedMaterial = blueMaterial;
        Debug.Log("Changed color to <color=blue>Blue</color>");
    }

    public void SelectGreen()
    {
        PlayerColorManager.Instance.SelectedMaterial = greenMaterial;
        Debug.Log("Changed color to <color=green>Green</color>");
    }

    public void SelectYellow()
    {
        PlayerColorManager.Instance.SelectedMaterial = yellowMaterial;
        Debug.Log("Changed color to <color=yellow>Yellow</color>");
    }
}
