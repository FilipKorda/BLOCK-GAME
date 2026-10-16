using UnityEngine;

public class PlayerColorView : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    private Renderer playerRenderer;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        BlockSkin.Apply(playerRenderer, PlayerColorManager.Instance.SelectedMaterial);
    }

    public void ChangeColor()
    {
        BlockSkin.Apply(playerRenderer, PlayerColorManager.Instance.SelectedMaterial);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
