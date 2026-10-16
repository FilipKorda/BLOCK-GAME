using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerColorChanger : MonoBehaviour
{
    [Serializable]
    public struct SkinColor
    {
        public string name;
        public Color color;

        public SkinColor(string name, Color color)
        {
            this.name = name;
            this.color = color;
        }
    }

    [SerializeField] private GameObject playersView;
    [SerializeField] private Material defaultRedMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material yellowMaterial;

    [Header("Palette")]
    [Tooltip("Paleta zapisana w scenie (PPM na komponencie -> Build Skin Palette). Pusta = paleta tworzona dopiero w trakcie gry.")]
    [SerializeField] private RectTransform paletteRoot;
    [SerializeField] private TMP_Text selectedLabel;
    [Tooltip("Kolory dodawane do palety po czterech podstawowych (Red, Blue, Green, Yellow) przy budowaniu palety.")]
    [SerializeField] private SkinColor[] extraColors =
    {
        new("Classic", new Color(0.78f, 0.36f, 0.12f)),
        new("Orange", new Color(1f, 0.52f, 0.06f)),
        new("Gold", new Color(0.86f, 0.68f, 0.2f)),
        new("Lime", new Color(0.58f, 0.85f, 0.2f)),
        new("Teal", new Color(0.1f, 0.56f, 0.52f)),
        new("Cyan", new Color(0.22f, 0.8f, 0.86f)),
        new("Navy", new Color(0.16f, 0.22f, 0.52f)),
        new("Purple", new Color(0.52f, 0.26f, 0.76f)),
        new("Pink", new Color(0.95f, 0.46f, 0.7f)),
        new("Brown", new Color(0.46f, 0.29f, 0.15f)),
        new("White", new Color(0.92f, 0.92f, 0.92f)),
        new("Graphite", new Color(0.26f, 0.26f, 0.29f)),
    };
    [SerializeField] private int paletteColumns = 4;
    [SerializeField] private float swatchSpacing = 10f;
    [Tooltip("Rozmiar palety, gdy nie da się go wyliczyć ze starych przycisków kolorów.")]
    [SerializeField] private Vector2 defaultPaletteSize = new(400f, 315f);
    [SerializeField] private Color swatchFrameColor = new(0.12f, 0.12f, 0.12f);
    [SerializeField] private Color swatchSelectedFrameColor = new(1f, 0.6f, 0.12f);

    [Header("Unlocking")]
    [Tooltip("Ile pierwszych kolorów palety jest dostępnych od początku. Każdy następny wymaga kolejnego ukończonego poziomu " +
             "(chyba że próbka ma własne Required Levels).")]
    [SerializeField] private int freeColors = 4;
    [Tooltip("Dymek z informacją o zablokowanym kolorze (PPM na komponencie -> Build Lock UI).")]
    [SerializeField] private RectTransform lockedHint;
    [SerializeField] private TMP_Text lockedHintText;
    [Tooltip("{0} = liczba poziomów, {1} = \"level\" albo \"levels\".")]
    [SerializeField] private string lockedHintFormat = "This skin will unlock after you complete {0} {1}.";
    [Tooltip("Ikona kłódki na zablokowanych próbkach. Pusta = kłódka generowana w trakcie gry.")]
    [SerializeField] private Sprite lockIcon;
    [Tooltip("Tło dymka z podpowiedzią (np. HUD Rounded Panel).")]
    [SerializeField] private Sprite hintBackground;

    private const float LabelHeight = 50f;
    private static readonly string[] OldColorMethods = { nameof(DefaultRed), nameof(SelectBlue), nameof(SelectGreen), nameof(SelectYellow) };

    private readonly List<(string name, Material material, Button button, SkinSwatch swatch, int requiredLevels)> palette = new();
    private SkinSwatch hintedSwatch;

    private static int CompletedLevels => GameManager.Instance != null ? GameManager.Instance.finishedLevels.Count : 0;

    private void Start()
    {
        if (paletteRoot == null)
        {
            paletteRoot = CreatePalette();
        }
        HideOldColorButtons();
        EnsureLockUI(false);
        WirePalette();
    }

    // Widok Skins otwiera się wielokrotnie - za każdym razem odświeżamy kłódki według postępu gracza.
    private void OnEnable()
    {
        if (palette.Count > 0) RefreshLocks();
        HideLockedHint();
    }

    public void ActivePlayersView()
    {
        playersView.SetActive(true);
    }

    public void DeactivePlayersView()
    {
        playersView.SetActive(true);
    }

    public void DefaultRed() => SelectMaterial(defaultRedMaterial);
    public void SelectBlue() => SelectMaterial(blueMaterial);
    public void SelectGreen() => SelectMaterial(greenMaterial);
    public void SelectYellow() => SelectMaterial(yellowMaterial);

    private void SelectMaterial(Material material)
    {
        if (material == null) return;

        PlayerColorManager.Instance.SelectedMaterial = material;
        SaveSystem.SaveSkin(material);
        foreach (PlayerColorView view in FindObjectsByType<PlayerColorView>(FindObjectsSortMode.None))
        {
            view.ChangeColor();
        }
        RefreshSelection();
        Debug.Log($"Changed color to {material.name}");
    }

    #region Runtime wiring
    // Podpina próbki z palety (zapisanej w scenie albo utworzonej przed chwilą):
    // tworzy materiały dla kolorów bez gotowego materiału i nakłada teksturę bloku na podglądy.
    private void WirePalette()
    {
        if (paletteRoot == null) return;

        SkinSwatch[] swatches = paletteRoot.GetComponentsInChildren<SkinSwatch>(true);
        for (int i = 0; i < swatches.Length; i++)
        {
            SkinSwatch swatch = swatches[i];
            Material material = CreateSkinMaterial(swatch);
            if (material == null) continue;

            if (swatch.preview != null)
            {
                swatch.preview.texture = material.mainTexture != null ? material.mainTexture : BlockSkin.TileTexture;
                swatch.preview.color = material.color;
            }

            int requiredLevels = RequiredLevelsFor(swatch, i);
            Button button = swatch.GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                if (CompletedLevels >= requiredLevels) SelectMaterial(material);
                else ShowLockedHint(swatch, requiredLevels);
            });
            palette.Add((swatch.colorName, material, button, swatch, requiredLevels));
        }

        RefreshLocks();
        RefreshSelection();
    }

    private int RequiredLevelsFor(SkinSwatch swatch, int paletteIndex)
    {
        return swatch.requiredLevels >= 0 ? swatch.requiredLevels : Mathf.Max(0, paletteIndex - freeColors + 1);
    }

    private void RefreshLocks()
    {
        int completed = CompletedLevels;
        foreach (var (_, _, _, swatch, requiredLevels) in palette)
        {
            if (swatch != null && swatch.lockOverlay != null)
            {
                swatch.lockOverlay.SetActive(completed < requiredLevels);
            }
        }
    }

    // Wywoływane przez SkinSwatch przy najechaniu myszą lub zaznaczeniu klawiaturą/padem.
    public void OnSwatchHover(SkinSwatch swatch, bool entered)
    {
        if (!entered)
        {
            if (hintedSwatch == swatch) HideLockedHint();
            return;
        }

        foreach (var (_, _, _, paletteSwatch, requiredLevels) in palette)
        {
            if (paletteSwatch != swatch) continue;

            if (CompletedLevels < requiredLevels) ShowLockedHint(swatch, requiredLevels);
            else HideLockedHint();
            return;
        }
    }

    private void ShowLockedHint(SkinSwatch swatch, int requiredLevels)
    {
        if (lockedHint == null || lockedHintText == null) return;

        lockedHintText.text = string.Format(lockedHintFormat, requiredLevels, requiredLevels == 1 ? "level" : "levels");

        // Dymek nad próbką (pivot dymka jest na środku dolnej krawędzi).
        RectTransform swatchRect = (RectTransform)swatch.transform;
        lockedHint.position = swatchRect.TransformPoint(new Vector3(swatchRect.rect.center.x, swatchRect.rect.yMax + 8f, 0f));
        lockedHint.SetAsLastSibling();
        lockedHint.gameObject.SetActive(true);
        hintedSwatch = swatch;
    }

    private void HideLockedHint()
    {
        if (lockedHint != null) lockedHint.gameObject.SetActive(false);
        hintedSwatch = null;
    }

    // Materiał skina: kolor próbki (albo gotowy materiał) + własny wzór koloru (pole Pattern albo
    // SkinPatterns po nazwie koloru). Gotowe materiały z projektu nie są modyfikowane - powstaje kopia.
    private Material CreateSkinMaterial(SkinSwatch swatch)
    {
        Material source = swatch.material != null ? swatch.material : defaultRedMaterial;
        if (source == null) return null;

        Texture2D pattern = swatch.pattern != null ? swatch.pattern : SkinPatterns.Get(swatch.colorName);
        if (swatch.material != null && pattern == null) return swatch.material;

        return new Material(source)
        {
            name = "Player" + swatch.colorName,
            color = swatch.material != null ? swatch.material.color : swatch.color,
            mainTexture = pattern,
            hideFlags = HideFlags.DontUnloadUnusedAsset
        };
    }

    private void HideOldColorButtons()
    {
        foreach (RectTransform oldButton in FindOldColorButtons(out _))
        {
            oldButton.gameObject.SetActive(false);
        }
    }

    private void RefreshSelection()
    {
        Material selected = PlayerColorManager.Instance != null ? PlayerColorManager.Instance.SelectedMaterial : null;

        foreach (var (colorName, material, button, _, _) in palette)
        {
            // Po powrocie do menu materiały kolorów dodatkowych powstają od nowa, więc porównujemy też kolor.
            bool isSelected = material == selected || (selected != null && material.color == selected.color);
            if (button != null) SetSwatchFrame(button, isSelected ? swatchSelectedFrameColor : swatchFrameColor);
            if (isSelected && selectedLabel != null) selectedLabel.text = colorName;
        }
    }

    private static void SetSwatchFrame(Button button, Color normal)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = new Color(0.75f, 0.75f, 0.75f);
        colors.selectedColor = new Color(0.75f, 0.75f, 0.75f);
        colors.pressedColor = new Color(0.55f, 0.55f, 0.55f);
        button.colors = colors;
    }
    #endregion

    #region Palette building
#if UNITY_EDITOR
    // Tworzy paletę jako zwykłe obiekty w scenie (z Undo). Potem wystarczy zapisać scenę.
    [ContextMenu("Build Skin Palette")]
    private void BuildPaletteInEditor()
    {
        UnityEditor.Undo.SetCurrentGroupName("Build Skin Palette");
        UnityEditor.Undo.RecordObject(this, "Build Skin Palette");

        if (paletteRoot != null)
        {
            UnityEditor.Undo.DestroyObjectImmediate(paletteRoot.gameObject);
        }

        paletteRoot = CreatePalette();
        if (paletteRoot == null) return;
        UnityEditor.Undo.RegisterCreatedObjectUndo(paletteRoot.gameObject, "Build Skin Palette");

        foreach (RectTransform oldButton in FindOldColorButtons(out _))
        {
            UnityEditor.Undo.RecordObject(oldButton.gameObject, "Build Skin Palette");
            oldButton.gameObject.SetActive(false);
        }

        BuildLockUI();
        UnityEditor.EditorUtility.SetDirty(this);
    }

    // Dodaje do palety zapisanej w scenie kłódki na próbkach i dymek z podpowiedzią - bez przebudowy palety.
    // Ikona kłódki zapisuje się jako sprite w Assets/Generated UI i trafia do pola Lock Icon.
    // Próbki, które mają już kłódkę, są pomijane, więc można to uruchamiać wielokrotnie.
    [ContextMenu("Build Lock UI")]
    private void BuildLockUI()
    {
        if (paletteRoot == null)
        {
            Debug.LogWarning("Najpierw zbuduj paletę (Build Skin Palette) i zapisz scenę.", this);
            return;
        }

        UnityEditor.Undo.RecordObject(this, "Build Lock UI");
        if (lockIcon == null) lockIcon = SavePadlockSpriteAsset();
        if (hintBackground == null) hintBackground = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Generated UI/HUD Rounded Panel.png");

        EnsureLockUI(true);
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private static Sprite SavePadlockSpriteAsset()
    {
        const string path = "Assets/Generated UI/Padlock.png";
        Sprite existing = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null) return existing;

        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Generated UI"))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Generated UI");

        Texture2D texture = GeneratePadlockTexture();
        System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
        DestroyImmediate(texture);

        UnityEditor.AssetDatabase.ImportAsset(path, UnityEditor.ImportAssetOptions.ForceUpdate);
        var importer = (UnityEditor.TextureImporter)UnityEditor.AssetImporter.GetAtPath(path);
        importer.textureType = UnityEditor.TextureImporterType.Sprite;
        importer.spriteImportMode = UnityEditor.SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
        return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
#endif

    #region Lock UI
    // Tworzy brakujące kłódki i dymek. W edytorze (recordUndo) jako obiekty sceny z Undo;
    // w trakcie gry tylko wtedy, gdy paleta w scenie ich nie ma (np. paleta tworzona dopiero w grze).
    private void EnsureLockUI(bool recordUndo)
    {
        if (paletteRoot == null) return;

        Sprite icon = lockIcon != null ? lockIcon : RuntimePadlockSprite;
        int completed = recordUndo ? 0 : CompletedLevels;
        SkinSwatch[] swatches = paletteRoot.GetComponentsInChildren<SkinSwatch>(true);
        for (int i = 0; i < swatches.Length; i++)
        {
            SkinSwatch swatch = swatches[i];
            if (swatch.lockOverlay != null) continue;

#if UNITY_EDITOR
            if (recordUndo) UnityEditor.Undo.RecordObject(swatch, "Build Lock UI");
#endif
            swatch.lockOverlay = CreateLockOverlay(swatch, icon, recordUndo);
            // W edytorze kłódki wyglądają tak, jak zobaczy je nowy gracz (0 ukończonych poziomów).
            swatch.lockOverlay.SetActive(RequiredLevelsFor(swatch, i) > completed);
#if UNITY_EDITOR
            if (recordUndo) UnityEditor.EditorUtility.SetDirty(swatch);
#endif
        }

        if (lockedHint == null) CreateLockedHint(recordUndo);
    }

    private static void RegisterCreated(GameObject created, bool recordUndo)
    {
#if UNITY_EDITOR
        if (recordUndo) UnityEditor.Undo.RegisterCreatedObjectUndo(created, "Build Lock UI");
#endif
    }

    private static GameObject CreateLockOverlay(SkinSwatch swatch, Sprite icon, bool recordUndo)
    {
        RectTransform overlay = CreateRect("Lock", swatch.transform);
        RegisterCreated(overlay.gameObject, recordUndo);
        overlay.anchorMin = Vector2.zero;
        overlay.anchorMax = Vector2.one;
        overlay.offsetMin = overlay.offsetMax = Vector2.zero;
        Image shade = overlay.gameObject.AddComponent<Image>();
        shade.color = new Color(0f, 0f, 0f, 0.62f);
        shade.raycastTarget = false;

        RectTransform iconRect = CreateRect("Padlock", overlay);
        iconRect.anchorMin = new Vector2(0.2f, 0.2f);
        iconRect.anchorMax = new Vector2(0.8f, 0.8f);
        iconRect.offsetMin = iconRect.offsetMax = Vector2.zero;
        Image iconImage = iconRect.gameObject.AddComponent<Image>();
        iconImage.sprite = icon;
        iconImage.preserveAspect = true;
        iconImage.color = new Color(1f, 0.95f, 0.86f);
        iconImage.raycastTarget = false;

        return overlay.gameObject;
    }

    private void CreateLockedHint(bool recordUndo)
    {
        RectTransform hint = CreateRect("Locked Hint", paletteRoot);
        RegisterCreated(hint.gameObject, recordUndo);
        hint.anchorMin = hint.anchorMax = new Vector2(0.5f, 1f);
        hint.pivot = new Vector2(0.5f, 0f);
        hint.sizeDelta = new Vector2(Mathf.Max(360f, paletteRoot.rect.width), 70f);
        hint.anchoredPosition = new Vector2(0f, 8f);

        Image background = hint.gameObject.AddComponent<Image>();
        background.sprite = hintBackground;
        background.type = hintBackground != null ? Image.Type.Sliced : Image.Type.Simple;
        background.color = new Color(0.04f, 0.02f, 0.01f, 0.9f);
        background.raycastTarget = false;

        RectTransform textRect = CreateRect("Text", hint);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(14f, 6f);
        textRect.offsetMax = new Vector2(-14f, -6f);
        TextMeshProUGUI text = textRect.gameObject.AddComponent<TextMeshProUGUI>();
        if (selectedLabel != null)
        {
            text.font = selectedLabel.font;
            text.fontSharedMaterial = selectedLabel.fontSharedMaterial;
        }
        text.color = new Color(1f, 0.95f, 0.86f);
        text.enableAutoSizing = true;
        text.fontSizeMin = 14f;
        text.fontSizeMax = 26f;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;
        text.text = string.Format(lockedHintFormat, 1, "level");

        hint.SetAsLastSibling();
        hint.gameObject.SetActive(false);
        lockedHint = hint;
        lockedHintText = text;
    }

    private static Sprite runtimePadlockSprite;

    private static Sprite RuntimePadlockSprite
    {
        get
        {
            if (runtimePadlockSprite != null) return runtimePadlockSprite;

            Texture2D texture = GeneratePadlockTexture();
            texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
            runtimePadlockSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            runtimePadlockSprite.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return runtimePadlockSprite;
        }
    }

    // Biała kłódka na przezroczystym tle (zaokrąglony korpus, pałąk, dziurka od klucza).
    private static Texture2D GeneratePadlockTexture()
    {
        const int size = 64;
        Texture2D texture = new(size, size, TextureFormat.RGBA32, false) { name = "Padlock", wrapMode = TextureWrapMode.Clamp };
        Color32[] pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float alpha = PadlockAlpha((x + 0.5f) / size, (y + 0.5f) / size, 1f / size);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
            }
        }
        texture.SetPixels32(pixels);
        texture.Apply();
        return texture;
    }

    private static float PadlockAlpha(float x, float y, float pixel)
    {
        float bodyDist = RoundedRectDistance(x - 0.5f, y - 0.34f, 0.3f, 0.24f, 0.06f);
        float body = Mathf.Clamp01(0.5f - bodyDist / pixel);

        // Pałąk: górna połowa pierścienia nad korpusem.
        float ringDist = Mathf.Abs(new Vector2(x - 0.5f, y - 0.6f).magnitude - 0.17f) - 0.055f;
        float ring = y >= 0.5f ? Mathf.Clamp01(0.5f - ringDist / pixel) : 0f;

        float keyDist = Mathf.Min(new Vector2(x - 0.5f, y - 0.38f).magnitude - 0.055f,
            RoundedRectDistance(x - 0.5f, y - 0.28f, 0.022f, 0.07f, 0.01f));
        float keyhole = Mathf.Clamp01(0.5f - keyDist / pixel);

        return Mathf.Clamp01(Mathf.Max(body * (1f - keyhole), ring));
    }

    private static float RoundedRectDistance(float x, float y, float halfWidth, float halfHeight, float radius)
    {
        Vector2 q = new(Mathf.Abs(x) - halfWidth + radius, Mathf.Abs(y) - halfHeight + radius);
        return Vector2.Max(q, Vector2.zero).magnitude + Mathf.Min(Mathf.Max(q.x, q.y), 0f) - radius;
    }
    #endregion

    // Buduje paletę: napis z nazwą wybranego koloru i siatkę próbek. Położenie bierze ze starych
    // przycisków kolorów (jeśli są), a w przeciwnym razie ustawia ją na środku widoku Skins.
    private RectTransform CreatePalette()
    {
        List<RectTransform> oldButtons = FindOldColorButtons(out TMP_Text styleSource);
        RectTransform area = CreateArea(oldButtons);

        selectedLabel = CreateLabel(area, styleSource);

        List<(string name, Color color, Material material)> colors = new()
        {
            ("Red", ColorOf(defaultRedMaterial), defaultRedMaterial),
            ("Blue", ColorOf(blueMaterial), blueMaterial),
            ("Green", ColorOf(greenMaterial), greenMaterial),
            ("Yellow", ColorOf(yellowMaterial), yellowMaterial),
        };
        foreach (SkinColor extra in extraColors)
        {
            colors.Add((extra.name, extra.color, null));
        }

        int rows = Mathf.CeilToInt(colors.Count / (float)paletteColumns);
        float availableHeight = area.rect.height - LabelHeight - swatchSpacing;
        float cellSize = Mathf.Max(10f, Mathf.Min(
            (area.rect.width - swatchSpacing * (paletteColumns - 1)) / paletteColumns,
            (availableHeight - swatchSpacing * (rows - 1)) / rows));

        RectTransform grid = CreateRect("Swatches", area);
        grid.anchorMin = Vector2.zero;
        grid.anchorMax = Vector2.one;
        grid.offsetMin = Vector2.zero;
        grid.offsetMax = new Vector2(0f, -(LabelHeight + swatchSpacing));

        GridLayoutGroup layout = grid.gameObject.AddComponent<GridLayoutGroup>();
        layout.cellSize = new Vector2(cellSize, cellSize);
        layout.spacing = new Vector2(swatchSpacing, swatchSpacing);
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = paletteColumns;
        layout.childAlignment = TextAnchor.UpperCenter;

        foreach (var (colorName, color, material) in colors)
        {
            CreateSwatch(grid, colorName, color, material);
        }

        return area;
    }

    private static Color ColorOf(Material material) => material != null ? material.color : Color.white;

    private List<RectTransform> FindOldColorButtons(out TMP_Text styleSource)
    {
        List<RectTransform> buttons = new();
        styleSource = null;

        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            UnityEvent onClick = button.onClick;
            for (int i = 0; i < onClick.GetPersistentEventCount(); i++)
            {
                if (onClick.GetPersistentTarget(i) == this && Array.IndexOf(OldColorMethods, onClick.GetPersistentMethodName(i)) >= 0)
                {
                    buttons.Add((RectTransform)button.transform);
                    if (styleSource == null) styleSource = button.GetComponentInChildren<TMP_Text>(true);
                    break;
                }
            }
        }
        return buttons;
    }

    private RectTransform CreateArea(List<RectTransform> oldButtons)
    {
        RectTransform parent = oldButtons.Count > 0 ? (RectTransform)oldButtons[0].parent : (RectTransform)transform;
        RectTransform area = CreateRect("Skin Palette", parent);
        area.anchorMin = area.anchorMax = parent.pivot;
        area.pivot = new Vector2(0.5f, 0.5f);

        if (oldButtons.Count == 0)
        {
            area.sizeDelta = defaultPaletteSize;
            area.anchoredPosition = Vector2.zero;
            return area;
        }

        Vector2 min = new(float.MaxValue, float.MaxValue);
        Vector2 max = new(float.MinValue, float.MinValue);
        Vector3[] corners = new Vector3[4];
        foreach (RectTransform button in oldButtons)
        {
            button.GetWorldCorners(corners);
            foreach (Vector3 corner in corners)
            {
                Vector2 local = parent.InverseTransformPoint(corner);
                min = Vector2.Min(min, local);
                max = Vector2.Max(max, local);
            }
        }

        // Za mało przycisków, żeby wyznaczyć cały obszar - zaczynamy od górnej krawędzi pierwszego.
        Vector2 size = Vector2.Max(max - min, defaultPaletteSize);
        area.sizeDelta = size;
        area.anchoredPosition = new Vector2((min.x + max.x) / 2f, max.y - size.y / 2f);
        area.SetSiblingIndex(oldButtons[0].GetSiblingIndex());
        return area;
    }

    private static TMP_Text CreateLabel(RectTransform area, TMP_Text styleSource)
    {
        RectTransform rect = CreateRect("Selected Color", area);
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(0f, LabelHeight);
        rect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        if (styleSource != null)
        {
            label.font = styleSource.font;
            label.fontSharedMaterial = styleSource.fontSharedMaterial;
            label.color = styleSource.color;
        }
        label.text = "Red";
        label.enableAutoSizing = true;
        label.fontSizeMax = 40f;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;
        return label;
    }

    private void CreateSwatch(RectTransform grid, string colorName, Color color, Material material)
    {
        RectTransform rect = CreateRect(colorName + " Swatch", grid);

        // Biała ramka - jej kolor (zwykły / najechany / wybrany) nadaje ColorBlock przycisku.
        Image frame = rect.gameObject.AddComponent<Image>();
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = frame;
        SetSwatchFrame(button, swatchFrameColor);

        RectTransform previewRect = CreateRect("Preview", rect);
        previewRect.anchorMin = Vector2.zero;
        previewRect.anchorMax = Vector2.one;
        previewRect.offsetMin = new Vector2(6f, 6f);
        previewRect.offsetMax = new Vector2(-6f, -6f);
        RawImage preview = previewRect.gameObject.AddComponent<RawImage>();
        preview.color = color;
        preview.raycastTarget = false;

        SkinSwatch swatch = rect.gameObject.AddComponent<SkinSwatch>();
        swatch.colorName = colorName;
        swatch.color = color;
        swatch.material = material;
        swatch.preview = preview;
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        GameObject child = new(name, typeof(RectTransform));
        child.layer = parent.gameObject.layer;
        RectTransform rect = (RectTransform)child.transform;
        rect.SetParent(parent, false);
        return rect;
    }
    #endregion
}
