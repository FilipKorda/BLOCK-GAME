using UnityEngine;

public class PlayerColorView : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    private Renderer playerRenderer;

    void Start()
    {
        playerRenderer = GetComponent<Renderer>();
        playerRenderer.material = PlayerColorManager.Instance.SelectedMaterial;
    }

    public void ChangeColor()
    {
        playerRenderer.material = PlayerColorManager.Instance.SelectedMaterial;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
