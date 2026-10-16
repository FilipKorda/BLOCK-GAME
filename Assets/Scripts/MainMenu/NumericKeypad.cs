using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Klawiatura numeryczna (0-9, kasowanie, OK) do wpisywania kodu poziomu padem.
// Zapisana w scenie (ChooseLevel -> PPM -> Build Numeric Keypad), więc można ją dowolnie edytować.
// Widoczna tylko, gdy gracz ostatnio używał pada - klawiatura lub mysz ją chowają.
public class NumericKeypad : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private ChooseLevel chooseLevel;
    [Tooltip("Obiekt z klawiszami - włączany i wyłączany zależnie od używanego urządzenia.")]
    [SerializeField] private GameObject keys;
    [Tooltip("Klawisz zaznaczany po pojawieniu się klawiatury.")]
    [SerializeField] private Selectable firstSelected;
    [SerializeField] private int maxLength = 6;

    private bool selectOnShow;

    private void Awake()
    {
        if (keys == null) return;

        foreach (NumericKeypadKey key in keys.GetComponentsInChildren<NumericKeypadKey>(true))
        {
            NumericKeypadKey captured = key;
            key.GetComponent<Button>().onClick.AddListener(() => Press(captured));
        }
    }

    private void OnEnable()
    {
        selectOnShow = true;
        Refresh();
    }

    // LateUpdate - po automatycznym zaznaczaniu w GamepadMenuSelection, żeby to klawiatura miała pierwszeństwo.
    private void LateUpdate()
    {
        Refresh();
    }

    private void Refresh()
    {
        if (keys == null) return;

        bool visible = GamepadMenuSelection.UsingGamepad;
        if (keys.activeSelf != visible)
        {
            keys.SetActive(visible);
            selectOnShow = visible;
        }

        if (!visible || !selectOnShow || EventSystem.current == null) return;

        Selectable target = firstSelected != null ? firstSelected : keys.GetComponentInChildren<Selectable>();
        if (target != null && target.gameObject.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(target.gameObject);
            selectOnShow = false;
        }
    }

    private void Press(NumericKeypadKey key)
    {
        if (inputField == null) return;

        switch (key.action)
        {
            case NumericKeypadKey.KeyAction.Digit:
                if (inputField.text.Length < maxLength) inputField.text += key.digit;
                break;
            case NumericKeypadKey.KeyAction.Backspace:
                if (inputField.text.Length > 0) inputField.text = inputField.text[..^1];
                break;
            case NumericKeypadKey.KeyAction.Enter:
                if (chooseLevel != null) chooseLevel.ClickEnter();
                break;
        }
    }

#if UNITY_EDITOR
    private static readonly (string label, NumericKeypadKey.KeyAction action)[] Layout =
    {
        ("1", NumericKeypadKey.KeyAction.Digit), ("2", NumericKeypadKey.KeyAction.Digit), ("3", NumericKeypadKey.KeyAction.Digit),
        ("4", NumericKeypadKey.KeyAction.Digit), ("5", NumericKeypadKey.KeyAction.Digit), ("6", NumericKeypadKey.KeyAction.Digit),
        ("7", NumericKeypadKey.KeyAction.Digit), ("8", NumericKeypadKey.KeyAction.Digit), ("9", NumericKeypadKey.KeyAction.Digit),
        ("<", NumericKeypadKey.KeyAction.Backspace), ("0", NumericKeypadKey.KeyAction.Digit), ("OK", NumericKeypadKey.KeyAction.Enter),
    };

    // Buduje klawiaturę jako zwykłe obiekty sceny (z Undo) pod polem kodu. Wywoływane z ChooseLevel.
    public static NumericKeypad Build(ChooseLevel chooseLevel, TMP_InputField field)
    {
        const float cell = 80f, spacing = 10f;
        Sprite rounded = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Generated UI/HUD Rounded Panel.png");
        RectTransform fieldRect = (RectTransform)field.transform;

        GameObject rootObject = new("Numeric Keypad", typeof(RectTransform));
        UnityEditor.Undo.RegisterCreatedObjectUndo(rootObject, "Build Numeric Keypad");
        rootObject.layer = field.gameObject.layer;
        RectTransform root = (RectTransform)rootObject.transform;
        root.SetParent(fieldRect.parent, false);
        root.anchorMin = root.anchorMax = fieldRect.anchorMin;
        root.pivot = new Vector2(0.5f, 1f);
        root.sizeDelta = new Vector2(cell * 3f + spacing * 2f, cell * 4f + spacing * 3f);
        // Pod polem kodu, wyśrodkowana względem niego.
        root.anchoredPosition = fieldRect.anchoredPosition
                                + new Vector2((0.5f - fieldRect.pivot.x) * fieldRect.rect.width, -fieldRect.pivot.y * fieldRect.rect.height - 20f);

        GameObject keysObject = new("Keys", typeof(RectTransform), typeof(GridLayoutGroup));
        keysObject.layer = rootObject.layer;
        RectTransform keysRect = (RectTransform)keysObject.transform;
        keysRect.SetParent(root, false);
        keysRect.anchorMin = Vector2.zero;
        keysRect.anchorMax = Vector2.one;
        keysRect.offsetMin = keysRect.offsetMax = Vector2.zero;

        GridLayoutGroup grid = keysObject.GetComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(cell, cell);
        grid.spacing = new Vector2(spacing, spacing);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.childAlignment = TextAnchor.UpperCenter;

        TMP_Text style = field.textComponent;
        Selectable first = null;
        foreach (var (label, action) in Layout)
        {
            GameObject keyObject = new("Key " + label, typeof(RectTransform), typeof(Image), typeof(Button), typeof(NumericKeypadKey));
            keyObject.layer = rootObject.layer;
            keyObject.transform.SetParent(keysRect, false);

            Image background = keyObject.GetComponent<Image>();
            background.sprite = rounded;
            background.type = rounded != null ? Image.Type.Sliced : Image.Type.Simple;

            Button button = keyObject.GetComponent<Button>();
            button.targetGraphic = background;
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.08f, 0.05f, 0.03f, 0.85f);
            colors.highlightedColor = new Color(1f, 0.58f, 0.12f, 1f);
            colors.selectedColor = new Color(1f, 0.58f, 0.12f, 1f);
            colors.pressedColor = new Color(0.8f, 0.4f, 0.05f, 1f);
            button.colors = colors;

            NumericKeypadKey key = keyObject.GetComponent<NumericKeypadKey>();
            key.action = action;
            key.digit = action == NumericKeypadKey.KeyAction.Digit ? label : "";

            GameObject labelObject = new("Label", typeof(RectTransform));
            labelObject.layer = rootObject.layer;
            RectTransform labelRect = (RectTransform)labelObject.transform;
            labelRect.SetParent(keyObject.transform, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = labelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI text = labelObject.AddComponent<TextMeshProUGUI>();
            if (style != null)
            {
                text.font = style.font;
                text.fontSharedMaterial = style.fontSharedMaterial;
            }
            text.text = label;
            text.fontSize = 40f;
            text.color = new Color(1f, 0.95f, 0.86f);
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;

            if (first == null) first = button;
        }

        NumericKeypad keypad = rootObject.AddComponent<NumericKeypad>();
        keypad.inputField = field;
        keypad.chooseLevel = chooseLevel;
        keypad.keys = keysObject;
        keypad.firstSelected = first;
        return keypad;
    }
#endif
}
