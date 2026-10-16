using UnityEngine;

// TYLKO LEVEL 27. Przycisk kółka: wejście bloku na płytę (w dowolnej pozycji) chowa przypięte mosty.
// Działa jeden raz - ponowne wejście nic nie zmienia. Komponent musi leżeć na obiekcie
// z colliderem typu trigger (np. CrossPlateA).
public class Level27CircleButton : MonoBehaviour
{
    [Tooltip("Mosty chowane przez ten przycisk.")]
    [SerializeField] private Level27Bridge[] bridges;

    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used || !enabled) return;
        if (!other.CompareTag("Player") || other.gameObject.scene != gameObject.scene) return;

        used = true;
        foreach (Level27Bridge bridge in bridges)
        {
            if (bridge != null) bridge.Hide();
        }
    }
}
