using System.Collections.Generic;
using UnityEngine;

// Wygląd bloku gracza w stylu Bloxorz: każdy segment 1x1 ma ciemną fugę, jasny skos i panel
// z delikatnym ziarnem. Tekstura jest w odcieniach szarości, a kolor skina nadaje materiał.
// Długi blok (1x2x1) dostaje siatkę z UV liczonym w jednostkach, więc wygląda jak dwa sklejone segmenty.
public static class BlockSkin
{
    private const int TextureSize = 256;

    private static Texture2D tileTexture;
    private static readonly Dictionary<Material, Material> skinnedMaterials = new();
    private static readonly Dictionary<Vector3, Mesh> meshes = new();

    public static Texture2D TileTexture
    {
        get
        {
            if (tileTexture == null) tileTexture = GenerateTileTexture();
            return tileTexture;
        }
    }

    public static void Apply(Renderer renderer, Material colorMaterial)
    {
        if (renderer == null || colorMaterial == null) return;

        renderer.sharedMaterial = GetSkinnedMaterial(colorMaterial);

        // Podmieniamy tylko wbudowany sześcian (albo naszą siatkę) - inne modele zostają bez zmian.
        if (renderer.TryGetComponent(out MeshFilter meshFilter) && meshFilter.sharedMesh != null
            && (meshFilter.sharedMesh.name == "Cube" || meshFilter.sharedMesh.name.StartsWith("Block Skin")))
        {
            meshFilter.sharedMesh = GetMesh(renderer.transform.localScale);
        }
    }

    private static Material GetSkinnedMaterial(Material colorMaterial)
    {
        if (!skinnedMaterials.TryGetValue(colorMaterial, out Material skinned) || skinned == null)
        {
            // Materiał skina może mieć własny wzór (SkinPatterns) - bez niego klasyczna tekstura segmentu.
            skinned = new Material(colorMaterial)
            {
                name = colorMaterial.name + " (Block Skin)",
                hideFlags = HideFlags.DontUnloadUnusedAsset,
                mainTexture = colorMaterial.mainTexture != null ? colorMaterial.mainTexture : TileTexture
            };
            skinnedMaterials[colorMaterial] = skinned;
        }
        return skinned;
    }

    #region Mesh
    private static Mesh GetMesh(Vector3 scale)
    {
        scale = new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
        if (!meshes.TryGetValue(scale, out Mesh mesh) || mesh == null)
        {
            mesh = BuildBoxMesh(scale);
            meshes[scale] = mesh;
        }
        return mesh;
    }

    // Sześcian 1x1x1 (jak wbudowany), ale UV każdej ściany liczone w jednostkach świata,
    // żeby tekstura powtarzała się co 1 jednostkę niezależnie od skali obiektu.
    private static Mesh BuildBoxMesh(Vector3 scale)
    {
        Vector3[] normals = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };

        List<Vector3> vertices = new();
        List<Vector3> vertexNormals = new();
        List<Vector2> uvs = new();
        List<int> triangles = new();

        foreach (Vector3 normal in normals)
        {
            Vector3 vAxis = Mathf.Abs(normal.y) > 0.5f ? Vector3.forward : Vector3.up;
            Vector3 uAxis = Vector3.Cross(vAxis, normal);
            // Unity rysuje trójkąty zgodnie z ruchem wskazówek zegara patrząc od strony normalnej.
            if (Vector3.Dot(Vector3.Cross(uAxis, vAxis), normal) > 0f) uAxis = -uAxis;

            float uLength = Vector3.Scale(uAxis, scale).magnitude;
            float vLength = Vector3.Scale(vAxis, scale).magnitude;

            int start = vertices.Count;
            Vector3 center = normal * 0.5f;
            Vector2[] corners = { new(-0.5f, -0.5f), new(-0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, -0.5f) };
            foreach (Vector2 corner in corners)
            {
                vertices.Add(center + uAxis * corner.x + vAxis * corner.y);
                vertexNormals.Add(normal);
                uvs.Add(new Vector2((corner.x + 0.5f) * uLength, (corner.y + 0.5f) * vLength));
            }

            triangles.AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });
        }

        Mesh mesh = new()
        {
            name = $"Block Skin {scale.x}x{scale.y}x{scale.z}",
            hideFlags = HideFlags.DontUnloadUnusedAsset
        };
        mesh.SetVertices(vertices);
        mesh.SetNormals(vertexNormals);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
        return mesh;
    }
    #endregion

    #region Texture
    private static Texture2D GenerateTileTexture()
    {
        Texture2D texture = new(TextureSize, TextureSize, TextureFormat.RGBA32, true)
        {
            name = "Block Skin Tile",
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Trilinear,
            anisoLevel = 4,
            hideFlags = HideFlags.DontUnloadUnusedAsset
        };

        Color32[] pixels = new Color32[TextureSize * TextureSize];
        for (int y = 0; y < TextureSize; y++)
        {
            for (int x = 0; x < TextureSize; x++)
            {
                float s = (x + 0.5f) / TextureSize;
                float t = (y + 0.5f) / TextureSize;
                byte value = (byte)Mathf.RoundToInt(Mathf.Clamp01(TileValue(s, t, x, y)) * 255f);
                pixels[y * TextureSize + x] = new Color32(value, value, value, 255);
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply(true, true);
        return texture;
    }

    // Jasność segmentu w punkcie (s, t) z zakresu 0-1.
    private static float TileValue(float s, float t, int x, int y)
    {
        float edge = Mathf.Min(Mathf.Min(s, 1f - s), Mathf.Min(t, 1f - t));

        // Ciemna fuga na styku segmentów.
        if (edge < 0.025f) return 0.28f;

        // Skos: jasny od góry i z lewej, ciemniejszy od dołu i z prawej (światło z lewego górnego rogu).
        if (edge < 0.085f)
        {
            bool nearLeftOrTop = Mathf.Min(s, 1f - t) <= Mathf.Min(1f - s, t);
            return nearLeftOrTop ? 1f : 0.62f;
        }

        // Panel: lekki gradient od lewego górnego rogu, cienka wewnętrzna linia i drobne ziarno.
        float panel = 0.82f + 0.08f * ((1f - s) + t - 1f);
        if (edge > 0.13f && edge < 0.145f) panel -= 0.1f;
        panel += (Hash(x, y) - 0.5f) * 0.06f;
        return panel;
    }

    private static float Hash(int x, int y)
    {
        uint h = (uint)(x * 374761393 + y * 668265263);
        h = (h ^ (h >> 13)) * 1274126177;
        return (h ^ (h >> 16)) / (float)uint.MaxValue;
    }
    #endregion
}
