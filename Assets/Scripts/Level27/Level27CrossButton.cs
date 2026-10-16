using UnityEngine;

// TYLKO LEVEL 27. Przycisk X: działa tylko, gdy blok stoi pionowo dokładnie na płycie - wtedy chowa
// przypięte mosty (oba). Działa jeden raz - ponowne wejście nic nie zmienia.
public class Level27CrossButton : MonoBehaviour
{
    [Tooltip("Mosty chowane przez ten przycisk.")]
    [SerializeField] private Level27Bridge[] bridges;
    [Tooltip("Płyta przycisku (jej środek wyznacza pole). Puste = ten obiekt.")]
    [SerializeField] private Transform plate;

    private bool used;
    private Player[] players;

    private void Start()
    {
        // Gracz i obie kostki z trybu dwóch kostek - wszystkie z tej sceny.
        players = System.Array.FindAll(FindObjectsByType<Player>(FindObjectsInactive.Include, FindObjectsSortMode.None),
            player => player.gameObject.scene == gameObject.scene);
    }

    private void Update()
    {
        if (used || players == null) return;

        Vector3 cell = (plate != null ? plate : transform).position;
        foreach (Player player in players)
        {
            if (player == null || !player.IsStandingUprightAt(cell)) continue;

            used = true;
            foreach (Level27Bridge bridge in bridges)
            {
                if (bridge != null) bridge.Hide();
            }
            return;
        }
    }
}
