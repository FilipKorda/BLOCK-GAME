using System.Collections.Generic;
using UnityEngine;

// Wzory (tekstury w odcieniach szarości) skinów gracza - każdy kolor ma własny wzór.
// Wzór wybierany jest po nazwie koloru. Najpierw szukany jest plik Resources/SkinPatterns/<nazwa>.png
// (Tools > BLOCK GAME > Generate Skin Patterns zapisuje tam wszystkie wzory - można je podmienić
// na własne grafiki), a gdy go nie ma, wzór generuje się w trakcie gry.
// Każdy wzór ma wspólną ramkę segmentu (ciemna fuga + skos), więc blok dalej wygląda jak sklejone segmenty.
public static class SkinPatterns
{
    public const string ResourcesFolder = "SkinPatterns";
    public const int TextureSize = 256;

    public static readonly string[] Names =
    {
        "Red", "Blue", "Green", "Yellow", "Classic", "Orange", "Gold", "Lime",
        "Teal", "Cyan", "Navy", "Purple", "Pink", "Brown", "White", "Graphite",
    };

    private static readonly Dictionary<string, Texture2D> cache = new();

    // Wzór dla koloru o podanej nazwie (null = brak wzoru o tej nazwie).
    public static Texture2D Get(string patternName)
    {
        if (string.IsNullOrEmpty(patternName)) return null;
        if (cache.TryGetValue(patternName, out Texture2D cached) && cached != null) return cached;

        Texture2D texture = Resources.Load<Texture2D>($"{ResourcesFolder}/{patternName}");
        if (texture == null && System.Array.IndexOf(Names, patternName) >= 0)
        {
            texture = Generate(patternName);
            texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        if (texture != null) cache[patternName] = texture;
        return texture;
    }

    public static Texture2D Generate(string patternName)
    {
        Texture2D texture = new(TextureSize, TextureSize, TextureFormat.RGBA32, true)
        {
            name = patternName,
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Trilinear,
            anisoLevel = 4
        };

        Color32[] pixels = new Color32[TextureSize * TextureSize];
        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float s = (x + 0.5f) / TextureSize;
                float t = (y + 0.5f) / TextureSize;
                float value = WithFrame(s, t, Mathf.Lerp(0.55f, 1f, Mathf.Clamp01(Pattern(patternName, s, t))));
                value += (Hash(x, y) - 0.5f) * 0.03f;
                byte b = (byte)Mathf.RoundToInt(Mathf.Clamp01(value) * 255f);
                pixels[y * TextureSize + x] = new Color32(b, b, b, 255);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(true);
        return texture;
    }

    // Wspólna ramka segmentu: ciemna fuga na krawędzi i skos (jasny od góry/lewej, ciemny od dołu/prawej).
    private static float WithFrame(float s, float t, float inner)
    {
        float edge = Mathf.Min(Mathf.Min(s, 1f - s), Mathf.Min(t, 1f - t));
        if (edge < 0.025f) return 0.28f;
        if (edge < 0.075f)
        {
            bool nearLeftOrTop = Mathf.Min(s, 1f - t) <= Mathf.Min(1f - s, t);
            return nearLeftOrTop ? 1f : 0.62f;
        }
        return inner;
    }

    // Wzór wewnątrz segmentu, wartości 0-1 (0 = najciemniej, 1 = najjaśniej).
    private static float Pattern(string patternName, float s, float t)
    {
        float cx = s - 0.5f, cy = t - 0.5f;
        float radius = Mathf.Sqrt(cx * cx + cy * cy);

        switch (patternName)
        {
            case "Red": // klasyczny panel: lekki gradient i wewnętrzna linia
            {
                float edge = Mathf.Min(Mathf.Min(s, 1f - s), Mathf.Min(t, 1f - t));
                float panel = 0.7f + 0.2f * ((1f - s) + t - 1f);
                return edge > 0.13f && edge < 0.15f ? panel - 0.35f : panel;
            }
            case "Blue": // poziome paski
                return Fract(t * 6f) < 0.5f ? 0.35f : 0.95f;
            case "Green": // szachownica
                return (Mathf.FloorToInt(s * 4f) + Mathf.FloorToInt(t * 4f)) % 2 == 0 ? 0.3f : 0.95f;
            case "Yellow": // kropki
            {
                float dx = Fract(s * 4f) - 0.5f, dy = Fract(t * 4f) - 0.5f;
                return Smooth(0.3f, 0.26f, Mathf.Sqrt(dx * dx + dy * dy)) * 0.65f + 0.3f;
            }
            case "Classic": // rdza: wielowarstwowy szum
                return 0.2f + Fbm(s * 5f, t * 5f, 3) * 1.1f;
            case "Orange": // skośne pasy
                return Fract((s + t) * 5f) < 0.5f ? 0.3f : 0.95f;
            case "Gold": // szczotkowany metal z nitami w rogach
            {
                float brushed = 0.55f + (ValueNoise(s * 2f, t * 60f, 3) - 0.5f) * 0.7f;
                float rivet = 1f;
                foreach (Vector2 corner in new[] { new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.2f), new Vector2(0.2f, 0.8f), new Vector2(0.8f, 0.8f) })
                {
                    float d = Vector2.Distance(new Vector2(s, t), corner);
                    if (d < 0.05f) rivet = Mathf.Min(rivet, 0.5f + (corner.y - t + corner.x - s) * 8f);
                }
                return rivet < 1f ? rivet : brushed;
            }
            case "Lime": // romby
            {
                float d = Mathf.Abs(Fract(s * 4f) - 0.5f) + Mathf.Abs(Fract(t * 4f) - 0.5f);
                return d < 0.35f ? 0.95f : 0.3f;
            }
            case "Teal": // koncentryczne kwadraty
                return Fract(Mathf.Max(Mathf.Abs(cx), Mathf.Abs(cy)) * 10f) < 0.5f ? 0.35f : 0.95f;
            case "Cyan": // fale
                return 0.5f + 0.5f * Mathf.Sin((t * 5f + Mathf.Sin(s * Mathf.PI * 4f) * 0.25f) * Mathf.PI * 2f);
            case "Navy": // siatka jak na planie
                return Fract(s * 8f) < 0.08f || Fract(t * 8f) < 0.08f ? 1f : 0.35f;
            case "Purple": // pięcioramienna gwiazda w środku
            {
                float angle = Mathf.Atan2(cy, cx) + Mathf.PI / 2f;
                float starRadius = 0.18f + 0.14f * Mathf.Pow(Mathf.Abs(Mathf.Cos(angle * 2.5f)), 3f);
                return radius < starRadius ? 1f : 0.35f + radius * 0.4f;
            }
            case "Pink": // okręgi
                return Fract(radius * 9f) < 0.5f ? 0.4f : 0.95f;
            case "Brown": // słoje drewna
            {
                float rings = Vector2.Distance(new Vector2(s, t * 0.35f), new Vector2(0.5f, -0.6f)) * 14f + Fbm(s * 3f, t * 3f, 3) * 1.5f;
                return 0.45f + 0.5f * Mathf.Pow(Mathf.Abs(Mathf.Sin(rings * Mathf.PI)), 0.5f);
            }
            case "White": // marmur z żyłkami
            {
                float vein = Mathf.Abs(Mathf.Sin((s + t + Fbm(s * 3f, t * 3f, 4) * 1.6f) * Mathf.PI * 3f));
                return 0.35f + 0.65f * Mathf.Pow(vein, 0.25f);
            }
            case "Graphite": // włókno węglowe (splot)
            {
                int cellX = Mathf.FloorToInt(s * 8f), cellY = Mathf.FloorToInt(t * 8f);
                float u = Fract(s * 8f), v = Fract(t * 8f);
                bool horizontal = (cellX / 2 + cellY / 2) % 2 == 0;
                float weave = horizontal ? Mathf.Sin(v * Mathf.PI) : Mathf.Sin(u * Mathf.PI);
                return 0.25f + 0.65f * weave;
            }
            default:
                return 0.8f;
        }
    }

    private static float Fract(float value) => value - Mathf.Floor(value);

    private static float Smooth(float edge0, float edge1, float value) => Mathf.SmoothStep(0f, 1f, (value - edge0) / (edge1 - edge0));

    private static float Fbm(float x, float y, int octaves)
    {
        float value = 0f, amplitude = 0.5f;
        for (int i = 0; i < octaves; i++)
        {
            value += ValueNoise(x, y, 11 + i) * amplitude;
            x *= 2f;
            y *= 2f;
            amplitude *= 0.5f;
        }
        return value;
    }

    private static float ValueNoise(float x, float y, int seed)
    {
        int x0 = Mathf.FloorToInt(x), y0 = Mathf.FloorToInt(y);
        float fx = x - x0, fy = y - y0;
        fx = fx * fx * (3f - 2f * fx);
        fy = fy * fy * (3f - 2f * fy);

        float a = Hash(x0, y0, seed), b = Hash(x0 + 1, y0, seed);
        float c = Hash(x0, y0 + 1, seed), d = Hash(x0 + 1, y0 + 1, seed);
        return Mathf.Lerp(Mathf.Lerp(a, b, fx), Mathf.Lerp(c, d, fx), fy);
    }

    private static float Hash(int x, int y, int seed = 0)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
        h = (h ^ (h >> 13)) * 1274126177;
        return (h ^ (h >> 16)) / (float)uint.MaxValue;
    }
}
