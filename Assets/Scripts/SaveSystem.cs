using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Trwały zapis postępu gracza w pliku JSON (Application.persistentDataPath/save.json):
// odblokowane poziomy i kody, wygrane poziomy (odblokowują skiny), wybrany skin i zdobyte osiągnięcia.
// Plik przetrwa zamknięcie gry. Zapis jest atomowy (plik tymczasowy + podmiana),
// więc przerwanie gry w trakcie zapisu nie psuje postępu.
public static class SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int version = 1;
        public List<int> reachedLevels = new();
        public int unlockedCodesIndex = -1;
        public List<string> finishedLevels = new();
        public bool hasSkin;
        public string skinName;
        public Color skinColor = Color.white;
        public string skinPattern;
        public List<string> achievements = new();

        public bool hasScreenSettings;
        public bool fullscreen = true;
        public int resolutionWidth;
        public int resolutionHeight;
    }

    private const string FileName = "save.json";

    private static SaveData data;

    public static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static SaveData Data
    {
        get
        {
            if (data == null) Load();
            return data;
        }
    }

    public static void Load()
    {
        data = new SaveData();
        try
        {
            if (File.Exists(FilePath))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(FilePath), data);
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[SaveSystem] Nie udało się wczytać zapisu, start od zera: {exception.Message}");
            data = new SaveData();
        }
    }

    public static void Save()
    {
        try
        {
            string tempPath = FilePath + ".tmp";
            File.WriteAllText(tempPath, JsonUtility.ToJson(Data, true));
            if (File.Exists(FilePath)) File.Replace(tempPath, FilePath, null);
            else File.Move(tempPath, FilePath);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[SaveSystem] Nie udało się zapisać postępu: {exception.Message}");
        }
    }

    public static void DeleteSave()
    {
        try
        {
            if (File.Exists(FilePath)) File.Delete(FilePath);
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[SaveSystem] Nie udało się usunąć zapisu: {exception.Message}");
        }
        data = new SaveData();
    }

    #region Game state
    public static void RestoreProgress(GameManager gameManager)
    {
        gameManager.completedLevels = new HashSet<int>(Data.reachedLevels);
        gameManager.currentUnlockedAvailableCodesIndex = Data.unlockedCodesIndex;
        gameManager.finishedLevels = new HashSet<string>(Data.finishedLevels);
    }

    public static void SaveProgress(GameManager gameManager)
    {
        Data.reachedLevels = new List<int>(gameManager.completedLevels);
        Data.unlockedCodesIndex = gameManager.currentUnlockedAvailableCodesIndex;
        Data.finishedLevels = new List<string>(gameManager.finishedLevels);
        Save();
    }

    public static void SaveSkin(Material material)
    {
        if (material == null) return;

        Data.hasSkin = true;
        Data.skinName = material.name;
        Data.skinColor = material.color;
        Data.skinPattern = material.mainTexture != null ? material.mainTexture.name : "";
        Save();
    }

    // Wzór domyślnego skina (pierwszy kolor palety), gdy gracz jeszcze nic nie wybrał.
    private const string DefaultSkinPattern = "Red";

    // Materiał zapisanego skina: kopia materiału domyślnego z zapisanym kolorem i wzorem
    // (paleta Skins rozpoznaje wybrany kolor po wartości, więc kopia jest wystarczająca).
    public static Material RestoreSkin(Material defaultMaterial)
    {
        if (defaultMaterial == null) return null;

        string patternName = Data.hasSkin ? Data.skinPattern : DefaultSkinPattern;
        return new Material(defaultMaterial)
        {
            name = Data.hasSkin ? Data.skinName : defaultMaterial.name,
            color = Data.hasSkin ? Data.skinColor : defaultMaterial.color,
            mainTexture = SkinPatterns.Get(patternName),
            hideFlags = HideFlags.DontUnloadUnusedAsset
        };
    }

    public static bool IsAchievementUnlocked(string title) => Data.achievements.Contains(title);

    public static void SaveAchievement(string title)
    {
        if (string.IsNullOrEmpty(title) || Data.achievements.Contains(title)) return;

        Data.achievements.Add(title);
        Save();
    }
    #endregion
}
