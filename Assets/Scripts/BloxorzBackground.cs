using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Pomarańczowo-czarne tło w stylu Bloxorz: jasna, pomarańczowa poświata w środku,
// delikatne promienie i ciemne, niemal czarne brzegi.
// Dodawane automatycznie do kamery z tagiem MainCamera w każdej wczytanej scenie,
// więc nie trzeba niczego ustawiać w scenach. Można je też dodać ręcznie do kamery,
// żeby zmienić ustawienia w inspektorze - wtedy automat go nie zdubluje.
[RequireComponent(typeof(Camera))]
public class BloxorzBackground : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color glowColor = new(1f, 0.6f, 0.12f);
    [SerializeField] private Color midColor = new(0.6f, 0.2f, 0.02f);
    [SerializeField] private Color edgeColor = new(0.02f, 0.01f, 0.005f);

    [Header("Shape")]
    [Tooltip("Środek poświaty w ułamkach ekranu (0-1).")]
    [SerializeField] private Vector2 glowCenter = new(0.5f, 0.45f);
    [Tooltip("Promień poświaty względem wielkości ekranu.")]
    [SerializeField] private float glowRadius = 0.75f;
    [SerializeField, Range(0, 48)] private int rayCount = 16;
    [SerializeField, Range(0f, 1f)] private float rayStrength = 0.15f;

    private const int TextureSize = 512;
    private static Texture2D cachedTexture;
    private static int cachedSettingsHash;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RegisterSceneHook()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Sceny, które mają własne tło i nie dostają automatycznie tła Bloxorz.
    private static readonly string[] ExcludedScenes = { "MainMenu", "Tutorial" };

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (System.Array.IndexOf(ExcludedScenes, scene.name) >= 0) return;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (Camera sceneCamera in root.GetComponentsInChildren<Camera>(true))
            {
                if (sceneCamera.CompareTag("MainCamera") && !sceneCamera.TryGetComponent<BloxorzBackground>(out _))
                {
                    sceneCamera.gameObject.AddComponent<BloxorzBackground>();
                }
            }
        }
    }

    private void Start()
    {
        Camera targetCamera = GetComponent<Camera>();
        targetCamera.clearFlags = CameraClearFlags.SolidColor;
        targetCamera.backgroundColor = edgeColor;

        // Płótno przypięte do kamery, ustawione tuż przed dalekim planem i z najniższą kolejnością,
        // więc rysuje się za całą geometrią i za każdym innym UI.
        GameObject canvasObject = new("Bloxorz Background", typeof(RectTransform), typeof(Canvas));
        canvasObject.layer = FirstVisibleLayer(targetCamera.cullingMask);
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = targetCamera;
        canvas.planeDistance = targetCamera.farClipPlane * 0.95f;
        canvas.sortingOrder = short.MinValue;

        // Kwadratowa grafika przycięta do ekranu - poświata zostaje okrągła przy każdych proporcjach.
        GameObject imageObject = new("Image", typeof(RectTransform), typeof(RawImage), typeof(AspectRatioFitter));
        imageObject.layer = canvasObject.layer;
        imageObject.transform.SetParent(canvasObject.transform, false);

        RawImage image = imageObject.GetComponent<RawImage>();
        image.texture = GetTexture();
        image.raycastTarget = false;

        AspectRatioFitter fitter = imageObject.GetComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 1f;
    }

    private static int FirstVisibleLayer(int cullingMask)
    {
        for (int layer = 0; layer < 32; layer++)
        {
            if ((cullingMask & (1 << layer)) != 0) return layer;
        }
        return 0;
    }

    private Texture2D GetTexture()
    {
        int settingsHash = (glowColor, midColor, edgeColor, glowCenter, glowRadius, rayCount, rayStrength).GetHashCode();
        if (cachedTexture == null || cachedSettingsHash != settingsHash)
        {
            cachedTexture = GenerateTexture();
            cachedSettingsHash = settingsHash;
        }
        return cachedTexture;
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new(TextureSize, TextureSize, TextureFormat.RGBA32, false)
        {
            name = "Bloxorz Background",
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            hideFlags = HideFlags.DontSave
        };

        Color32[] pixels = new Color32[TextureSize * TextureSize];
        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                Vector2 offset = new Vector2((x + 0.5f) / TextureSize, (y + 0.5f) / TextureSize) - glowCenter;
                float distance = offset.magnitude / glowRadius;

                Color color = Color.Lerp(glowColor, midColor, Mathf.SmoothStep(0f, 1f, distance / 0.5f));
                color = Color.Lerp(color, edgeColor, Mathf.SmoothStep(0f, 1f, (distance - 0.3f) / 0.7f));

                // Promienie rozchodzące się od środka - najmocniejsze blisko poświaty.
                if (rayCount > 0)
                {
                    float angle = Mathf.Atan2(offset.y, offset.x);
                    float ray = Mathf.Pow(0.5f + 0.5f * Mathf.Cos(angle * rayCount), 3f);
                    color *= 1f + rayStrength * (ray - 0.3f) * Mathf.Clamp01(1.2f - distance);
                }

                // Delikatny szum usuwa pasmowanie gradientu.
                float noise = (Hash(x, y) - 0.5f) / 255f;
                color.r += noise;
                color.g += noise;
                color.b += noise;
                color.a = 1f;

                pixels[y * TextureSize + x] = color;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(false, true);
        return texture;
    }

    private static float Hash(int x, int y)
    {
        uint h = (uint)(x * 374761393 + y * 668265263);
        h = (h ^ (h >> 13)) * 1274126177;
        return (h ^ (h >> 16)) / (float)uint.MaxValue;
    }
}
