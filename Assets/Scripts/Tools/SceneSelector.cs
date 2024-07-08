#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SceneSelector : EditorWindow
{

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("--== Build Settings Scenes ==--", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        string[] scenes = EditorBuildSettings.scenes.Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path)).ToArray();

        foreach (string scene in scenes)
        {
            if (GUILayout.Button(scene))
            {
                SwitchToScene(scene);
            }
        }
    }

    void SwitchToScene(string sceneName)
    {

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            string scenePath = EditorBuildSettings.scenes.FirstOrDefault(s => System.IO.Path.GetFileNameWithoutExtension(s.path) == sceneName)?.path;
            if (!string.IsNullOrEmpty(scenePath))
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }

    }

    [MenuItem("Tools/Custom Build Settings Window")]
    public static void ShowWindow()
    {
        GetWindow(typeof(SceneSelector), false, "Build Settings");
    }


}
#endif