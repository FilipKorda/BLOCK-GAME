using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Root : MonoBehaviour
{
    public List<SceneData> scenesToLoad = new();
    public SceneData thisRootSceneToUnload;

    public void Start()
    {
        StartCoroutine(LoadScenes());
    }

    private IEnumerator LoadScenes()
    {
        foreach (var sceneInfo in scenesToLoad)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneInfo.sceneName, LoadSceneMode.Additive);
            yield return asyncLoad;
        }
        SceneManager.UnloadSceneAsync(thisRootSceneToUnload.sceneName);
    }
}