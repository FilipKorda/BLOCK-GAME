using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Generuje nowe modele płyt (Ground, MetaGround, przyciski, mosty) i gwiazdki oraz podmienia je w prefabach.
// Uruchamiane ręcznie: Tools > BLOCK GAME > Generate Tile & Star Models. Można je uruchamiać wielokrotnie.
//
// Płyty: model Crate.fbx zostaje (wszystkie płyty są jego instancjami, a poziomy mają do nich nadpisania),
// ale jego siatka jest podmieniana na płytę z fazowanymi górnymi krawędziami, generowaną osobno dla każdego
// rozmiaru (płyta 1x0.2x1, wysoki blok MetaGround 1x1.2x1, połówki mostów...), żeby fazy się nie rozciągały.
// Materiały płyt dostają nową teksturę; ich kolory (zwykła płyta, most, pułapka) zostają bez zmian.
public static class BlockGameModelGenerator
{
    private const string OutputFolder = "Assets/Generated Models";
    private const string TileTexturePath = OutputFolder + "/TileSurface.png";
    private const string StarMeshPath = OutputFolder + "/Star.asset";
    private const string CrateModelPath = "Assets/3D Models/Crate.fbx";
    private const string PrefabFolder = "Assets/Gameplay Prefabs";
    private const string StarHolderPath = PrefabFolder + "/StarHolder.prefab";
    private const string StarMaterialPath = "Assets/Materials/Star.mat";
    private static readonly string[] TileMaterialPaths =
    {
        "Assets/Materials/Ground.mat",
        "Assets/Materials/Bridge Ground.mat",
        "Assets/Materials/Bridge.mat",
        "Assets/Materials/TrappedGround.mat",
    };

    private const float BevelSize = 0.05f;
    private const int TextureSize = 512;

    [MenuItem("Tools/BLOCK GAME/Generate Tile & Star Models")]
    public static void Generate()
    {
        if (!EditorUtility.DisplayDialog("Generate Tile & Star Models",
                "Wygenerować nowe modele płyt i gwiazdki oraz podmienić je we wszystkich prefabach w \"" + PrefabFolder + "\"?\n\n" +
                "Zmienione zostaną: prefaby płyt, przycisków, mostów, poziomów i StarHolder oraz materiały Ground, Bridge Ground, Bridge, TrappedGround i Star.",
                "Generuj", "Anuluj"))
        {
            return;
        }

        if (!AssetDatabase.IsValidFolder(OutputFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Generated Models");
        }

        Texture2D tileTexture = CreateTileTexture();
        UpdateTileMaterials(tileTexture);
        int replacedTiles = ReplaceTileMeshes();
        bool starUpdated = UpdateStar();

        AssetDatabase.SaveAssets();
        Debug.Log($"[BLOCK GAME] Podmieniono {replacedTiles} siatek płyt, gwiazdka {(starUpdated ? "zaktualizowana" : "bez zmian")}.");
    }

    #region Tile texture
    private static Texture2D CreateTileTexture()
    {
        Texture2D texture = new(TextureSize, TextureSize, TextureFormat.RGBA32, false);
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
        texture.Apply();
        File.WriteAllBytes(TileTexturePath, texture.EncodeToPNG());
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(TileTexturePath, ImportAssetOptions.ForceUpdate);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(TileTexturePath);
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.mipmapEnabled = true;
        importer.anisoLevel = 4;
        importer.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Texture2D>(TileTexturePath);
    }

    // Jasna, metalowa płyta: brzeg na fazie, ciemniejszy rowek wzdłuż krawędzi, panel z plamami i ziarnem.
    private static float TileValue(float s, float t, int x, int y)
    {
        float edge = Mathf.Min(Mathf.Min(s, 1f - s), Mathf.Min(t, 1f - t));

        float value = 0.8f;
        value += (ValueNoise(s * 6f, t * 6f) - 0.5f) * 0.1f;
        value += (Hash(x, y) - 0.5f) * 0.05f;
        value -= Mathf.Clamp01((0.5f - edge) * 0.25f) * 0.1f;

        if (edge < BevelSize) value = 0.92f + (Hash(x, y) - 0.5f) * 0.04f;
        else if (edge < BevelSize + 0.018f) value *= 0.6f;
        else if (edge > 0.2f && edge < 0.21f) value *= 0.9f;

        return value;
    }

    private static float ValueNoise(float x, float y)
    {
        int x0 = Mathf.FloorToInt(x), y0 = Mathf.FloorToInt(y);
        float fx = x - x0, fy = y - y0;
        fx = fx * fx * (3f - 2f * fx);
        fy = fy * fy * (3f - 2f * fy);

        float a = Hash(x0, y0, 7), b = Hash(x0 + 1, y0, 7);
        float c = Hash(x0, y0 + 1, 7), d = Hash(x0 + 1, y0 + 1, 7);
        return Mathf.Lerp(Mathf.Lerp(a, b, fx), Mathf.Lerp(c, d, fx), fy);
    }

    private static float Hash(int x, int y, int seed = 0)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 2246822519);
        h = (h ^ (h >> 13)) * 1274126177;
        return (h ^ (h >> 16)) / (float)uint.MaxValue;
    }

    private static void UpdateTileMaterials(Texture2D tileTexture)
    {
        foreach (string path in TileMaterialPaths)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null) continue;

            Undo.RecordObject(material, "Generate Tile Models");
            material.mainTexture = tileTexture;
            material.mainTextureScale = Vector2.one;
            material.mainTextureOffset = Vector2.zero;
            EditorUtility.SetDirty(material);
        }
    }
    #endregion

    #region Tile meshes
    private static int ReplaceTileMeshes()
    {
        Mesh crateMesh = AssetDatabase.LoadAllAssetsAtPath(CrateModelPath).OfType<Mesh>().FirstOrDefault();
        if (crateMesh == null)
        {
            Debug.LogError("[BLOCK GAME] Nie znaleziono siatki w " + CrateModelPath);
            return 0;
        }

        // Najpierw prefaby płyt i przycisków, potem poziomy - wtedy zagnieżdżone płyty w poziomach
        // dostają nową siatkę z prefabu źródłowego, bez dodatkowych nadpisań.
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .OrderBy(path => Path.GetFileName(path).StartsWith("Level") ? 1 : 0)
            .ThenBy(path => path)
            .ToArray();

        int replaced = 0;
        foreach (string prefabPath in prefabPaths)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(prefabPath);
            bool changed = false;

            foreach (MeshFilter meshFilter in root.GetComponentsInChildren<MeshFilter>(true))
            {
                Mesh current = meshFilter.sharedMesh;
                bool isTile = current == crateMesh || (current != null && current.name.StartsWith("Tile ") && AssetDatabase.GetAssetPath(current).StartsWith(OutputFolder));
                if (!isTile) continue;

                Mesh tileMesh = GetTileMesh(meshFilter.transform.lossyScale);
                if (current == tileMesh) continue;

                meshFilter.sharedMesh = tileMesh;
                changed = true;
                replaced++;
            }

            if (changed) PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }
        return replaced;
    }

    // Siatka płyty dla danej skali obiektu. Siatka ma wymiary -1..1 jak Crate, a fazy
    // są liczone w jednostkach świata, żeby miały tę samą wielkość na każdej płycie.
    private static Mesh GetTileMesh(Vector3 lossyScale)
    {
        Vector3 half = new(Round(Mathf.Abs(lossyScale.x)), Round(Mathf.Abs(lossyScale.y)), Round(Mathf.Abs(lossyScale.z)));
        string name = $"Tile {half.x * 2f:0.###}x{half.y * 2f:0.###}x{half.z * 2f:0.###}";
        string path = $"{OutputFolder}/{name}.asset";

        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        if (mesh == null)
        {
            mesh = new Mesh();
            AssetDatabase.CreateAsset(mesh, path);
        }

        BuildTileMesh(mesh, half);
        mesh.name = name;
        EditorUtility.SetDirty(mesh);
        return mesh;
    }

    private static float Round(float value) => Mathf.Round(value * 1000f) / 1000f;

    private static void BuildTileMesh(Mesh mesh, Vector3 half)
    {
        float bevel = Mathf.Min(BevelSize, half.x * 0.4f, half.y * 0.8f, half.z * 0.4f);
        float top = half.y, shoulder = half.y - bevel, bottom = -half.y;
        float ix = half.x - bevel, iz = half.z - bevel;

        MeshBuilder builder = new(half);

        // Góra i fazy - UV z rzutu z góry, żeby tekstura płynnie przechodziła z panelu na fazę.
        builder.AddQuad(Vector3.up,
            new(-ix, top, -iz), new(-ix, top, iz), new(ix, top, iz), new(ix, top, -iz));
        builder.AddQuad(new Vector3(1f, 1f, 0f),
            new(ix, top, -iz), new(ix, top, iz), new(half.x, shoulder, half.z), new(half.x, shoulder, -half.z));
        builder.AddQuad(new Vector3(-1f, 1f, 0f),
            new(-ix, top, iz), new(-ix, top, -iz), new(-half.x, shoulder, -half.z), new(-half.x, shoulder, half.z));
        builder.AddQuad(new Vector3(0f, 1f, 1f),
            new(ix, top, iz), new(-ix, top, iz), new(-half.x, shoulder, half.z), new(half.x, shoulder, half.z));
        builder.AddQuad(new Vector3(0f, 1f, -1f),
            new(-ix, top, -iz), new(ix, top, -iz), new(half.x, shoulder, -half.z), new(-half.x, shoulder, -half.z));

        // Boki i spód.
        builder.AddSide(Vector3.right, new(half.x, shoulder, -half.z), new(half.x, shoulder, half.z), bottom);
        builder.AddSide(Vector3.left, new(-half.x, shoulder, half.z), new(-half.x, shoulder, -half.z), bottom);
        builder.AddSide(Vector3.forward, new(half.x, shoulder, half.z), new(-half.x, shoulder, half.z), bottom);
        builder.AddSide(Vector3.back, new(-half.x, shoulder, -half.z), new(half.x, shoulder, -half.z), bottom);
        builder.AddQuad(Vector3.down,
            new(-half.x, bottom, -half.z), new(half.x, bottom, -half.z), new(half.x, bottom, half.z), new(-half.x, bottom, half.z));

        builder.Apply(mesh);
    }

    private class MeshBuilder
    {
        private readonly Vector3 half;
        private readonly List<Vector3> vertices = new();
        private readonly List<Vector2> uvs = new();
        private readonly List<int> triangles = new();

        public MeshBuilder(Vector3 half) => this.half = half;

        // Czworokąt w jednostkach świata; UV z rzutu z góry (x, z).
        public void AddQuad(Vector3 outward, params Vector3[] corners)
        {
            AddPolygon(outward, corners, corners.Select(c => new Vector2((c.x + half.x) / (2f * half.x), (c.z + half.z) / (2f * half.z))).ToArray());
        }

        // Pionowa ściana od krawędzi fazy do spodu; UV z jednolitego pasa tekstury (bez rowków).
        public void AddSide(Vector3 outward, Vector3 topA, Vector3 topB, float bottom)
        {
            Vector3 bottomB = new(topB.x, bottom, topB.z);
            Vector3 bottomA = new(topA.x, bottom, topA.z);
            float height = topA.y - bottom;
            float vScale = 0.1f / Mathf.Max(height, 0.2f);
            Vector2[] sideUvs =
            {
                new(0.3f, 0.45f + height * vScale), new(0.7f, 0.45f + height * vScale), new(0.7f, 0.45f), new(0.3f, 0.45f)
            };
            AddPolygon(outward, new[] { topA, topB, bottomB, bottomA }, sideUvs);
        }

        private void AddPolygon(Vector3 outward, Vector3[] corners, Vector2[] cornerUvs)
        {
            int start = vertices.Count;
            for (int i = 0; i < corners.Length; i++)
            {
                // Siatka ma wymiary -1..1 jak Crate - skalę nadaje transform obiektu.
                vertices.Add(new Vector3(corners[i].x / half.x, corners[i].y / half.y, corners[i].z / half.z));
                uvs.Add(cornerUvs[i]);
            }

            // Unity rysuje przód trójkąta, gdy Cross(b - a, c - a) wskazuje na zewnątrz.
            Vector3 normal = Vector3.Cross(corners[1] - corners[0], corners[2] - corners[0]);
            bool flip = Vector3.Dot(normal, outward) < 0f;
            for (int i = 1; i < corners.Length - 1; i++)
            {
                if (flip) triangles.AddRange(new[] { start, start + i + 1, start + i });
                else triangles.AddRange(new[] { start, start + i, start + i + 1 });
            }
        }

        public void Apply(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            mesh.RecalculateBounds();
        }
    }
    #endregion

    #region Star
    private static bool UpdateStar()
    {
        Mesh starMesh = AssetDatabase.LoadAssetAtPath<Mesh>(StarMeshPath);
        if (starMesh == null)
        {
            starMesh = new Mesh();
            AssetDatabase.CreateAsset(starMesh, StarMeshPath);
        }
        BuildStarMesh(starMesh);
        starMesh.name = "Star";
        EditorUtility.SetDirty(starMesh);

        UpdateStarMaterial();

        GameObject root = PrefabUtility.LoadPrefabContents(StarHolderPath);
        bool changed = false;
        foreach (Star star in root.GetComponentsInChildren<Star>(true))
        {
            if (!star.TryGetComponent(out MeshFilter meshFilter)) continue;

            meshFilter.sharedMesh = starMesh;
            // Gwiazdka stoi pionowo i obraca się wokół osi Y (Star.Update) - jak w Bloxorz.
            star.transform.localRotation = Quaternion.identity;
            changed = true;
        }

        if (changed) PrefabUtility.SaveAsPrefabAsset(root, StarHolderPath);
        PrefabUtility.UnloadPrefabContents(root);
        return changed;
    }

    // Pięcioramienna gwiazdka z fasetami: krawędź w płaszczyźnie XY, środek wypchnięty do przodu i do tyłu.
    // Promień 1.2 przy skali 0.25 daje gwiazdkę o szerokości ok. 0.6 jednostki.
    private static void BuildStarMesh(Mesh mesh)
    {
        const int points = 5;
        const float outerRadius = 1.2f, innerRadius = 0.52f, depth = 0.42f;

        Vector3[] rim = new Vector3[points * 2];
        for (int i = 0; i < rim.Length; i++)
        {
            float angle = Mathf.PI / 2f + i * Mathf.PI / points;
            float radius = i % 2 == 0 ? outerRadius : innerRadius;
            rim[i] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0f);
        }

        List<Vector3> vertices = new();
        List<int> triangles = new();
        foreach (float side in new[] { 1f, -1f })
        {
            Vector3 apex = new(0f, 0f, -depth * side);
            for (int i = 0; i < rim.Length; i++)
            {
                Vector3 a = apex, b = rim[i], c = rim[(i + 1) % rim.Length];
                Vector3 outward = (a + b + c) / 3f;
                if (Vector3.Dot(Vector3.Cross(b - a, c - a), outward) < 0f) (b, c) = (c, b);

                int start = vertices.Count;
                vertices.AddRange(new[] { a, b, c });
                triangles.AddRange(new[] { start, start + 1, start + 2 });
            }
        }

        mesh.Clear();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private static void UpdateStarMaterial()
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(StarMaterialPath);
        if (material == null) return;

        Undo.RecordObject(material, "Generate Star Model");
        material.SetFloat("_Glossiness", 0.75f);
        material.SetFloat("_Metallic", 0.4f);
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", material.color * 0.35f);
        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        EditorUtility.SetDirty(material);
    }
    #endregion
}
