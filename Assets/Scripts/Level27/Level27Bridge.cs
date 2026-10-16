using System.Collections;
using UnityEngine;

// TYLKO LEVEL 27. Most, który można schować jeden raz (przyciskiem kółka albo X) i nie da się go przywrócić.
// Chowanie: most zjeżdża o Hide Offset i wyłącza się. Jeśli gracz na nim stał - spada.
public class Level27Bridge : MonoBehaviour
{
    [Tooltip("Obiekt mostu. Puste = ten obiekt.")]
    [SerializeField] private Transform bridge;
    [Tooltip("Przesunięcie (lokalne) przy chowaniu mostu.")]
    [SerializeField] private Vector3 hideOffset = new(0f, -2f, 0f);
    [SerializeField] private float hideDuration = 0.35f;
    [Tooltip("Wyłącz most po schowaniu (bez tego zostaje w miejscu docelowym).")]
    [SerializeField] private bool deactivateWhenHidden = true;

    public bool IsHidden { get; private set; }

    private Transform Target => bridge != null ? bridge : transform;

    public void Hide()
    {
        if (IsHidden) return;
        IsHidden = true;

        StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        Transform target = Target;
        Vector3 start = target.localPosition;
        Vector3 end = start + hideOffset;

        for (float elapsed = 0f; elapsed < hideDuration; elapsed += Time.deltaTime)
        {
            float t = elapsed / hideDuration;
            target.localPosition = Vector3.Lerp(start, end, t * t);
            yield return null;
        }
        target.localPosition = end;

        // Wyłączenie i sprawdzenie gracza w tym samym kroku (bez yield) - działa też wtedy,
        // gdy ten komponent leży na samym moście.
        if (deactivateWhenHidden) target.gameObject.SetActive(false);

        // Most zniknął - gracz stojący na nim (choćby jedną połową) musi spaść.
        foreach (Player player in FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
            if (player.gameObject.scene == gameObject.scene) player.CheckSupportNow();
        }
    }
}
