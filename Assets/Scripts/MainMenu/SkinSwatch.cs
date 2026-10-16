using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Jedna próbka koloru w palecie Skins. Zapisana w scenie - kolor, nazwę i wymaganie odblokowania
// można zmieniać w inspektorze. Gdy Material jest pusty, materiał powstaje w trakcie gry z szablonu
// PlayerColorChanger. Najechanie lub zaznaczenie (klawiatura/pad) zablokowanej próbki pokazuje podpowiedź.
[RequireComponent(typeof(Button))]
public class SkinSwatch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public string colorName;
    public Color color = Color.white;
    [Tooltip("Opcjonalny gotowy materiał (np. PlayerBlue). Pusty = materiał tworzony z koloru.")]
    public Material material;
    [Tooltip("Podgląd koloru - w trakcie gry dostaje wzór skina.")]
    public RawImage preview;
    [Tooltip("Wzór (tekstura w odcieniach szarości) tego skina. Puste = Resources/SkinPatterns/<Color Name>.")]
    public Texture2D pattern;

    [Header("Unlock")]
    [Tooltip("Ile ukończonych poziomów potrzeba do odblokowania. -1 = automatycznie według kolejności w palecie " +
             "(pierwsze 4 kolory od razu, każdy następny po kolejnym ukończonym poziomie).")]
    public int requiredLevels = -1;
    [Tooltip("Nakładka z kłódką widoczna, gdy kolor jest zablokowany.")]
    public GameObject lockOverlay;

    private PlayerColorChanger owner;

    private PlayerColorChanger Owner => owner != null ? owner : owner = GetComponentInParent<PlayerColorChanger>(true);

    public void OnPointerEnter(PointerEventData eventData) => Owner?.OnSwatchHover(this, true);
    public void OnPointerExit(PointerEventData eventData) => Owner?.OnSwatchHover(this, false);
    public void OnSelect(BaseEventData eventData) => Owner?.OnSwatchHover(this, true);
    public void OnDeselect(BaseEventData eventData) => Owner?.OnSwatchHover(this, false);

    private void OnValidate()
    {
        if (preview != null) preview.color = material != null ? material.color : color;
    }
}
