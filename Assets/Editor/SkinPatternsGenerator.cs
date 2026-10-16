using System.IO;
using UnityEditor;
using UnityEngine;

// Tools > BLOCK GAME > Generate Skin Patterns
// Zapisuje wzory wszystkich skinów jako PNG w Assets/Resources/SkinPatterns (nazwa pliku = nazwa koloru).
// Pliki można potem podmienić na własne grafiki (w odcieniach szarości - kolor nadaje skin).
// Istniejące pliki są nadpisywane.
public static class SkinPatternsGenerator
{
    private const string Folder = "Assets/Resources/" + SkinPatterns.ResourcesFolder;

    [MenuItem("Tools/BLOCK GAME/Generate Skin Patterns")]
    public static void Generate()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets/Resources", SkinPatterns.ResourcesFolder);

        foreach (string patternName in SkinPatterns.Names)
        {
            string path = $"{Folder}/{patternName}.png";
            Texture2D texture = SkinPatterns.Generate(patternName);
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.wrapMode = TextureWrapMode.Repeat;
            importer.mipmapEnabled = true;
            importer.filterMode = FilterMode.Trilinear;
            importer.anisoLevel = 4;
            importer.SaveAndReimport();
        }

        Debug.Log($"[BLOCK GAME] Zapisano {SkinPatterns.Names.Length} wzorów skinów w {Folder}.");
    }
}
