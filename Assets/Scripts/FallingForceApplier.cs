using UnityEngine;

public class FallingForceApplier : MonoBehaviour
{
    [SerializeField] private GameObject[] players;

    // Spadanie z krawędzi wykrywa teraz sam Player po każdym ruchu (brak płyty pod blokiem).
    // Ten trigger zostaje tylko jako zabezpieczenie, gdy podłoże zniknie pod stojącym graczem
    // (np. most schowany przyciskiem). W trakcie toczenia jest ignorowany, żeby nie przerywać
    // płynnego przechylenia przez krawędź. Reaguje tylko na gracza z własnej sceny - przy przejściu
    // między poziomami oba są chwilowo załadowane w tych samych współrzędnych.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.scene == gameObject.scene
            && other.TryGetComponent<Player>(out var playerScript) && playerScript.enabled
            && !playerScript.isRotating && !playerScript.IsFalling)
        {
            playerScript.PreparPlayerToFallDown();
        }
    }
}
