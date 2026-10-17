using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Czytelność tekstów HUD (ruchy, gwiazdki, czas) na pomarańczowo-czarnym tle.
// Panel i cień to zwykłe obiekty zapisane w prefabie / scenie (tworzy je
// Tools > BLOCK GAME > Build HUD Backdrops) - można je dowolnie edytować w inspektorze.
// Ten komponent tylko dopasowuje panel do długości tekstu, przepisuje tekst do cienia
// i chowa oba razem z tekstem. Działa też w edytorze.
// Czcionka Gugi ma atlas bitmapowy, więc obrys z materiału TMP nie działa - stąd cień jako kopia tekstu.
[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class HudTextBackdrop : MonoBehaviour
{
    [Tooltip("Panel pod tekstem (rodzeństwo tekstu, wyżej w hierarchii).")]
    public RectTransform panel;
    [Tooltip("Cień - kopia tekstu przesunięta w dół (rodzeństwo tekstu, między panelem a tekstem).")]
    public TMP_Text shadow;
    [Tooltip("Margines panelu wokół tekstu.")]
    public Vector2 padding = new(22f, 8f);
    [Tooltip("Przesunięcie cienia względem tekstu.")]
    public Vector2 shadowOffset = new(2.5f, -2.5f);

    private TMP_Text text;

    private TMP_Text Text => text != null ? text : text = GetComponent<TMP_Text>();

    private void OnEnable() => SetDecorationsVisible(true);
    private void OnDisable() => SetDecorationsVisible(false);

    // Przełączamy tylko komponenty graficzne (nie SetActive), bo OnDisable przychodzi też
    // w trakcie wyłączania rodzica, a wtedy nie wolno zmieniać aktywności innych obiektów.
    private void SetDecorationsVisible(bool visible)
    {
        if (panel != null)
        {
            foreach (Graphic graphic in panel.GetComponentsInChildren<Graphic>(true)) graphic.enabled = visible;
        }
        if (shadow != null) shadow.enabled = visible;
    }

    private void LateUpdate()
    {
#if UNITY_EDITOR
        // W edytorze nie ruszamy instancji prefabu w scenach poziomów - inaczej każde otwarcie sceny
        // tworzyłoby nadpisania. Układ aktualizuje się w samym prefabie, w scenie Root i w trakcie gry.
        if (!Application.isPlaying && UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this)) return;
#endif
        UpdateLayout();
    }

    public void UpdateLayout()
    {
        TMP_Text source = Text;
        RectTransform textRect = (RectTransform)transform;

        if (shadow != null)
        {
            if (shadow.text != source.text) shadow.text = source.text;
            if (!Mathf.Approximately(shadow.fontSize, source.fontSize)) shadow.fontSize = source.fontSize;

            RectTransform shadowRect = shadow.rectTransform;
            Vector2 shadowPosition = textRect.anchoredPosition + shadowOffset;
            if (shadowRect.anchoredPosition != shadowPosition) shadowRect.anchoredPosition = shadowPosition;
        }

        if (panel == null) return;

        Bounds bounds = source.textBounds;
        bool hasText = !string.IsNullOrEmpty(source.text) && bounds.size.x > 0f;
        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null && panelImage.enabled != hasText) SetDecorationsVisible(hasText);
        if (!hasText) return;

        // Panel obejmuje faktyczny tekst (wyrównanie do prawej/środka) z marginesem.
        Vector3 center = textRect.TransformPoint(bounds.center);
        Vector2 size = new(bounds.size.x + padding.x * 2f, Mathf.Max(bounds.size.y, source.fontSize * 0.8f) + padding.y * 2f);
        if ((panel.position - center).sqrMagnitude > 0.0001f) panel.position = center;
        if ((panel.sizeDelta - size).sqrMagnitude > 0.01f) panel.sizeDelta = size;
    }
}
