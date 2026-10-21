using UnityEngine;
using UnityEngine.SceneManagement;

// Rozdzielczość i tryb ekranu.
// Na pełnym ekranie okno gry ma ZAWSZE natywną rozdzielczość monitora - przy innej Windows/sterownik
// przełącza sposób wyświetlania i cały obraz robi się ciemniejszy. Wybrana rozdzielczość działa wtedy
// jako rozdzielczość renderowania sceny 3D (dynamiczna rozdzielczość), a UI zostaje ostre.
// W oknie wybrana rozdzielczość to po prostu rozmiar okna.
// Wybór zapisuje się w save.json i jest przywracany przy starcie gry.
public static class ScreenSettings
{
    public static Vector2Int NativeResolution => new(Display.main.systemWidth, Display.main.systemHeight);

    // Wybrana rozdzielczość (dla pełnego ekranu - rozdzielczość renderowania sceny).
    public static Vector2Int ChosenResolution { get; private set; }
    public static bool Fullscreen { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded += (_, _) => EnableDynamicResolution();

        SaveSystem.SaveData data = SaveSystem.Data;
        bool fullscreen = data.hasScreenSettings ? data.fullscreen : Screen.fullScreen;
        Vector2Int chosen = data.hasScreenSettings && data.resolutionWidth > 0
            ? new Vector2Int(data.resolutionWidth, data.resolutionHeight)
            : new Vector2Int(Screen.width, Screen.height);

        Apply(chosen, fullscreen, false);
    }

    public static void Apply(Vector2Int resolution, bool fullscreen) => Apply(resolution, fullscreen, true);

    private static void Apply(Vector2Int resolution, bool fullscreen, bool save)
    {
        ChosenResolution = resolution;
        Fullscreen = fullscreen;

        float renderScale = 1f;
        if (fullscreen)
        {
            Vector2Int native = NativeResolution;
            if (Screen.fullScreenMode != FullScreenMode.FullScreenWindow || Screen.width != native.x || Screen.height != native.y)
            {
                Screen.SetResolution(native.x, native.y, FullScreenMode.FullScreenWindow);
            }
            renderScale = Mathf.Clamp(resolution.y / (float)native.y, 0.25f, 1f);
        }
        else
        {
            Screen.SetResolution(resolution.x, resolution.y, FullScreenMode.Windowed);
        }

        EnableDynamicResolution();
        ScalableBufferManager.ResizeBuffers(renderScale, renderScale);

        if (save)
        {
            SaveSystem.Data.hasScreenSettings = true;
            SaveSystem.Data.fullscreen = fullscreen;
            SaveSystem.Data.resolutionWidth = resolution.x;
            SaveSystem.Data.resolutionHeight = resolution.y;
            SaveSystem.Save();
        }
    }

    // Skalowanie renderowania działa tylko na kamerach z włączoną dynamiczną rozdzielczością.
    private static void EnableDynamicResolution()
    {
        foreach (Camera camera in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            camera.allowDynamicResolution = true;
        }
    }
}
