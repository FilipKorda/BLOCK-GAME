using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Tools > BLOCK GAME > Build HUD Backdrops
// Dodaje na stałe (w prefabie Level_Template i w scenie Root) panel z pomarańczowym akcentem
// i cień pod tekstami HUD: Move_Text (TMP), Star_Text (TMP) oraz tekstem czasu z RealGameTime.
// Obiekty są zwykłymi elementami UI - po zbudowaniu można je dowolnie edytować.
// Ponowne uruchomienie pomija teksty, które mają już panel i cień.
public static class HudBackdropBuilder
{
    private const string LevelTemplatePath = "Assets/Gameplay Prefabs/Level_Template.prefab";
    private const string RootScenePath = "Assets/Scenes/Root.unity";
    private const string OutputFolder = "Assets/Generated UI";
    private const string PanelSpritePath = OutputFolder + "/HUD Rounded Panel.png";
    private static readonly string[] LevelHudTexts = { "Move_Text (TMP)", "Star_Text (TMP)" };

    private static readonly Color TextColor = new(1f, 0.95f, 0.86f);
    private static readonly Color PanelColor = new(0.04f, 0.02f, 0.01f, 0.72f);
    private static readonly Color AccentColor = new(1f, 0.58f, 0.12f, 0.95f);
    private static readonly Color ShadowColor = new(0f, 0f, 0f, 0.75f);
    private const float AccentHeight = 3f;

    [MenuItem("Tools/BLOCK GAME/Build HUD Backdrops")]
    public static void Build()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        Sprite panelSprite = GetPanelSprite();
        int built = BuildInLevelTemplate(panelSprite) + BuildInRootScene(panelSprite);
        AssetDatabase.SaveAssets();
        Debug.Log($"[BLOCK GAME] Zbudowano tło HUD dla {built} tekstów.");
    }

    private static int BuildInLevelTemplate(Sprite panelSprite)
    {
        GameObject root = PrefabUtility.LoadPrefabContents(LevelTemplatePath);
        int built = 0;

        foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
        {
            if (System.Array.IndexOf(LevelHudTexts, text.name) >= 0 && BuildFor(text, panelSprite)) built++;
        }

        if (built > 0) PrefabUtility.SaveAsPrefabAsset(root, LevelTemplatePath);
        PrefabUtility.UnloadPrefabContents(root);
        return built;
    }

    private static int BuildInRootScene(Sprite panelSprite)
    {
        Scene scene = SceneManager.GetSceneByPath(RootScenePath);
        bool openedHere = !scene.isLoaded;
        if (openedHere) scene = EditorSceneManager.OpenScene(RootScenePath, OpenSceneMode.Additive);

        int built = 0;
        foreach (GameObject rootObject in scene.GetRootGameObjects())
        {
            foreach (RealGameTime gameTime in rootObject.GetComponentsInChildren<RealGameTime>(true))
            {
                if (gameTime.timeText != null && BuildFor(gameTime.timeText, panelSprite)) built++;
            }
        }

        if (built > 0)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        if (openedHere) EditorSceneManager.CloseScene(scene, true);
        return built;
    }

    private static bool BuildFor(TMP_Text text, Sprite panelSprite)
    {
        HudTextBackdrop backdrop = text.GetComponent<HudTextBackdrop>();
        if (backdrop == null) backdrop = Undo.AddComponent<HudTextBackdrop>(text.gameObject);
        if (backdrop.panel != null && backdrop.shadow != null) return false;

        Undo.RecordObject(text, "Build HUD Backdrop");
        text.color = TextColor;

        RectTransform textRect = text.rectTransform;
        Transform parent = textRect.parent;
        bool visible = text.gameObject.activeSelf && text.enabled;

        if (backdrop.panel == null) backdrop.panel = CreatePanel(text, parent, panelSprite, visible);
        if (backdrop.shadow == null) backdrop.shadow = CreateShadow(text, parent, backdrop.shadowOffset, visible);

        // Kolejność rysowania: panel, cień, tekst.
        int index = textRect.GetSiblingIndex();
        backdrop.panel.SetSiblingIndex(index);
        backdrop.shadow.transform.SetSiblingIndex(index + 1);

        text.ForceMeshUpdate();
        backdrop.UpdateLayout();
        if (!visible)
        {
            foreach (Graphic graphic in backdrop.panel.GetComponentsInChildren<Graphic>(true)) graphic.enabled = false;
            backdrop.shadow.enabled = false;
        }

        EditorUtility.SetDirty(backdrop);
        return true;
    }

    private static RectTransform CreatePanel(TMP_Text text, Transform parent, Sprite panelSprite, bool visible)
    {
        GameObject panelObject = new(text.name + " Backdrop", typeof(RectTransform), typeof(Image));
        Undo.RegisterCreatedObjectUndo(panelObject, "Build HUD Backdrop");
        panelObject.layer = text.gameObject.layer;

        RectTransform rect = (RectTransform)panelObject.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = rect.anchorMax = text.rectTransform.anchorMin;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = text.rectTransform.anchoredPosition;
        rect.sizeDelta = new Vector2(120f, text.fontSize + 16f);
        rect.localScale = text.rectTransform.localScale;

        Image image = panelObject.GetComponent<Image>();
        image.sprite = panelSprite;
        image.type = Image.Type.Sliced;
        image.color = PanelColor;
        image.raycastTarget = false;
        image.enabled = visible;

        // Pomarańczowy pasek u dołu - nawiązuje do tła poziomu.
        GameObject accentObject = new("Accent", typeof(RectTransform), typeof(Image));
        accentObject.layer = panelObject.layer;
        RectTransform accent = (RectTransform)accentObject.transform;
        accent.SetParent(rect, false);
        accent.anchorMin = new Vector2(0.12f, 0f);
        accent.anchorMax = new Vector2(0.88f, 0f);
        accent.pivot = new Vector2(0.5f, 0f);
        accent.sizeDelta = new Vector2(0f, AccentHeight);
        accent.anchoredPosition = new Vector2(0f, 3f);

        Image accentImage = accentObject.GetComponent<Image>();
        accentImage.color = AccentColor;
        accentImage.raycastTarget = false;
        accentImage.enabled = visible;

        return rect;
    }

    private static TMP_Text CreateShadow(TMP_Text text, Transform parent, Vector2 offset, bool visible)
    {
        GameObject shadowObject = new(text.name + " Shadow", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(shadowObject, "Build HUD Backdrop");
        shadowObject.layer = text.gameObject.layer;

        RectTransform source = text.rectTransform;
        RectTransform rect = (RectTransform)shadowObject.transform;
        rect.SetParent(parent, false);
        rect.anchorMin = source.anchorMin;
        rect.anchorMax = source.anchorMax;
        rect.pivot = source.pivot;
        rect.sizeDelta = source.sizeDelta;
        rect.localScale = source.localScale;
        rect.localRotation = source.localRotation;
        rect.anchoredPosition = source.anchoredPosition + offset;

        TextMeshProUGUI copy = shadowObject.AddComponent<TextMeshProUGUI>();
        copy.font = text.font;
        copy.fontSharedMaterial = text.fontSharedMaterial;
        copy.fontSize = text.fontSize;
        copy.fontStyle = text.fontStyle;
        copy.alignment = text.alignment;
        copy.textWrappingMode = text.textWrappingMode;
        copy.overflowMode = text.overflowMode;
        copy.characterSpacing = text.characterSpacing;
        copy.margin = text.margin;
        copy.color = ShadowColor;
        copy.raycastTarget = false;
        copy.text = text.text;
        copy.enabled = visible;
        return copy;
    }

    // Biały prostokąt z zaokrąglonymi rogami, zapisany jako sprite 9-slice.
    private static Sprite GetPanelSprite()
    {
        Sprite existing = AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
        if (existing != null) return existing;

        if (!AssetDatabase.IsValidFolder(OutputFolder)) AssetDatabase.CreateFolder("Assets", "Generated UI");

        const int size = 32;
        const float radius = 12f;
        Texture2D texture = new(size, size, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = Mathf.Max(0f, Mathf.Abs(x + 0.5f - size / 2f) - (size / 2f - radius));
                float dy = Mathf.Max(0f, Mathf.Abs(y + 0.5f - size / 2f) - (size / 2f - radius));
                float alpha = Mathf.Clamp01(radius - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
            }
        }
        texture.SetPixels32(pixels);
        texture.Apply();
        File.WriteAllBytes(PanelSpritePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(PanelSpritePath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(PanelSpritePath);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spriteBorder = new Vector4(radius, radius, radius, radius);
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(PanelSpritePath);
    }
}
